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

            foreach (var a in JArray.Parse(stage.ParsedTextractJSON))
            {
                var childform = new TextractDocument.TextractDocument();
                childform.FromJson(a);
                textractform.AddPages(childform);
            }

            AbstractFormObject ts;

            try
            {
                ts = AbstractFormObject.FromTextract(textractform, stage.formType);
            }
            catch (Exception)
            {
                return Json(new JsonResponse("invalid"));
            }

            ts.id = id;

            return Json(ts);
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

            foreach (var a in JArray.Parse(stage.ParsedTextractJSON))
            {
                var childform = new TextractDocument.TextractDocument();
                childform.FromJson(a);
                textractform.AddPages(childform);
            }
            var ts = AbstractFormObject.FromTextract(textractform, stage.formType);

            ts.id = id;

            var PWAForm = PWAsubmission.FromForm(ts, stage.formType);
            switch (stage.formType) {
                case AbstractFormObject.FormType.OR507_RELIEF:
                    return SubmitTimesheet((PWATimesheet)PWAForm);
                case AbstractFormObject.FormType.OR004_MILEAGE:
                    return SubmitMileage((PWAMileage)PWAForm);
                default:
                    return Json(new
                    {
                        response = "invalid"
                    }
            );
            }
        }

        [Route("Timesheet/Submit")]
        [HttpPost("Submit")]
        [Produces("application/json")]
        public IActionResult SubmitTimesheet([FromBody] PWATimesheet submittedform)
        {
            var dbutil = new FormToDbUtil(_subcontext, _context);
            Timesheet ts = dbutil.PopulateTimesheet(submittedform);

            var submission = _subcontext;
            submission.Add(ts);
            submission.SaveChanges();

            // Do something with form
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            return Json(new {response="ok"});
        }


        [Route("Timesheet/SubmitMileage")]
        [HttpPost("Submit")]
        [Produces("application/json")]
        public IActionResult SubmitMileage([FromBody] PWAMileage submittedform)
        {
            var dbutil = new FormToDbUtil(_subcontext, _context);
            MileageForm mf = dbutil.PopulateMileage(submittedform);

            var submission = _subcontext;
            submission.Add(mf);
            submission.SaveChanges();

            // Do something with form
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            return Json(new { response = "ok" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        /*******************************************************************************
        /// Classes
        *******************************************************************************/
        private class JsonResponse
        {
            public JsonResponse(string res = "ok")
            {
                response = res;
            }
            public string response;
        }
    }
}
