using NUnit.Framework;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Http;
using Appserver.Controllers;
using Appserver.Data;
using Common.Data;
using Microsoft.EntityFrameworkCore;

namespace ImageUpload.Tests
{
    [TestFixture]
    public class ImageUploadTest
    {
        CloudBlobContainer container;
        string fileName;
        string localFilePath;
        string storage_name;

        [SetUp]
        public async Task SetupAzureStorage()
        {
            var storageAccount = CloudStorageAccount.Parse(
                "UseDevelopmentStorage=true;"
            );

            var blobStorage = storageAccount.CreateCloudBlobClient();

            //Create a unique name for the container
            string containerName = "testing";

            // Create the container and return a container client object
            container = blobStorage.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });


            // Hard code an image file name here instead
            fileName = "Appserver/ImageUpload/pineapple.jpg";
            string localPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;

            localFilePath = Path.Combine(localPath, fileName);
        }

        [TearDown]
        public async Task RemoveFileFromAzure()
        {
            BlobContinuationToken continuationToken = null;
            var list = await container.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, 100, continuationToken, null, null);

            foreach (CloudBlob item in list.Results)
            {

                if( item.Name == storage_name)
                {
                    await item.DeleteIfExistsAsync();
                }
            }

        }

        [Test]
        public async Task FromLocalStorageTest()
        {
            //////////////////////////////////////////////////////////////////////////////
            // Initial Setup
            //////////////////////////////////////////////////////////////////////////////
            

            // Get a reference to a blob
            var blobClient = container.GetBlockBlobReference(fileName);

            ////////////////////////////////////////////////////////////////////////////////
            //// Function to Test
            ////////////////////////////////////////////////////////////////////////////////

            storage_name = fileName + Guid.NewGuid().ToString() + "_local";
            await IDD.ImageUpload.UploadImage(blobClient, fileName, localFilePath);

            ////////////////////////////////////////////////////////////////////////////////
            //// Assert it worked
            ////////////////////////////////////////////////////////////////////////////////

            //// Check item is in Blob
            var blobitem = container.GetBlockBlobReference(fileName);

            var itemexists = blobitem.ExistsAsync();
            await itemexists;
            Assert.IsTrue(itemexists.Result);
        }

        [Test]
        public async Task FromFileStreamTest()
        {
            //////////////////////////////////////////////////////////////////////////////
            // Initial Setup
            //////////////////////////////////////////////////////////////////////////////

            using FileStream uploadFileStream = File.OpenRead(localFilePath);
            using MemoryStream memoryStream = new MemoryStream();
            uploadFileStream.CopyTo(memoryStream);

            string image64 = Convert.ToBase64String(memoryStream.ToArray());

            // Get a reference to a blob
            var blobClient = container.GetBlockBlobReference(fileName);

            ////////////////////////////////////////////////////////////////////////////////
            //// Function to Test
            ////////////////////////////////////////////////////////////////////////////////
            storage_name = fileName + Guid.NewGuid().ToString() + "_stream";
            await IDD.ImageUpload.UploadImage64(blobClient, storage_name, image64);

            ////////////////////////////////////////////////////////////////////////////////
            //// Assert it worked
            ////////////////////////////////////////////////////////////////////////////////

            
            //// Check item is in Blob
            var blobitem = container.GetBlockBlobReference(storage_name);

            var itemexists = blobitem.ExistsAsync();
            await itemexists;
            Assert.IsTrue(itemexists.Result);
        }


    }

    [TestFixture]
    public class ImageToTextractTest
    {
        private readonly SubmissionStagingContext context;

        [Test]
        public async Task PassValidFile()
        {
            //////////////////////////////////////////////////////////////////////////////
            // Initial Setup
            //////////////////////////////////////////////////////////////////////////////

            // Get local image file
            string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + @"/ImageUpload/timesheet_front.jpg";

            // Validate path
            if (!File.Exists(filePath))
            {
                Assert.False(true);
            }

            ////////////////////////////////////////////////////////////////////////////////
            //// Function to Test
            ////////////////////////////////////////////////////////////////////////////////

            // Open file and pass to textract
            var li = File.ReadAllBytes(filePath);
            IFormFile file = new FormFile(new MemoryStream(li), 0, li.Length, "testFile", "testimage.jpg");
            ImageUploadController iuc = new ImageUploadController(context);
            var res = await iuc.pass_to_textract(file);
            Assert.IsNotEmpty(res.ToString());
        }


        [Test]
        public async Task PassInvalidFile()
        {
            //////////////////////////////////////////////////////////////////////////////
            // Initial Setup
            //////////////////////////////////////////////////////////////////////////////

            // Get local image file
            string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + @"/ImageUpload/timesheet_front.jpg";

            // Validate path
            if (!File.Exists(filePath))
            {
                Assert.False(true);
            }

            ////////////////////////////////////////////////////////////////////////////////
            //// Function to Test
            ////////////////////////////////////////////////////////////////////////////////

            // Open file and pass to textract
            var li = File.ReadAllBytes(filePath);
            IFormFile file = new FormFile(new MemoryStream(li), 0, 0, "testFile", "testimage.jpg");
            ImageUploadController iuc = new ImageUploadController(context);
            var res = await iuc.pass_to_textract(file);
            string x = "{response_miss: Something doesn't smell right...}";
            Assert.AreEqual(res.ToString(), x);
        }

    }
}
