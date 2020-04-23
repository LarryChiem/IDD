using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Net.Http;

namespace Appserver.Controllers
{
    public class ImageUploadController : Controller
    {
        // POST: /home/timesheet/
        [HttpPost("ImageUpload")]
        public async Task<IActionResult> PostImage(List<IFormFile> files)
        {
            Response.Headers.Add("Allow", "POST");

            // MIME types for image processing
            var image_types = new List<string>();
            image_types.Add("image/jpeg");
            image_types.Add("image/png");

            List<String> textract_responses = new List<string>();
            List<String> skipped_files = new List<string>();
            List<String> stats = new List<string>();

            // Iterate over list of submitted documents
            foreach (var file in files)
            {
                // Is the file non-empty?
                if(file.Length > 0)
                {
                    // Process image file upload
                    if (image_types.Contains(file.ContentType))
                    {
                        //Time how long it takes to upload image
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        var res = await pass_to_textract(file);
                        textract_responses.Add(res);

                        stopwatch.Stop();
                        TimeSpan ts = stopwatch.Elapsed;
                        string s = String.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
                        stopwatch.Reset();
                        stats.Add(file.FileName + " :: " + s);
                    }
                    // Process PDF upload
                    else if ("application/pdf".Equals(file.ContentType))
                    {
                        //Time how long it takes to upload pdf
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        process_pdf_upload(file);

                        stopwatch.Stop();
                        TimeSpan ts = stopwatch.Elapsed;
                        string s = String.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
                        stopwatch.Reset();
                        stats.Add(file.FileName + " :: " + s);
                    }
                    // Skip unhandled MIME types
                    else
                    {
                        skipped_files.Add(
                            "File name " + file.Name +
                            " has incompatible type " + file.ContentType
                            );
                    }
                }
            }

            return Json(new {
                file_count = files.Count,
                textract_stats = stats,
                azfc_resp = textract_responses,
                skipped = skipped_files}
            );
        }


        // Controller to accept images POSTed as bytes in the body
        [Route("ImageUpload/DocAsForm")]
        [HttpPost("ImageList")]
        public async Task<IActionResult> ImageList(IFormCollection file_collection)
        {
            var c = file_collection.Files.Count;
            List<String> textract_responses = new List<string>();
            List<String> skipped_files = new List<string>();
            List<String> stats = new List<string>();

            // MIME types for image processing
            var image_types = new List<string>();
            image_types.Add("image/jpeg");
            image_types.Add("image/png");

            // Iterate of collection of file and send to Textract
            foreach (var file in file_collection.Files)
            {
                // Only process image types Textract can handle
                if (image_types.Contains(file.ContentType) && file.Length > 0)
                {
                    //Time how long it takes Textract to process image
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    var res = await pass_to_textract(file);
                    textract_responses.Add(res);

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

            return Json(new {
                file_count = c,
                azfc_resp = textract_responses,
                skipped = skipped_files,
                textract_stats = stats}
            );
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
        public async Task<String> pass_to_textract(IFormFile file)
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
