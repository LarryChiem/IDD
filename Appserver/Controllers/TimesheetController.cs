using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Appserver.Data;
using Appserver.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Appserver.TextractDocument;
using Newtonsoft.Json.Linq;
using Common.Models;
using IDD;
using Common.Data;

namespace Appserver.Controllers
{
    public class TimesheetController : Controller
    {
        /*******************************************************************************
        /// Fields
        *******************************************************************************/
        private readonly SubmissionStagingContext _context;
        private readonly SubmissionContext _subcontext;

        /*******************************************************************************
        /// Constructor
        *******************************************************************************/
        public TimesheetController(SubmissionStagingContext context, SubmissionContext subcontext)
        {
            _context = context;
            _subcontext = subcontext;
        }


        /*******************************************************************************
        /// Methods
        *******************************************************************************/

        [Produces("application/json")]
        [Route("Timesheet/ReadyTest")]
        [HttpGet]
        public IActionResult ReadyTest(int id)
        {
            return Ready(id);
        }
        [Produces("application/json")]
        [HttpGet]
        public IActionResult Ready(int id)
        {
            var stage = _context.Stagings.FirstOrDefault(m => m.Id == id);

            if (stage == null)
            {
                return Json(new JsonResponse("not ready"));
            }
            var textractform = new TextractDocument.TextractDocument();

            textractform.FromJson(JObject.Parse(stage.ParsedTextractJSON.Trim(',')));
            TimesheetForm ts;
            try
            {
                ts = (TimesheetForm)AbstractFormObject.FromTextract(textractform);
            }
            catch (Exception)
            {
                return Json(new JsonResponse("invalid"));
            }

            ts.id = id;

            return Json(ts);
        }

        private class JsonResponse
        {
            public JsonResponse(string res = "ok")
            {
                response = res;
            }
            public string response;
        }

        [Route("Timesheet/Validate")]
        [HttpPost("Validate")]
        [Produces("application/json")]
        public IActionResult Validate(TimesheetForm ?form )
        {
            // Do something with form

            JsonResponse model = new JsonResponse();
            return Json(model);
        }


        [Route("Timesheet/SubmitTest")]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult Submit(int id)
        {
            var stage = _context.Stagings.FirstOrDefault(m => m.Id == id);

            if (stage == null)
            {
                return Json(new JsonResponse("not ready"));
            }
            var textractform = new TextractDocument.TextractDocument();

            textractform.FromJson(JObject.Parse(stage.ParsedTextractJSON.Trim(',')));

            var tsf = (TimesheetForm)AbstractFormObject.FromTextract(textractform);

            tsf.id = id;

            var dbutil = new FormToDbUtil(_subcontext, _context);
            Timesheet ts = dbutil.PopulateTimesheet(tsf);
            dbutil.PopulateTimesheetEntries(tsf, ts);

            var submission = _subcontext;
            submission.Add(ts);
            submission.SaveChanges();
            
            // Do something with form
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            return Json(new { response = "ok" });
        }

        [Route("Timesheet/Submit")]
        [HttpPost("Submit")]
        [Produces("application/json")]
        public IActionResult Submit([FromBody] PWAsubmission submittedform)
        {
           
            var dbutil = new FormToDbUtil(_subcontext, _context);
            TimesheetForm tsf = dbutil.PWAtoTimesheetFormConverter(submittedform);
            Timesheet ts = dbutil.PopulateTimesheet(tsf);
            dbutil.PopulateTimesheetEntries(tsf, ts);

            var submission = _subcontext;
            submission.Add(ts);
            submission.SaveChanges();

            // Do something with form
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            return Json(new {response="ok"});
        }

        [Produces("application/json")]
        public IActionResult Received()
        {
            JsonResponse model = new JsonResponse();
            return Json(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
