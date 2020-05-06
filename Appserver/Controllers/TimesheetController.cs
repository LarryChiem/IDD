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

            var ts = (TimesheetForm)AbstractFormObject.FromTextract(textractform);

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
