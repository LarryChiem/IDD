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


        public int FormSubmissionTest()
        {

            TimesheetForm model = new TimesheetForm();
            model.prime = "A1234";
            model.providerName = "Donald P. Duck";
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
            model.addTimeRow("2020-04-02", "09:00", "10:00", "1.0", "true");
            model.addTimeRow("2020-04-03", "09:00", "10:00", "1.0", "true");
            model.addTimeRow("2020-04-04", "09:00", "10:00", "1.0", "true");

            var dbutil = new FormToDbUtil(_scontext, _context);

            Timesheet ts = dbutil.PopulateTimesheet(model);
            dbutil.PopulateTimesheetEntries(model, ts);

            return dbutil.TimesheetEFtoDB(ts);
        }

        // POST: /home/timesheet/
        [HttpPost("ImageUpload")]
        public async Task<IActionResult> PostImage(List<IFormFile> files)
        {
            var c = files.Count;
            var textract_responses = new List<AnalyzeDocumentResponse>();
            var skipped_files = new List<string>();
            var stats = new List<string>();

            // MIME types for image processing
            var image_types = new List<string>
            {
                "image/jpeg",
                "image/png"
            };


            // Iterate of collection of file and send to Textract
            foreach (var file in files)
            {
                // Only process image types Textract can handle
                if (image_types.Contains(file.ContentType) && file.Length > 0)
                {
                    //Time how long it takes Textract to process image
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    textract_responses.Add(process_image(file.OpenReadStream()));

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

            
            return Json(new
            {
                file_count = c,
                //azfc_resp = textract_responses,
                skipped = skipped_files,
                //textract_stats = stats,
                id = await saveSubmissisionStage(await UploadToBlob(files), textract_responses)
            }
            );
        }


        // Controller to accept images POSTed as bytes in the body
        [Route("ImageUpload/DocAsForm")]
        [HttpPost("ImageList")]
        public async Task<IActionResult> ImageList(IFormCollection file_collection)
        {
            return await PostImage(file_collection.Files.ToList());
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

        private async Task<int> saveSubmissisionStage(string uriString, List<AnalyzeDocumentResponse> responses)
        {
            // Create a SubmissionStaging to upload to SubmissionStaging table
            var ss = new SubmissionStaging
            {
                ParsedTextractJSON = System.Text.Json.JsonSerializer.Serialize(responses),
                UriString = uriString
            };

            // Add SubmissionStaging to table and get the Id to add to JSON response return
            _context.Add(ss);
            await _context.SaveChangesAsync();
            return ss.Id;
        }

        private AnalyzeDocumentResponse process_image(Stream file)
        {
            return new TextractHandler().HandleAsyncJob(file);
        }


        // Method to handle PDF uploads. Pages from PDF uploads
        // needs to be turned into bytes to send to Textract.
        // We could do this by page in the PDF, but how would we know
        // what type of page we're sending? Milage, hours, etc.?
        // Method argument is file sent with an HTTP Request (IFormFile)
        private void process_pdf_upload(IFormFile file)
        {
            Debug.WriteLine("Would have processed a PDF");
            return;
        }

        // Takes an IFormFile and sends it to AWS Textract for processing.
        public async Task<string> pass_to_textract(IFormFile file)
        {
            // Convert file to bytes
            MemoryStream ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();

            // Bytes to ByteArray
            var data = new ByteArrayContent(fileBytes);
            data.Headers.Add("Content-Type", "application/json");
            data.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            // Create HttpClient to call Azure Function
            HttpClient client = new HttpClient();
            string functionDomain = "https://clownedpineapple.azurewebsites.net";
            string functionURI = "/api/HttpTrigger2?code=01sWzhyR/lezKX8pqrLGcbyRG26qgyM0VGxPfyYm9x3WeJXKjOeDsg==";

            // Wait for Azure Function response
            var response = await client.PostAsync(functionDomain + functionURI, data);
            return response.Content.ReadAsStringAsync().Result.Replace("\"", "");
        }


    }
}
