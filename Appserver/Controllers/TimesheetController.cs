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

namespace Appserver.Controllers
{
    public class TimesheetController : Controller
    {
        /*******************************************************************************
        /// Fields
        *******************************************************************************/
        private readonly SubmissionStagingContext _context;

        /*******************************************************************************
        /// Constructor
        *******************************************************************************/
        public TimesheetController(SubmissionStagingContext context)
        {
            _context = context;
        }


        /*******************************************************************************
        /// Methods
        *******************************************************************************/

        [Produces("application/json")]
        [Route("Timesheet/ReadyTest")]
        public IActionResult ReadyTest()
        {
            // get the response from db
            int id = 2;
            var stage = _context.Stagings.FirstOrDefault(m => m.Id == id);

            if( stage == null)
            {
                return Json(new JsonResponse("not ready"));
            }
            var textractform = new TextractDocument.TextractDocument();
            
            textractform.FromJson(JObject.Parse(stage.ParsedTextractJSON.Trim(',')));

            var ts = AbstractFormObject.FromTextract(textractform);
            
            return Json(ts);
        }
        [Produces("application/json")]
        public IActionResult Ready()
        {
            TimesheetForm model = new TimesheetForm();
            model.prime = "A1234";
            model.providerName = "Donald Duck";
            model.providerNum = "N6543";
            model.providerSignature = true;
            model.providerSignDate = DateTime.Now.ToString();
            model.progressNotes = "Looking good for a retired hero.\nNeeds a new hobby.";
            model.scpaName = "SCPA";
            model.serviceAuthorized = "Feeding";
            model.serviceGoal = "Feed fish";
            model.authorization = true;
            model.type = "House call";
            model.brokerage = "Daffy";
            model.approval = true;
            model.clientName = "Darkwing Duck";
            model.employerSignature = true;
            model.employerSignDate = DateTime.Now.ToString();
            model.frequency = "Daily";
            model.addTimeRow("2020-04-02", "09:00", "10:00", 1.0f, 1);
            model.addTimeRow("2020-04-03", "09:00", "10:00", 1.0f, 1);
            model.addTimeRow("2020-04-04", "09:00", "10:00", 1.0f, 1);
            return Json(model);
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

        [Route("Timesheet/Validate")]
        [HttpPost("Submit")]
        [Produces("application/json")]
        public IActionResult Submit(TimesheetForm ?form)
        {
            // Do something with form
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            JsonResponse model = new JsonResponse();
            return Json(model);
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
