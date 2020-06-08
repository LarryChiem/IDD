#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appserver.Data;
using Appserver.Models;
using Common.Data;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Appserver.Controllers
{
    public class TestController : Controller
    {
        /*******************************************************************************
        /// Fields
        *******************************************************************************/
        private readonly SubmissionStagingContext _context;
        private readonly SubmissionContext _subcontext;

        /*******************************************************************************
        /// Constructor
        *******************************************************************************/
        public TestController(SubmissionStagingContext context, SubmissionContext subcontext)
        {
            _context = context;
            _subcontext = subcontext;
        }

        /*******************************************************************************
        /// Actions
        *******************************************************************************/
        // Takes the staging id from a previous upload operation, does not require the guid. 
        [Produces("application/json")]
        [Route("Test/ReadyTest")]
        [HttpGet]
        public IActionResult ReadyTest(int id)
        {
            var result = _context.Stagings.FirstOrDefault(m => m.Id == id);
            if (result == null)
            {
                return Json(new
                {
                    resonse = "not ready"
                });
            }
            var controller = new DocumentSubmissionController(_context, _subcontext);
            return controller.Ready(id, result.Guid);
        }

        [Route("Test/UploadImage")]
        public async Task<IActionResult> PostImage(List<IFormFile> files, AbstractFormObject.FormType formType)
        {
            var controller = new DocumentUploadController(_context);
            return await controller.PostImage(files, formType);
        }

        // Used primarily for testing and validation
        // Takes the id of a staged upload, copies it to a new stage, then submits it.
        // This method causes an exception to be thrown and caught due to the lack of edit
        // data.
        [Produces("application/json")]
        [Route("Test/SubmitTest")]
        [HttpGet]
        public IActionResult Submit(int id)
        {
            var oldstage = _context.Stagings.FirstOrDefault(m => m.Id == id);

            if (oldstage == null)
            {
                return Json(new 
                { 
                    response = "not ready" 
                });
            }

            // Copy staging
            var stage = new SubmissionStaging
            {
                Guid = Guid.NewGuid().ToString(),
                UriString = oldstage.UriString,
                ParsedTextractJSON = oldstage.ParsedTextractJSON,
                formType = oldstage.formType
            };
            _context.Add(stage);
            _context.SaveChanges();

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
                return Json(new 
                { 
                    response = "invalid form" 
                });
            }

            ts.id = stage.Id;
            ts.guid = stage.Guid;

            var controller = new DocumentSubmissionController(_context, _subcontext);
            var PWAForm = PWAsubmission.FromForm(ts, stage.formType);
            switch (stage.formType)
            {
                case AbstractFormObject.FormType.OR526_ATTENDANT:
                case AbstractFormObject.FormType.OR507_RELIEF:
                    return controller.SubmitTimesheet((PWATimesheet)PWAForm);
                case AbstractFormObject.FormType.OR004_MILEAGE:
                    return controller.SubmitMileage((PWAMileage)PWAForm);
                default:
                    return Json(new
                    {
                        response = "invalid"
                    });
            }
        }
    }
}
#endif