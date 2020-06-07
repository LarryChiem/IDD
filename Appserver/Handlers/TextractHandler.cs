using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.Textract;
using Amazon.Textract.Model;
using Microsoft.AspNetCore.Http;
using Amazon.S3.Transfer;
using Amazon.S3.Model;

namespace Appserver.Controllers
{
    public class TextractHandler
    {

		/*******************************************************************************
        /// Fields
        *******************************************************************************/
		private AmazonTextractClient textractClient;
		private AmazonS3Client s3Client;


		/*******************************************************************************
        /// Constructor
        *******************************************************************************/
		public TextractHandler()
		{
			this.textractClient = new AmazonTextractClient();
			this.s3Client = new AmazonS3Client();
		}


		/*******************************************************************************
        /// Methods
        *******************************************************************************/


		// Handler for image files
		public AnalyzeDocumentResponse HandleAsyncJob(Stream file)
		{
			var job = StartDocumentAnalysis(file, new List<string> { "TABLES", "FORMS" });
			try
			{
				job.Wait();
			}
			catch (AggregateException e)
			{
				Console.WriteLine(e.Message);
				throw;
			}
			var result = job.Result;

			return result;
		}


        // Handler for PDFs
        public GetDocumentAnalysisResponse HandlePDFasync(IFormFile file)
        {
			var job = StartPDFAnalysis(file, new List<string> { "TABLES", "FORMS" });
			try
			{
				job.Wait();
			}
			catch (AggregateException e)
			{
				Console.WriteLine(e.Message);
				throw;
			}
			var result = job.Result;

			return result;
		}


		// Handler for image formatted documents
		private async Task<AnalyzeDocumentResponse> StartDocumentAnalysis(Stream file, List<string> featureTypes)
		{
			var request = new AnalyzeDocumentRequest();
			var memoryStream = new MemoryStream();
			file.CopyTo(memoryStream);

			request.Document = new Document
			{
				Bytes = memoryStream
			};

			request.FeatureTypes = featureTypes;
			var response = await this.textractClient.AnalyzeDocumentAsync(request);
			return response;
		}


        // Does the work of analyzing PDFs. Start by saving the file to S3, as this is currently the only
        // way for textract to analyze PDFs. Then, point the analysis request to the S3 bucket and begin
        // processing. Finally, wait for the results of processing.
		private async Task<GetDocumentAnalysisResponse> StartPDFAnalysis(IFormFile file, List<string> featureTypes)
        {
			// Upload PDF to S3, with guid as file key
			var guid = Guid.NewGuid();
			PDFtoS3Bucket(file, guid.ToString()).Wait();

			// Create S3 obj to hand to Textract
			var s3 = new Amazon.Textract.Model.S3Object();
			s3.Bucket = Environment.GetEnvironmentVariable("BUCKET_NAME");
			s3.Name = guid.ToString();
			var startrequest = new StartDocumentAnalysisRequest();

			// Set document for request to S3 obj
			startrequest.DocumentLocation = new DocumentLocation
			{
				S3Object = s3
			};
			startrequest.FeatureTypes = featureTypes;

            // Start analysis
			var response = await this.textractClient.StartDocumentAnalysisAsync(startrequest);

            // Wait for analysis to finish
			var getrequest = new GetDocumentAnalysisRequest();
			getrequest.JobId = response.JobId;

			var results = await GetDocAnalysisResponse(response);

			// Remove PDF from S3
			RemoveFromS3Bucket(guid.ToString()).Wait();
			return results;
		}


        // Gets the result of a PDF analysis request.
        private async Task<GetDocumentAnalysisResponse> GetDocAnalysisResponse(StartDocumentAnalysisResponse response)
        {
			// Get jobID from start analysis response
			var getrequest = new GetDocumentAnalysisRequest();
			getrequest.JobId = response.JobId;

			// Poll for analysis to finish. Can take over 15sec. to
            // get results, thus the somewhat long delay.
			GetDocumentAnalysisResponse res = await this.textractClient.GetDocumentAnalysisAsync(getrequest);
			int c = 0;
            while(res.JobStatus != "SUCCEEDED")
            {
				await Task.Delay(200);
				res = await this.textractClient.GetDocumentAnalysisAsync(getrequest);
				c++;
				System.Diagnostics.Debug.WriteLine("Trying again.... " + res.JobStatus + " Attempt " + c);
            }

			return res;
        }


		// Saves PDF to S3 bucket
		private async Task PDFtoS3Bucket(IFormFile file, string filekey)
        {
			var fileTransferUtil = new TransferUtility(s3Client);
			var req = new TransferUtilityUploadRequest();
			var mem = new MemoryStream();

            try
            {
                // Set Params for upload request
				file.CopyTo(mem);
				req.BucketName = Environment.GetEnvironmentVariable("BUCKET_NAME");
				req.InputStream = mem;
				req.Key = filekey;

				await fileTransferUtil.UploadAsync(req);
            }
            catch (Exception)
            {
				return;
            }
        }

        // Removes object from bucket with matching key
        private async Task RemoveFromS3Bucket(string key)
        {
			var deleteObj = new DeleteObjectRequest
			{
				BucketName = Environment.GetEnvironmentVariable("BUCKET_NAME"),
				Key = key
			};

            try
            {
				await s3Client.DeleteObjectAsync(deleteObj);
            }
            catch (Exception)
            {
				return;
            }
        }
	}
}
