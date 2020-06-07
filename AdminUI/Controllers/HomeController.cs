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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Lock = Common.Models.Lock;

namespace AdminUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SubmissionContext _context;
        private readonly bool _payPeriodWarning;
        private readonly UserManager<AdminUIUser> _userManager;

        public HomeController(ILogger<HomeController> logger, SubmissionContext submissionContext, PayPeriodContext payPeriodContext, UserManager<AdminUIUser> userManager)
        {
            _logger = logger;
            _context = submissionContext;
            _payPeriodWarning = payPeriodContext.PayPeriods.Count() < 3;
            _userManager = userManager;
        }

        /*
         * Index is the main home page of the AdminUI
         * Parameters: A sortOrder for the table, filters for the table, and page number and perPage counts for pagination
         * Returns the Home Index
         */
        public IActionResult Index(string sortOrder = "id", string pName="", string cName="", string dateFrom="", string dateTo="", string prime="", string providerId="", 
            string status="pending", int page = 1, int perPage = 20, string formType="timesheet")
        {
            var submissions = GetSubmissions(formType, pName, providerId, cName, prime, dateFrom, dateTo, status);

            var model = new HomeModel
            {
                FormType = formType,
                PName = pName,
                ProviderId = providerId,
                CName = cName,
                Prime = prime,
                Status = status,
                SortOrder = sortOrder,
                Page = page,
                PerPage = perPage
            };

            if (!string.IsNullOrEmpty(dateFrom))
                model.DateFrom = dateFrom;
            else if (GlobalVariables.CurrentPayPeriod != null)
                model.DateFrom = GlobalVariables.CurrentPayPeriod.DateFrom.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(dateTo))
                model.DateTo = dateTo;
            else if (GlobalVariables.CurrentPayPeriod != null)
                model.DateTo = GlobalVariables.CurrentPayPeriod.DateTo.ToString("yyyy-MM-dd");

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

            model.TotalSubmissions = submissions.Count();
            model.TotalPages = model.TotalSubmissions / perPage + (model.TotalSubmissions % perPage == 0 ? 0 : 1);
            model.Submissions = new List<Submission>(submissions.Skip((page - 1) * perPage).Take(perPage));

            foreach (var sub in model.Submissions)
            {
                _context.Entry(sub).Reference(s => s.LockInfo).Load();
                sub.LoadEntries(_context);
            }

            model.Warning = _payPeriodWarning;
            model.Filters = _userManager.Users.Include(u=>u.Filters).Single(u=>u.UserName == User.Identity.Name).Filters;
            return View("Index", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /*
         * GetSubmissions queries the submission database with the provided filters. Only the Form Type is required
         * Parameters: The 8 different filters
         * Returns: The list of submissions matching the query
         */
        private IEnumerable<Submission> GetSubmissions(string formType, string providerName ="", string providerId ="", string clientName ="",
            string clientPrime ="", string dateFrom ="", string dateTo ="", string status ="")
        {
            IQueryable<Submission> submissions;
            
            if (formType.Equals("timesheet",StringComparison.CurrentCultureIgnoreCase))
                submissions = _context.Timesheets;
            else
                submissions = _context.MileageForms;
            
            if (!string.IsNullOrEmpty(providerName))
                submissions = submissions.Where(s => s.ProviderName.Contains(providerName));

            if (!string.IsNullOrEmpty(providerId))
                submissions = submissions.Where(s => s.ProviderId.Contains(providerId));

            if (!string.IsNullOrEmpty(clientName))
                submissions = submissions.Where(s => s.ClientName.Contains(clientName));
            
            if (!string.IsNullOrEmpty(clientPrime))
                submissions = submissions.Where(s => s.ClientPrime.Contains(clientPrime));
            
            if (!string.IsNullOrEmpty(dateFrom))
                submissions = submissions.Where(s => s.Submitted >= DateTime.Parse(dateFrom));
            else if (GlobalVariables.CurrentPayPeriod != null)
                submissions = submissions.Where(s => s.Submitted >= GlobalVariables.CurrentPayPeriod.DateFrom);
            
            if (!string.IsNullOrEmpty(dateTo))
                submissions = submissions.Where(s => s.Submitted <= DateTime.Parse(dateTo).AddDays(1));
            else if (GlobalVariables.CurrentPayPeriod != null)
                submissions = submissions.Where(s => s.Submitted <= GlobalVariables.CurrentPayPeriod.DateTo.AddDays(1));

            if(!string.Equals(status,"all",StringComparison.CurrentCultureIgnoreCase))
                submissions = submissions.Where(s => s.Status == status);

            return submissions.ToList();
        }

        /*
         * GetLockInfo() retrieves the LockInfo of a submission from the database. If none exists, it creates on and assigns it to the current user
         * Parameters: ID of the submission
         * Returns true if the current user holds the lock
         */
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

        /*
         * Releases the Lock if the current User is holding the lock
         * Parameters: The ID of the submission
         * Returns nothing
         */
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
        /*
         * DownloadCSV() should build a CSV summary of all sheets matching the filters
         * Parameters: Submission filters
         * Returns a CSV file
         */
        public FileContentResult DownloadCSV(string pName, string cName, string dateFrom, string dateTo, string prime, string status, string formType, string providerId)
        {
            var submissions = GetSubmissions(formType, pName, providerId, cName, prime, dateFrom, dateTo, status);


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

        /*
         * DownloadPDFs creates a zip file of every PDF matching the filters
         * Parameters: Submission filters
         * Returns a Zip file containing PDFs
         */
        public FileContentResult DownloadPDFs(string pName, string cName, string dateFrom, string dateTo, string prime, string status, string formType, string providerId)
        {
            var submissions = GetSubmissions(formType, pName, providerId, cName, prime, dateFrom, dateTo, status);

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

        /*
         * SaveFilter saves a filter and assigns it to a user
         * Parameters: The name of the filters and the 8 fields that make a filter
         * Returns the Home Index with the filter applied
         */
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
        /*
         * DeleteFilter removes a filter from the list of User Filters
         * Parameters: The ID of the filter
         * Returns the default Home Index
         */
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
