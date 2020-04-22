using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AdminUI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AdminUI.Models;
using Amazon.DeviceFarm.Model;
using Amazon.DirectConnect.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;


namespace AdminUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TimesheetContext _Tcontext;
        private readonly LockTableContext _Lcontext;

        public HomeController(ILogger<HomeController> logger, TimesheetContext Tcontext, LockTableContext Lcontext)
        {
            _Tcontext = Tcontext;
            _logger = logger;
            _Lcontext = Lcontext;
        }

        public IActionResult Index(string sortOrder, string pName, string cName, string dateFrom, string dateTo, string prime, string id, string providerId, string status)
        {
            var sheets = GetSheets();

            var model = new HomeModel();
            
            //filter the timesheets 
            if (!string.IsNullOrEmpty(pName))
            {
                model.PName = pName;
                sheets = sheets.Where(t => t.ProviderName.ToLower().Contains(pName.ToLower()));
            }
            if (!string.IsNullOrEmpty(providerId))
            {
                model.ProviderId = providerId;
                sheets = sheets.Where(t => t.ProviderID.ToLower().Contains(providerId.ToLower()));
            }

            if (!string.IsNullOrEmpty(cName))
            {
                model.CName = cName;
                sheets = sheets.Where(t => t.ClientName.ToLower().Contains(cName.ToLower()));
            }
            if (!string.IsNullOrEmpty(prime))
            {
                model.Prime = prime;
                sheets = sheets.Where(t => t.ClientPrime.Contains(prime, StringComparison.CurrentCultureIgnoreCase));
            }
            if (!string.IsNullOrEmpty(dateFrom))
            {
                model.DateFrom = dateFrom;
                sheets = sheets.Where(t => t.Submitted >= DateTime.Parse(dateFrom));
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                model.DateTo = dateTo;
                sheets = sheets.Where(t => t.Submitted <= DateTime.Parse(dateTo));
            }
            if (!string.IsNullOrEmpty(id))
            {
                model.Id = int.Parse(id);
                sheets = sheets.Where(t => t.TimesheetID == int.Parse(id));
            }
            
            if (string.IsNullOrEmpty(status))
                status = "pending";
            
            if(!string.Equals(status,"all",StringComparison.CurrentCultureIgnoreCase))
                sheets = sheets.Where(t => t.Status.Equals(status,StringComparison.CurrentCultureIgnoreCase));
            
            model.Status = status;

            //big ol' switch statement determines how to sort the data in the table
            sheets = sortOrder switch
            {
                "id" => sheets.OrderBy(t => t.TimesheetID),
                "id_desc" => sheets.OrderByDescending(t => t.TimesheetID),
                "pname" => sheets.OrderBy(t => t.ProviderName),
                "pname_desc" => sheets.OrderByDescending(t => t.ProviderName),
                "prime" => sheets.OrderBy(t => t.ClientPrime),
                "prime_desc" => sheets.OrderByDescending(t => t.ClientPrime),
                "cname" => sheets.OrderBy(t => t.ClientName),
                "cname_desc" => sheets.OrderByDescending(t => t.ClientName),
                "date" => sheets.OrderBy(t => t.Submitted),
                "date_desc" => sheets.OrderByDescending(t => t.Submitted),
                "hours" => sheets.OrderBy(t => t.Hours),
                "hours_desc" => sheets.OrderByDescending(t => t.Hours),
                "providerid" => sheets.OrderBy(t => t.ProviderID),
                "providerid_desc" => sheets.OrderByDescending(t => t.ProviderID),
                _ => sheets.OrderBy(t => t.TimesheetID),
            };
            model.SortOrder = sortOrder;

            foreach (var t in sheets)
            {
                t.Shifts = new List<Shift>
                {
                    new Shift
                    {
                        Date = DateTime.Parse("3/11/2020"),
                        In = DateTime.Parse("11:30 AM"),
                        Out = DateTime.Parse("7:30 PM"),
                        Hours = 8.00,
                        Group = false
                    },
                    new Shift
                    {
                        Date = DateTime.Parse("3/11/2020"),
                        In = DateTime.Parse("11:30 AM"),
                        Out = DateTime.Parse("7:30 PM"),
                        Hours = 8.00,
                        Group = false
                    },
                    new Shift
                    {
                        Date = DateTime.Parse("3/11/2020"),
                        In = DateTime.Parse("11:30 AM"),
                        Out = DateTime.Parse("7:30 PM"),
                        Hours = 8.00,
                        Group = false
                    },
                    new Shift
                    {
                        Date = DateTime.Parse("3/11/2020"),
                        In = DateTime.Parse("11:30 AM"),
                        Out = DateTime.Parse("7:30 PM"),
                        Hours = 8.00,
                        Group = false
                    },
                    new Shift
                    {
                        Date = DateTime.Parse("3/11/2020"),
                        In = DateTime.Parse("11:30 AM"),
                        Out = DateTime.Parse("7:30 PM"),
                        Hours = 8.00,
                        Group = false
                    },
                    new Shift
                    {
                        Date = DateTime.Parse("3/12/2020"),
                        In = DateTime.Parse("11:30 AM"),
                        Out = DateTime.Parse("7:30 PM"),
                        Hours = 8.00,
                        Group = false
                    }
                };
            }


            model.Sheets = new List<Timesheet>(sheets);
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IEnumerable<Timesheet> GetSheets()
        {
            return  _Tcontext.Timesheet.AsEnumerable();
        }

        //should return a Timesheet View
        public async Task<IActionResult> Timesheet(int ID)
        {
            var timesheet =  _Tcontext.Timesheet.Find(ID);
            timesheet.Shifts = new List<Shift>
            {
                new Shift
                {
                    Date = DateTime.Parse("3/11/2020"),
                    In = DateTime.Parse("11:30 AM"),
                    Out = DateTime.Parse("7:30 PM"),
                    Hours = 8.00,
                    Group = false
                },
                new Shift
                {
                    Date = DateTime.Parse("3/12/2020"),
                    In = DateTime.Parse("11:30 AM"),
                    Out = DateTime.Parse("7:30 PM"),
                    Hours = 8.00,
                    Group = false
                }
            };
                
            timesheet.Lock = _Lcontext.LockTableRow.FirstOrDefault(r => r.TimesheetID == ID);

            if (timesheet.Lock != null) return View(timesheet);

            timesheet.Lock = new LockTableRow
                {
                    TimesheetID = ID,
                    LastInteraction = DateTime.Now,
                    TimeLocked = DateTime.Now,
                    User = User.Identity.Name
                };

            _Lcontext.Add(timesheet.Lock);
            await _Lcontext.SaveChangesAsync();

            return View(timesheet);
        }

        public FileContentResult DownloadCSV(string pName, string cName, string dateFrom, string dateTo, string prime, string id, string status)
        {
            var sheets = GetSheets();

            
            //filter the timesheets 
            if (!string.IsNullOrEmpty(pName))
                sheets = sheets.Where(t => t.ProviderName.ToLower().Contains(pName.ToLower()));

            if (!string.IsNullOrEmpty(cName))
                sheets = sheets.Where(t => t.ClientName.ToLower().Contains(cName.ToLower()));

            if (!string.IsNullOrEmpty(dateFrom))
                sheets = sheets.Where(t => t.Submitted >= DateTime.Parse(dateFrom));

            if (!string.IsNullOrEmpty(dateTo))
                sheets = sheets.Where(t => t.Submitted <= DateTime.Parse(dateTo));

            if (!string.IsNullOrEmpty(prime))
                sheets = sheets.Where(t => t.ClientPrime == prime);

            if (!string.IsNullOrEmpty(id))
                sheets = sheets.Where(t => t.TimesheetID == int.Parse(id));

            if (string.IsNullOrEmpty(status))
                status = "pending";

            if(!string.Equals(status,"all",StringComparison.CurrentCultureIgnoreCase))
                sheets = sheets.Where(t => t.Status.Equals(status,StringComparison.CurrentCultureIgnoreCase));

            //the following loops through every property in a timesheet, first saving the names of the properties to 
            //act as a header. Then, it loops through every timesheet, adding every individual property of the timesheet
            //to the csv, then returning it for download.
            var properties = typeof(Timesheet).GetProperties();
            var csv = properties.Aggregate("", (current, f) => current + (f.Name + ','));
            foreach (var s in sheets)
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
