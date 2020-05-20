using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AdminUI.Areas.Identity.Data;
using AdminUI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AdminUI.Models;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PdfSharp.Pdf;
using SQLitePCL;
using Lock = Common.Models.Lock;

namespace AdminUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SubmissionContext _context;
        private readonly PayPeriodContext _pcontext;
        private readonly UserManager<AdminUIUser> _userManager;

        public HomeController(ILogger<HomeController> logger, SubmissionContext context, PayPeriodContext pcontext, UserManager<AdminUIUser> userManager)
        {
            _logger = logger;
            _context = context;
            _pcontext = pcontext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string sortOrder = "id", string pName="", string cName="", string dateFrom="", string dateTo="", string prime="", string providerId="", string status="pending", int page = 1, int perPage = 20, string formType="timesheet")
        {
            var submissions = GetSubmissions(formType);

            var model = new HomeModel{FormType = formType};
            
            //filter the timesheets 
            if (!string.IsNullOrEmpty(pName))
            {
                model.PName = pName;
                submissions = submissions.Where(t => t.ProviderName.ToLower().Contains(pName.ToLower()));
            }
            if (!string.IsNullOrEmpty(providerId))
            {
                model.ProviderId = providerId;
                submissions = submissions.Where(t => t.ProviderId.ToLower().Contains(providerId.ToLower()));
            }

            if (!string.IsNullOrEmpty(cName))
            {
                model.CName = cName;
                submissions = submissions.Where(t => t.ClientName.ToLower().Contains(cName.ToLower()));
            }
            if (!string.IsNullOrEmpty(prime))
            {
                model.Prime = prime;
                submissions = submissions.Where(t => t.ClientPrime.Contains(prime, StringComparison.CurrentCultureIgnoreCase));
            }
            if (!string.IsNullOrEmpty(dateFrom))
            {
                model.DateFrom = dateFrom;
                submissions = submissions.Where(t => t.Submitted >= DateTime.Parse(dateFrom));
            }
            else if (GlobalVariables.CurrentPayPeriod != null)
            {
                model.DateFrom = GlobalVariables.CurrentPayPeriod.DateFrom.ToString("yyyy-MM-dd");
                submissions = submissions.Where(t => t.Submitted >= GlobalVariables.CurrentPayPeriod.DateFrom);
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                model.DateTo = dateTo;
                submissions = submissions.Where(t => t.Submitted <= DateTime.Parse(dateTo).AddDays(1));
            }
            else if (GlobalVariables.CurrentPayPeriod != null)
            {
                model.DateTo = GlobalVariables.CurrentPayPeriod.DateTo.ToString("yyyy-MM-dd");
                submissions = submissions.Where(t => t.Submitted <= GlobalVariables.CurrentPayPeriod.DateTo.AddDays(1));
            }

            if(!string.Equals(status,"all",StringComparison.CurrentCultureIgnoreCase))
                submissions = submissions.Where(t => t.Status.Equals(status,StringComparison.CurrentCultureIgnoreCase));

            
            model.Status = status;

            //big ol' switch statement determines how to sort the data in the table
            submissions = sortOrder switch
            {
                "id" => submissions.OrderBy(t => t.Id),
                "id_desc" => submissions.OrderByDescending(t => t.Id),
                "pname" => submissions.OrderBy(t => t.ProviderName),
                "pname_desc" => submissions.OrderByDescending(t => t.ProviderName),
                "prime" => submissions.OrderBy(t => t.ClientPrime),
                "prime_desc" => submissions.OrderByDescending(t => t.ClientPrime),
                "cname" => submissions.OrderBy(t => t.ClientName),
                "cname_desc" => submissions.OrderByDescending(t => t.ClientName),
                "date" => submissions.OrderBy(t => t.Submitted),
                "date_desc" => submissions.OrderByDescending(t => t.Submitted),
                "ProviderId" => submissions.OrderBy(t => t.ProviderId),
                "ProviderId_desc" => submissions.OrderByDescending(t => t.ProviderId),
                _ => submissions.OrderBy(t => t.Id),
            };

            model.SortOrder = sortOrder;
            model.TotalSubmissions = submissions.Count();
            model.TotalPages = model.TotalSubmissions / perPage + (model.TotalSubmissions % perPage == 0 ? 0 : 1);
            submissions = submissions.Skip((page - 1) * perPage).Take(perPage);
            model.PerPage = perPage;
            model.Page = page;

            foreach (var sub in submissions)
            {
                _context.Entry(sub).Reference(s => s.LockInfo).Load();
                sub.LoadEntries(_context);
            }

            model.Submissions = new List<Submission>(submissions);
            model.Warning = _pcontext.PayPeriods.Count() < 3;
            model.Filters = _userManager.Users.Include(u=>u.Filters).Single(u=>u.UserName == User.Identity.Name).Filters;
            return View(formType + "Index", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IEnumerable<Submission> GetSubmissions(string formType)
        {
            if (formType.Equals("timesheet"))
                return _context.Timesheets.ToList();
            return _context.MileageForms.ToList();
        }

        public bool GetLockInfo(int id)
        {
            var submission = _context.Submissions.Find(id);
            _context.Entry(submission).Reference(t => t.LockInfo).Load();

            //if lock exists, disable processing and indicate sheet is locked
            //else no lock, create lock.
            if (submission.LockInfo == null)
            {

                submission.LockInfo = new Lock
                {
                    LastActivity = DateTime.Now,
                    User = User.Identity.Name
                };

                _context.Update(submission);
                _context.SaveChanges();
            }

            return submission.LockInfo.User.Equals(User.Identity.Name);
        }

        //Releases the Lock if the current User is holding the lock
        public void ReleaseLock(int id)
        {
            var submission = _context.Submissions.Find(id);
            _context.Entry(submission).Reference(t => t.LockInfo).Load();

            if (submission.LockInfo.User.Equals(User.Identity.Name))
            {
                submission.LockInfo = null;
                _context.Update(submission);
                _context.SaveChanges();
            }
        }

        //TODO: Work with Gloria to figure out exactly what the CSV should look like, then fix this and re-enable button
        public FileContentResult DownloadCSV(string pName, string cName, string dateFrom, string dateTo, string prime, string id, string status, string formType, string providerId)
        {
            var submissions = GetSubmissions(formType);

            //filter the submissions 
            if (!string.IsNullOrEmpty(pName))
                submissions = submissions.Where(t => t.ProviderName.ToLower().Contains(pName.ToLower()));

            if (!string.IsNullOrEmpty(cName))
                submissions = submissions.Where(t => t.ClientName.ToLower().Contains(cName.ToLower()));

            if (!string.IsNullOrEmpty(dateFrom))
                submissions = submissions.Where(t => t.Submitted >= DateTime.Parse(dateFrom));

            if (!string.IsNullOrEmpty(dateTo))
                submissions = submissions.Where(t => t.Submitted <= DateTime.Parse(dateTo));

            if (!string.IsNullOrEmpty(prime))
                submissions = submissions.Where(t => t.ClientPrime == prime);
            
            if (!string.IsNullOrEmpty(providerId))
                submissions = submissions.Where(t => t.ClientPrime == providerId);

            if (!string.IsNullOrEmpty(id))
                submissions = submissions.Where(t => t.Id == int.Parse(id));

            if (string.IsNullOrEmpty(status))
                status = "pending";

            if(!string.Equals(status,"all",StringComparison.CurrentCultureIgnoreCase))
                submissions = submissions.Where(t => t.Status.Equals(status,StringComparison.CurrentCultureIgnoreCase));

            //the following loops through every property in a submission, first saving the names of the properties to 
            //act as a header. Then, it loops through every submission, adding every individual property of the submission
            //to the csv, then returning it for download.
            
            //var properties = typeof(Timesheet).GetProperties();
            var properties = submissions.GetType().GetGenericArguments()[0].GetProperties();
            var csv = properties.Aggregate("", (current, f) => current + (f.Name + ','));
            foreach (var s in submissions)
            {
                csv += '\n';
                foreach (var p in properties)
                {
                    
                    if (p.GetValue(s) != null)
                        csv += "\"" + p.GetValue(s).ToString().Replace("\"","\"\"") + "\"";
                    csv += ',';
                }
            }
            var name = "Submissions_summary_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") + ".csv";
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", name);
        }

        public FileContentResult DownloadPDFs(string pName, string cName, string dateFrom, string dateTo, string prime,
            string id, string status, string formType, string providerId)
        {

            var submissions = GetSubmissions(formType);

            //filter the submissions 
            if (!string.IsNullOrEmpty(pName))
                submissions = submissions.Where(t => t.ProviderName.ToLower().Contains(pName.ToLower()));

            if (!string.IsNullOrEmpty(cName))
                submissions = submissions.Where(t => t.ClientName.ToLower().Contains(cName.ToLower()));

            if (!string.IsNullOrEmpty(dateFrom))
                submissions = submissions.Where(t => t.Submitted >= DateTime.Parse(dateFrom));

            if (!string.IsNullOrEmpty(dateTo))
                submissions = submissions.Where(t => t.Submitted <= DateTime.Parse(dateTo));

            if (!string.IsNullOrEmpty(prime))
                submissions = submissions.Where(t => t.ClientPrime == prime);

            if (!string.IsNullOrEmpty(providerId))
                submissions = submissions.Where(t => t.ClientPrime == providerId);
            
            if (!string.IsNullOrEmpty(id))
                submissions = submissions.Where(t => t.Id == int.Parse(id));

            if (string.IsNullOrEmpty(status))
                status = "pending";

            if (!string.Equals(status, "all", StringComparison.CurrentCultureIgnoreCase))
                submissions = submissions.Where(t => t.Status.Equals(status, StringComparison.CurrentCultureIgnoreCase));

            var ms = new MemoryStream();
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {

                foreach (var submission in submissions)
                {
                    //ClientName_Prime_ProviderID_ProviderName_yyyyMMdd_FormNumber
                    var fileDownloadName = (submission.ClientName + "_" + submission.ClientPrime + "_" + submission.ProviderId + "_" +
                                           submission.ProviderName + "_" + submission.Submitted.ToString("yyyyMMdd") + "_" + 
                                           submission.FormType.Split(" ")[0] + ".pdf").Replace("/",string.Empty).Replace(" ",string.Empty);
                    submission.LoadEntries(_context);
                    var zipEntry = archive.CreateEntry(fileDownloadName, CompressionLevel.Fastest);
                    using var zipStream = zipEntry.Open();
                    var pdfStream = new MemoryStream();
                    submission.ToPdf().Save(pdfStream, false);
                    zipStream.Write(pdfStream.ToArray(), 0, (int) pdfStream.Length);
                    zipStream.Close();
                }
            }

            return File(ms.ToArray(), "application/zip", DateTime.Now.ToString("yyyy-M-dd") + "_" + formType + "_pdfs" + ".zip");

        }

        public async Task<IActionResult> SaveFilter(string pName, string cName, string dateFrom, string dateTo, string prime,
            string status, string formType, string providerId, string filterName)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user.Filters == null)
                user.Filters = new List<Filter>();
            if (string.IsNullOrEmpty(filterName))
                filterName = "Default Name";
            user.Filters.Add(new Filter
            {
                FilterName = filterName,
                ProviderName = pName,
                ProviderId = providerId,
                ClientName = cName,
                ClientPrime = prime,
                DateFrom = DateTime.Parse(dateFrom),
                DateTo = DateTime.Parse(dateTo),
                Status = status,
                FormType = formType
            }); 
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", new
            {
                ProviderName = pName,
                ProviderId = providerId,
                ClientName = cName,
                ClientPrime = prime,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Status = status,
                FormType = formType
            });
        }
        public async Task<IActionResult> DeleteFilter(int id)
        {
            var user = _userManager.Users.Include(u => u.Filters).Single(u => u.UserName == User.Identity.Name);
            var filter = user.Filters.Single(f => f.Id == id);
            user.Filters.Remove(filter);
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }

    }
}
