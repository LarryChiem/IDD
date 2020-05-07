using System;
using NUnit.Framework;
using Newtonsoft.Json;
using Amazon.Textract.Model;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Appserver.TextractDocument;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace AppserverTest.TextractDocument
{
    [TestFixture]
    class TextractDocumentTest {

        private StreamReader jsonFile;
        [SetUp]
        public void loadFile()
        {
            jsonFile = File.OpenText(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "TextractDocument/textract.json"));
        }
        [Test]
        public void TextractFromJsonTest()
        {
            var document = new Appserver.TextractDocument.TextractDocument();
            
            using (StreamReader reader = jsonFile)
            {
                document.FromJson((JObject)JToken.ReadFrom(new JsonTextReader(reader)));
            }
            document.printSummary();
            Assert.IsTrue(document.PageCount() == 1);
        }

        [Test]
        public void KeyValuePairSort()
        {
            var document = new Appserver.TextractDocument.TextractDocument();
            document.FromJson((JObject)JToken.ReadFrom(new JsonTextReader(jsonFile)));
            Assert.IsTrue(IsSorted(document.GetPage(0).GetFormItems()));
        }

        private static bool IsSorted(List<KeyValuePair<KeyValueSet, KeyValueSet>> kvps)
        {
            // Function works by comparing the location of each block to the previous block
            const double threshold = 0.01;

            if (kvps.Count > 0)
                Console.WriteLine("Top:\t" + kvps[0].Key.GetGeometry().box.Top + "\tLeft:\t" + kvps[0].Key.GetGeometry().box.Left);

            for (var i = 1; i < kvps.Count; i++)
            {
                Console.WriteLine("Top:\t" + kvps[i].Key.GetGeometry().box.Top + "\tLeft:\t" + kvps[i].Key.GetGeometry().box.Left);

                // If both blocks are on the same line
                if (Math.Abs(kvps[i].Key.GetGeometry().box.Top - kvps[i - 1].Key.GetGeometry().box.Top) < threshold)
                {
                    // Return false if the 2nd block is to the left of the first block
                    if (kvps[i].Key.GetGeometry().box.Left < kvps[i - 1].Key.GetGeometry().box.Left)
                        return false;
                }

                // Or return false if the second block is above the first block
                else if (kvps[i].Key.GetGeometry().box.Top < kvps[i - 1].Key.GetGeometry().box.Top)
                    return false;
            }

            // Return true if everything checks out
            return true;
        }

        [TearDown]
        public void closeFile()
        {
            jsonFile.Close();
        }
    }
}
