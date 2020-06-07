using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Appserver.Data;
using Appserver.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Common.Models;
using IDD;
using Common.Data;

namespace Appserver.Controllers
{
    public class DocumentSubmissionController : Controller
    {
        /*******************************************************************************
        /// Fields
        *******************************************************************************/
        private readonly SubmissionStagingContext _context;
        private readonly SubmissionContext _subcontext;

        /*******************************************************************************
        /// Constructor
        *******************************************************************************/
        public DocumentSubmissionController(SubmissionStagingContext context, SubmissionContext subcontext)
        {
            _context = context;
            _subcontext = subcontext;
        }


        /*******************************************************************************
        /// Methods
        *******************************************************************************/


        // A previous upload is considered ready when the response from textract has
        // been received. When ready, a json repersentation of the appropriate form is returned.
        [Produces("application/json")]
        [Route("Timesheet/Ready")]
        [HttpGet]
        public IActionResult Ready(int id, string guid)
        {
            var stage = _context.Stagings.FirstOrDefault(m => m.Id == id && m.Guid == guid);

            if (stage == null)
            {
                return Json(new
                {
                    response = "not ready"
                });
            }

            var textractform = new TextractDocument.TextractDocument();

            foreach (var a in JArray.Parse(stage.ParsedTextractJSON))
            {
                var childform = new TextractDocument.TextractDocument();
                childform.FromJson(a);
                textractform.AddPages(childform);
            }

            AbstractFormObject form;

            try
            {
                form = AbstractFormObject.FromTextract(textractform, stage.formType);
            }
            catch (Exception)
            {
                return Json(new 
                { 
                    response = "invalid" 
                });
            }

            form.id = id;
            form.guid = guid;

            return Json(form);
        }

        // Takes a Timesheet submitted by the PWA after the user has had a chance
        // to review the processed upload and possibly make edits. Converts the
        // PWATimesheet model into a Timesheet model which can be placed into the submissions
        // table. 
        [Produces("application/json")]
        [Route("Timesheet/Submit")]
        [HttpPost("Submit")]
        public IActionResult SubmitTimesheet([FromBody] PWATimesheet submittedform)
        {
            // Check correct authorization
            var result = _context.Stagings.FirstOrDefault(m => m.Id == submittedform.id && m.Guid == submittedform.guid);
            if( result == null)
            {
                return Json(new
                {
                    response = "invalid"
                });
            }

            var dbutil = new FormToDbUtil(_subcontext, _context);
            Timesheet ts = dbutil.PopulateTimesheet(submittedform);

            var submission = _subcontext;
            using (var transaction = submission.Database.BeginTransaction())
            {
                submission.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Submissions ON");
                submission.Add(ts);
                submission.SaveChanges();
                transaction.Commit();
            }

            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            return Json(new {response="ok"});
        }


        // Takes a submission from the PWA modeled by PWAMileage, converts that
        // into a MileageForm (the model used the DB), and saves it to
        // the Submissions table.
        [Produces("application/json")]
        [Route("Timesheet/SubmitMileage")]
        [HttpPost("Submit")]
        public IActionResult SubmitMileage([FromBody] PWAMileage submittedform)
        {
            var dbutil = new FormToDbUtil(_subcontext, _context);
            MileageForm mf = dbutil.PopulateMileage(submittedform);

            var submission = _subcontext;
            using (var transaction = submission.Database.BeginTransaction())
            {
                submission.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Submissions ON");
                submission.Add(mf);
                submission.SaveChanges();
                transaction.Commit();
            }

            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            return Json(new { response = "ok" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
