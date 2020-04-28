using System;
using NUnit.Framework;
using Newtonsoft.Json;
using Amazon.Textract.Model;
using System.Collections.Generic;
using System.Text;
using System.IO;
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
            Assert.IsTrue(document.PageCount() == 4);
        }

        [TearDown]
        public void closeFile()
        {
            jsonFile.Close();
        }
    }
}
