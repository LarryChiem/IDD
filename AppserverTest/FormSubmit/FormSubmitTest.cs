using NUnit.Framework;
using FormSubmit;
using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Collections.Generic;
using Common.Models;
using IDD;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using Appserver.Data;

namespace FormSubmit.Tests
{
    [TestFixture]
    public class FormSubmitTest
    {
        [Test]
        public void EmptyTimesheet()
        {
            
            string path = TestContext.CurrentContext.TestDirectory + @"\FormSubmit\emptyTimesheet.json";

            if (!File.Exists(path))
            {
                Console.WriteLine(path);
                Assert.IsTrue(false);
            }

            string k = File.ReadAllText(path).Replace("\r", "");
            TimesheetForm obj;
            obj = new TimesheetForm();

            // Due to Windows adding \r for newlines we remove these, and for whatever reason Windows
            // Also adds a newline at the end of the file even if it doesn't exist, so we add that.
            string j = JsonConvert.SerializeObject(obj, Formatting.Indented).Replace("\r","") + "\n";

            Assert.IsTrue(String.Equals(j, k));
        }

        [Test]
        public void TenRowTimesheet()
        {
            string path = TestContext.CurrentContext.TestDirectory + @"\FormSubmit\TenRowTimesheet.json";

            if (!File.Exists(path))
            {
                System.Console.WriteLine(path);
                Assert.IsTrue(false);
            }

            string k = File.ReadAllText(path).Replace("\r", "");
            TimesheetForm obj = new TimesheetForm();

            obj.clientName   = "Donald Duck";
            obj.prime        = "123456";
            obj.providerName = "Daughy Duck";
            obj.providerNum  = "654321";
            obj.brokerage    = "Not sure";
            obj.scpaName     = "SC/PA";
            obj.serviceAuthorized = "All";
            obj.units = 20;
            obj.type = "Feeding";
            obj.frequency = "Daily";

            obj.addTimeRow("2020-03-20", "9:00", "10:00", "1", "true");
            obj.addTimeRow("2020-03-21", "9:00", "10:00", "1", "true");
            obj.addTimeRow("2020-03-22", "9:00", "10:00", "1", "true");
            obj.addTimeRow("2020-03-23", "9:00", "10:00", "1", "true");
            obj.addTimeRow("2020-03-24", "9:00", "10:00", "1", "true");
            obj.addTimeRow("2020-03-25", "9:00", "10:00", "1", "true");
            obj.addTimeRow("2020-03-26", "9:00", "10:00", "1", "true");
            obj.addTimeRow("2020-03-27", "9:00", "10:00", "1", "true");
            obj.addTimeRow("2020-03-28", "9:00", "10:00", "1", "true");
            obj.addTimeRow("2020-03-29", "9:00", "10:00", "1", "true");

            obj.totalHours = "10";
            obj.serviceGoal = "Feed them";
            obj.progressNotes = "Eating more fish";
            obj.employerSignature = true;
            obj.employerSignDate = "2020-04-01";
            obj.authorization = true;
            obj.approval = true;
            obj.providerSignature = true;
            obj.providerSignDate = "2020-04-01";

            // Due to Windows adding \r for newlines we remove these, and for whatever reason Windows
            // Also adds a newline at the end of the file even if it doesn't exist, so we add that.
            string j = JsonConvert.SerializeObject(obj, Formatting.Indented).Replace("\r", "") + "\n";

            Assert.IsTrue(String.Equals(j, k));
        }

        [Test]
        public void DefaultTimesheetStatus()
        {
            TimesheetForm form;
            form = new TimesheetForm();

            Assert.IsTrue(String.Equals(form.review_status, "Pending"));
        }

        [Test]
        public void TextractToTimesheetTest()
        {
            // Setup Textract Document
            var jsonFile = File.OpenText( "TextractDocument/textract.json" );
            var document = new Appserver.TextractDocument.TextractDocument();

            using (StreamReader reader = jsonFile)
            {
                document.FromJson((JObject)JToken.ReadFrom(new JsonTextReader(reader)));
            }

            var obj = AbstractFormObject.FromTextract(document);
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented).Replace("\r", "") + "\n");
        }

        [Test]
        public void TextractDateParsingTest()
        {
            var dlist = new List<string>();
            dlist.Add("20-1-12");
            dlist.Add("2020-1-12");
            dlist.Add("2020-01-12");
            dlist.Add("1-12-20");
            dlist.Add("1-12-2020");
            dlist.Add("01-12-20");
            dlist.Add("01-12-2020");
            dlist.Add("January, 1st, 2020");
            dlist.Add("Jan, 1st, 2020");
            dlist.Add("January, 12th, 2020");
            dlist.Add("Jan, 12th, 2020");
            dlist.Add("April, 13th, 2020");
            dlist.Add("Feb, 3rd, 2020");


            // Incorrect spelling causes parse failure
            dlist.Add("Janaury, 12th, 2020");

            // Semicolon delimeters causes failure
            //dlist.Add("20:1:12"); 

            // Convert or raise exception
            foreach (var x in dlist)
            {
                DateTime dt;
                try
                {
                    dt = FormConversionUtils.DateStringConvertUtil(x);
                    System.Console.WriteLine("Date: " + dt.ToString());
                    Assert.IsInstanceOf(typeof(DateTime), dt);
                }
                catch (FormatException)
                {
                    System.Console.WriteLine("Couldn't Parse Date From: " + x);
                }

            }
        }
    }
}
