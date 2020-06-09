using Appserver.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace TextToDoc
{
    class TextractProcessing
    {
        public static void Run( string []args)
        {

            // Create empty document
            var document = new Appserver.TextractDocument.TextractDocument();
            switch (args[0])
            {
                case "image":
                    Console.WriteLine("##\n##\n## Form Processing Image to Textract: ");
                    // Get json response
                    if (!File.Exists(Path.Combine(Environment.CurrentDirectory, args[1])))
                    {
                        Console.WriteLine(Path.Combine(Environment.CurrentDirectory, args[1]));
                        Console.WriteLine("Could not open file {0}\n", args[1]);
                        Console.WriteLine(Environment.CurrentDirectory);
                        return;
                    }
                    var imageFile = File.OpenRead(args[1]);
                    Console.WriteLine("Sending to Textract\n");
                    var handler = new TextractHandler();
                    var response = handler.HandleAsyncJob(imageFile);

                    // Write to file
                    File.WriteAllText("test.json", JsonConvert.SerializeObject(response, Formatting.Indented));


                    document.FromTextractResponse(response);
                    Console.WriteLine("Received Response from Textract\n");
                    break;
                case "json":
                    Console.WriteLine("##\n##\n## Form Processing JSON to Textract: ");
                    var jsonFile = File.OpenText(args[1]);

                    using (StreamReader reader = jsonFile)
                    {
                        document.FromJson((JObject)JToken.ReadFrom(new JsonTextReader(reader)));
                    }
                    break;
            }

            var p = document.GetPage(0);

            foreach (var l in p.GetFormItems())
            {
                Console.WriteLine(String.Format("Key: {0}\nValue: {1}", l.Key.ToString(), l.Value.ToString()));
            }

            Console.WriteLine("##\n##\n## Beginning Conversion to Timesheet Document:\n##\n");

            var ts = new TimesheetForm();
        }
    }
}
