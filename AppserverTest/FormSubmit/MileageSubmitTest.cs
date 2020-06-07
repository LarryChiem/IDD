using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Appserver.FormSubmit;

namespace AppserverTest.FormSubmit
{
    [TestFixture]
    class MileageSubmitTest
    {
        [Test]
        public void EmptyMileagesheetTest()
        {

            string path = TestContext.CurrentContext.TestDirectory + @"\FormSubmit\emptyMileagesheet.json";

            if (!File.Exists(path))
            {
                Console.WriteLine(path);
                Assert.IsTrue(false);
            }

            string k = File.ReadAllText(path).Replace("\r", "");
            var obj = new MileageForm();

            //// Due to Windows adding \r for newlines we remove these, and for whatever reason Windows
            //// Also adds a newline at the end of the file even if it doesn't exist, so we add that.
            string j = JsonConvert.SerializeObject(obj, Formatting.Indented).Replace("\r", "") + "\n";

            Assert.IsTrue(String.Equals(j, k));
        }

        [Test]
        public void TenRowTimesheet()
        {
            string path = TestContext.CurrentContext.TestDirectory + @"\FormSubmit\TenRowMileagesheet.json";

            if (!File.Exists(path))
            {
                System.Console.WriteLine(path);
                Assert.IsTrue(false);
            }

            string k = File.ReadAllText(path).Replace("\r", "");
            MileageForm obj = new MileageForm();

            obj.id = 1;
            obj.clientName = "Donald Duck";
            obj.prime = "123456";
            obj.providerName = "Daughy Duck";
            obj.providerNum = "654321";
            obj.brokerage = "Not sure";
            obj.scpaName = "SC/PA";
            obj.serviceAuthorized = "All";

            obj.addMileRow("2020-03-20", "10", "true", "driving");
            obj.addMileRow("2020-03-21", "10", "true", "driving");
            obj.addMileRow("2020-03-22", "10", "true", "driving");
            obj.addMileRow("2020-03-23", "10", "true", "driving");
            obj.addMileRow("2020-03-24", "10", "true", "driving");
            obj.addMileRow("2020-03-25", "10", "true", "driving");
            obj.addMileRow("2020-03-26", "10", "true", "driving");
            obj.addMileRow("2020-03-27", "10", "true", "driving");
            obj.addMileRow("2020-03-28", "10", "true", "driving");
            obj.addMileRow("2020-03-29", "10", "true", "driving");

            obj.totalMiles = "100";
            obj.serviceGoal = "Drive them";
            obj.progressNotes = "Well behaved";
            obj.employerSignature = true;
            obj.employerSignDate = "2020-04-01";
            obj.authorization = true;
            obj.approval = "";
            obj.providerSignature = true;
            obj.providerSignDate = "2020-04-01";

            // Due to Windows adding \r for newlines we remove these, and for whatever reason Windows
            // Also adds a newline at the end of the file even if it doesn't exist, so we add that.
            string j = JsonConvert.SerializeObject(obj, Formatting.Indented).Replace("\r", "") + "\n";

            Assert.IsTrue(String.Equals(j, k));
        }
    }
}
