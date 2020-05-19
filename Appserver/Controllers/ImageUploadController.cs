using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Net.Http;
using Amazon.Textract;
using Amazon.Textract.Model;
using System;
using System.Configuration;
using System.Data;
using Appserver.Data;
using Appserver.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using Microsoft.Extensions.Configuration;
using IDD;
using Common.Models;
using Common.Data;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Extensions.Primitives;

namespace Appserver.Controllers
{
    public class ImageUploadController : Controller
    {
        private readonly SubmissionStagingContext _context;
        private readonly SubmissionContext _scontext;


        public ImageUploadController(SubmissionStagingContext context, SubmissionContext scontext)
        {
            _context = context;
            _scontext = scontext;
        }

        // POST: /home/timesheet/
        [HttpPost("ImageUpload")]
        public async Task<IActionResult> PostImage(List<IFormFile> files, AbstractFormObject.FormType formType)
        {
            var c = files.Count;
            var image_responses = new List<AnalyzeDocumentResponse>();
            var pdf_responses = new List<GetDocumentAnalysisResponse>();
            var skipped_files = new List<string>();
            var stats = new List<string>();

            // MIME types we can send to textract
            var accepted_types = new List<string>
            {
                "image/jpeg",
                "image/png",
                "application/pdf",
            };

            // Iterate of collection of file and send to Textract
            foreach (var file in files)
            {
                // Nothing to work with, next!
                if(file.Length == 0)
                {
                    skipped_files.Add("File name " + file.Name + " is empty");
                    continue;
                }
                // Only process files that have acceptable types
                if (accepted_types.Contains(file.ContentType))
                {
                    //Time how long it takes Textract to process image
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // Process PDF
                    if(file.ContentType == "application/pdf")
                    {
                        pdf_responses.Add(process_pdf(file));
                    }
                    // Process image
                    else
                    {
                        image_responses.Add(process_image(file));
                    }

                    stopwatch.Stop();
                    TimeSpan ts = stopwatch.Elapsed;
                    string s = String.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
                    stopwatch.Reset();
                    stats.Add(file.FileName + " " + s);
                }
                else
                {
                    skipped_files.Add(
                        "File name " + file.Name +
                        " has incompatible type " + file.ContentType
                        );
                }
            }

            int stageId;
            if (pdf_responses.Count > 0)
            {
                stageId = await saveSubmissionStage(await UploadToBlob(files), pdf_responses , formType);
            }
            else
            {
                stageId = await saveSubmissionStage(await UploadToBlob(files), image_responses, formType);
            }



            return Json(new
            {
                file_count = c,
                skipped = skipped_files,
                id = stageId
            }
            );
        }


        // Controller to accept images POSTed as bytes in the body
        [Route("ImageUpload/DocAsForm")]
        [HttpPost("ImageList")]
        public async Task<IActionResult> ImageList(IFormCollection file_collection)
        {
            if (file_collection["formChoice"].Equals(StringValues.Empty))
            {
                return Json(new
                {
                    response = "invalid form type"
                });
            }
            AbstractFormObject.FormType formType = (AbstractFormObject.FormType)Enum.Parse(typeof(AbstractFormObject.FormType),file_collection["formChoice"].ToString());

            return await PostImage(file_collection.Files.ToList(), formType);
        }

        private async Task<string> UploadToBlob(List<IFormFile> files)
        {
            // Get Blob Container
            var container = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("BLOB_CONNECTION"))
                .CreateCloudBlobClient()
                .GetContainerReference("submissionfiles");
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });


            // Upload images to container and save UriStrings
            var uriString ="";
            foreach (var f in files)
            {
                if (!string.IsNullOrEmpty(uriString))
                    uriString += ',';
                var blockBlob = container.GetBlockBlobReference(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fffffff") + "_" + f.FileName);
                uriString += blockBlob.Uri.AbsoluteUri;
                await blockBlob.UploadFromStreamAsync(f.OpenReadStream());
            }

            return uriString;
        }

        private async Task<int> saveSubmissionStage<T>(string uriString, List<T> responses, AbstractFormObject.FormType formType)
        {
            // Create a SubmissionStaging to upload to SubmissionStaging table
            var ss = new SubmissionStaging
            {
                ParsedTextractJSON = System.Text.Json.JsonSerializer.Serialize(responses),
                UriString = uriString,
                formType = formType
            };

            // Add SubmissionStaging to table and get the Id to add to JSON response return
            _context.Add(ss);
            await _context.SaveChangesAsync();
            return ss.Id;
        }

        private AnalyzeDocumentResponse process_image(IFormFile file)
        {
            return new TextractHandler().HandleAsyncJob(file.OpenReadStream());
        }


        // Method to handle PDF uploads. Pages from PDF uploads
        // needs to be turned into bytes to send to Textract.
        // We could do this by page in the PDF, but how would we know
        // what type of page we're sending? Milage, hours, etc.?
        // Method argument is file sent with an HTTP Request (IFormFile)
        private GetDocumentAnalysisResponse process_pdf(IFormFile file)
        {
            return new TextractHandler().HandlePDFasync(file);
        }

    }
}
