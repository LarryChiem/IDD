using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Amazon.Textract.Model;
using System;
using Appserver.Data;
using Appserver.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Primitives;

namespace Appserver.Controllers
{
    public class DocumentUploadController : Controller
    {

        /*******************************************************************************
        /// Fields
        *******************************************************************************/
        private readonly SubmissionStagingContext _context;

        /*******************************************************************************
        /// Constructor
        *******************************************************************************/
        public DocumentUploadController(SubmissionStagingContext context)
        {
            _context = context;
        }


        /*******************************************************************************
        /// Methods
        *******************************************************************************/

        // POST: /home/timesheet/
        // Handle the initial processing of document uploads. Currently, the only accepted
        // formats are jpg/png/pdf. Valid documents are sent to AWS Textract for processing
        // with the results being stored in a staging table. The documents themselves are
        // stored in Azure Blob upon being uploaded. 
        [NonAction]
        public async Task<IActionResult> PostImage(List<IFormFile> files, AbstractFormObject.FormType formType)
        {
            var c = files.Count;
            var image_responses = new List<AnalyzeDocumentResponse>();
            var pdf_responses = new List<GetDocumentAnalysisResponse>();
            var skipped_files = new List<string>();

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
            string stageGuid = Guid.NewGuid().ToString();
            if (pdf_responses.Count > 0)
            {
                stageId = await saveSubmissionStage(await UploadToBlob(files), pdf_responses, formType, stageGuid);
            }
            else
            {
                stageId = await saveSubmissionStage(await UploadToBlob(files), image_responses, formType, stageGuid);
            }


            return Json(new
            {
                file_count = c,
                skipped = skipped_files,
                id = stageId,
                guid = stageGuid
            }
            ) ;
        }


        // Document upload controller that is compatible with the format
        // being sent from the PWA.
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

        // Store user submissions in Azure and get back the file's bucket handle.
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

        // Place yet-to-be-submitted documents, in the form of a reference to a bucket file
        // and the response from textract, into a staging table.
        private async Task<int> saveSubmissionStage<T>(string uriString, List<T> responses, AbstractFormObject.FormType formType, string guid)
        {
            // Create a SubmissionStaging to upload to SubmissionStaging table
            var ss = new SubmissionStaging
            {
                Guid = guid,
                ParsedTextractJSON = System.Text.Json.JsonSerializer.Serialize(responses),
                UriString = uriString,
                formType = formType
            };

            // Add SubmissionStaging to table and get the Id to add to JSON response return
            _context.Add(ss);
            await _context.SaveChangesAsync();
            return ss.Id;
        }

        // Method to handle image format uploads
        private AnalyzeDocumentResponse process_image(IFormFile file)
        {
            return new TextractHandler().HandleAsyncJob(file.OpenReadStream());
        }

        // Method to handle PDF uploads.
        private GetDocumentAnalysisResponse process_pdf(IFormFile file)
        {
            return new TextractHandler().HandlePDFasync(file);
        }

    }
}
