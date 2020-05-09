using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AdminUI.Models;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Common.Data;

namespace AdminUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SubmissionContext _scontext;

        public HomeController(ILogger<HomeController> logger, SubmissionContext scontext)
        {
            _logger = logger;
            _scontext = scontext;
        }

        public IActionResult Index(string sortOrder = "id", string pName="", string cName="", string dateFrom="", string dateTo="", string prime="", string id="", string ProviderId="", string status="pending", int page = 1, int perPage = 20)
        {
            var submissions = GetTimesheets();

            var model = new HomeModel();
            
            //filter the timesheets 
            if (!string.IsNullOrEmpty(pName))
            {
                model.PName = pName;
                submissions = submissions.Where(t => t.ProviderName.ToLower().Contains(pName.ToLower()));
            }
            if (!string.IsNullOrEmpty(ProviderId))
            {
                model.ProviderId = ProviderId;
                submissions = submissions.Where(t => t.ProviderId.ToLower().Contains(ProviderId.ToLower()));
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
            if (!string.IsNullOrEmpty(dateTo))
            {
                model.DateTo = dateTo;
                submissions = submissions.Where(t => t.Submitted <= DateTime.Parse(dateTo));
            }
            if (!string.IsNullOrEmpty(id))
            {
                model.Id = int.Parse(id);
                submissions = submissions.Where(t => t.Id == int.Parse(id));
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
                "hours" => submissions.OrderBy(t => t.TotalHours),
                "hours_desc" => submissions.OrderByDescending(t => t.TotalHours),
                "ProviderId" => submissions.OrderBy(t => t.ProviderId),
                "ProviderId_desc" => submissions.OrderByDescending(t => t.ProviderId),
                _ => submissions.OrderBy(t => t.Id),
            };
            model.SortOrder = sortOrder;
            model.TotalSubmissions = submissions.Count();

            model.TotalPages = submissions.Count() / perPage + (submissions.Count() % perPage == 0 ? 0 : 1);
            submissions = submissions.Skip((page - 1) * perPage).Take(perPage);
            model.PerPage = perPage;
            model.Page = page;

            foreach (var s in submissions)
                _scontext.Entry(s).Collection(t => t.TimeEntries).Load();
            model.Timesheets = new List<Timesheet>(submissions);
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IEnumerable<Timesheet> GetTimesheets()
        {
            return _scontext.Timesheets
                .Include(t => t.TimeEntries)
                .AsEnumerable();
        }

        //should return a timesheet View
        public async Task<IActionResult> Timesheet(int id)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                return View("../submission/NoPermission");
            
            var submission =  _scontext.Timesheets.Find(id);
                
            _scontext.Entry(submission).Reference(t => t.LockInfo).Load();
            _scontext.Entry(submission).Collection(t => t.TimeEntries).Load();

            if (submission.LockInfo != null) return View(submission);

            submission.LockInfo = new Lock
                {
                    LastActivity = DateTime.Now,
                    User = User.Identity.Name
                };


            _scontext.Update(submission);
            await _scontext.SaveChangesAsync();

            return View(submission);
        }

        public FileContentResult DownloadCSV(string pName, string cName, string dateFrom, string dateTo, string prime, string id, string status)
        {
            var submissions = GetTimesheets();

            //filter the timesubmissions 
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

    }
}
