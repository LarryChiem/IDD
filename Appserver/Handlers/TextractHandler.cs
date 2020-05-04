using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime.Internal;
using Amazon.Textract;
using Amazon.Textract.Model;
using Microsoft.Extensions.Configuration;


namespace Appserver.Controllers
{
    public class TextractHandler
    {
		private AmazonTextractClient textractClient;
		public TextractHandler()
		{
			this.textractClient = new AmazonTextractClient();
		}

		public AnalyzeDocumentResponse HandleAsyncJob(Stream file)
		{
			var job = StartDocumentAnalysis(file, new List<string> { "TABLES", "FORMS" });
			try
			{
				job.Wait();
			}catch(System.AggregateException e)
			{
				Console.WriteLine(e.Message);
				throw;
			}
			var result = job.Result;

			return result;
		}
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
	}
}

/*
namespace Dotnet_Core.Services
{
	public class TextractTextDetectionService
	{
		public void WaitForJobCompletion(string jobId, int delay = 5000)
		{
			while (!IsJobComplete(jobId))
			{
				this.Wait(delay);
			}
		}

		public bool IsJobComplete(string jobId)
		{
			var response = this.textract.GetDocumentTextDetectionAsync(new GetDocumentTextDetectionRequest
			{
				JobId = jobId
			});
			response.Wait();
			return !response.Result.JobStatus.Equals("IN_PROGRESS");
		}

		public List<GetDocumentTextDetectionResponse> GetJobResults(string jobId)
		{
			var result = new List<GetDocumentTextDetectionResponse>();
			var response = this.textract.GetDocumentTextDetectionAsync(new GetDocumentTextDetectionRequest
			{
				JobId = jobId
			});
			response.Wait();
			result.Add(response.Result);
			var nextToken = response.Result.NextToken;
			while (nextToken != null)
			{
				this.Wait();
				response = this.textract.GetDocumentTextDetectionAsync(new GetDocumentTextDetectionRequest
				{
					JobId = jobId,
					NextToken = response.Result.NextToken
				});
				response.Wait();
				result.Add(response.Result);
				nextToken = response.Result.NextToken;
			}
			return result;
		}

		private void Wait(int delay = 5000)
		{
			Task.Delay(delay).Wait();
			Console.Write(".");
		}

		public async Task<DetectDocumentTextResponse> DetectTextS3(string bucketName, string key)
		{
			var result = new DetectDocumentTextResponse();
			var s3Object = new S3Object
			{
				Bucket = bucketName,
				Name = key
			};
			var request = new DetectDocumentTextRequest();
			request.Document = new Document
			{
				S3Object = s3Object
			};
			return await this.textract.DetectDocumentTextAsync(request);
		}

		private void Print(List<Block> blocks)
		{
			blocks.ForEach(x => {
				if (x.BlockType.Equals("LINE"))
				{
					Console.WriteLine(x.Text);
				}
			});
		}

		public void Print(DetectDocumentTextResponse response)
		{
			if (response != null)
			{
				this.Print(response.Blocks);
			}
		}

		public void Print(List<GetDocumentTextDetectionResponse> response)
		{
			if (response != null && response.Count > 0)
			{
				response.ForEach(r => this.Print(r.Blocks));
			}
		}

		public List<string> GetLines(DetectDocumentTextResponse result)
		{
			var lines = new List<string>();
			result.Blocks.FindAll(block => block.BlockType == "LINE").ForEach(block => lines.Add(block.Text));
			return lines;
		}
	}
}

    */
