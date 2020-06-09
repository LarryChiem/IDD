using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Appserver.Controllers;
using Microsoft.AspNetCore.Http;

namespace TextToDoc
{
    class Images
    {
        public static void Run( string [] args)
        {
            var files = new List<string>(args);
            files.RemoveAt(0); // Remove the first argument

            foreach( var file in files)
            {
                if (!File.Exists(Path.Combine(Environment.CurrentDirectory, file)))
                {
                    Console.WriteLine(Path.Combine(Environment.CurrentDirectory, file));
                    Console.WriteLine("Could not open file {0}\n", file);
                    Console.WriteLine(Environment.CurrentDirectory);
                }
                else
                {
                    string contenttype = "";
                    switch (Path.GetExtension(file).ToLower())
                    {
                        case ".jpeg":
                        case ".jpg":
                            contenttype = "image/jpeg";
                            break;
                        case ".png":
                            contenttype = "image/png";
                            break;
                        case ".heic":
                            contenttype = "image/heic";
                            break;
                        default:
                            Console.WriteLine("Invalid Content Type: {0}", Path.GetExtension(file).ToLower());
                            break;
                    }
                    if(String.IsNullOrEmpty(contenttype))
                    {
                        break;
                    }
                    var imageFile = File.OpenRead(file);
                    using (var ms = new MemoryStream())
                    {
                        imageFile.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        var blur = DocumentUploadController.detect_blur(new FormFile(ms, 0, ms.Length, imageFile.Name, imageFile.Name)
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = contenttype
                        });
                        Console.WriteLine("{0} {1}", Path.GetFileName(imageFile.Name), blur);
                    }
                }
            }
        }
    }
}
