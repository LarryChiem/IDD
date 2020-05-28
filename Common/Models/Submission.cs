using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using PdfSharp.Pdf;
using Microsoft.AspNetCore.Mvc;
using Common.Data;
using Microsoft.EntityFrameworkCore;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;

namespace Common.Models
{
    public abstract class Submission
    {
        public int Id { get; set; }
        public DateTime Submitted { get; set; }
        public string FormType { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public string ClientName { get; set; }
        public string ClientPrime { get; set; }
        public string ServiceGoal { get; set; }
        public string ProgressNotes { get; set; }
        public string Status { get; set; } = "Pending";
        public string UserActivity { get; set; }
        public string RejectionReason { get; set; }
        public bool Edited { get; set; }
        public Lock LockInfo { get; set; }
        [NotMapped]
        public IList<string> UriList { get; set; }
        public string UriString
        {
            get => string.Join(",", UriList); 
            set => UriList = value.Split(',').ToList();
        }



        /*
         *  Creates a PDF representation of the Timesheet
         *  No Parameters
         *  Returns a PDF
         */
        public PdfDocument ToPdf()
        {
            //http://www.pdfsharp.net/wiki/Unicode-sample.ashx
            // Create new document
            var document = new PdfDocument();

            var pdfString = "eXPRS Plan of Care - Services Delivered Report Form\n\n" +
                               "Timesheet ID: " + Id + "\n" +
                               "Status of Timesheet: " + Status + "\n" +
                               "Customer Name: " + ClientName + "\n" +
                               "Prime: " + ClientPrime + "\n" +
                               "Provider Name: " + ProviderName + "\n" +
                               "Provider Num: " + ProviderId + "\n" +
                               "CM Organization: Multnomah Case Management\n" +
                               "Form Type: " + FormType + "\n\n" +
                               "Service Goal: " + ServiceGoal + "\n\n" +
                               "Progress Notes: " + ProgressNotes + "\n\n" +
                               "Submitted on: " + Submitted + "\n";

            // Set font encoding to unicode
            var options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

            var font = new XFont("Times New Roman", 12, XFontStyle.Regular, options);


            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var tf = new XTextFormatter(gfx)
            {
                Alignment = XParagraphAlignment.Left
            };

            tf.DrawString(pdfString, font, XBrushes.Black,
                new XRect(100, 100, page.Width - 200, 600), XStringFormats.TopLeft);

            //Timesheets and Mileage forms will have different tables, so this part is delegated to the child classes
            AddEntriesToPdf(gfx);

            //This code section will add new pages with images.
            foreach (var uri in UriList)
            {
                using var wc = new WebClient();
                using var objImage = XImage.FromStream(wc.OpenRead(uri));
                var newPage = document.AddPage();
                var gfx2 = XGraphics.FromPdfPage(newPage);
                gfx2.DrawImage(objImage, 0, 0, newPage.Width, newPage.Height);
            }
            return document;
        }
        protected abstract void AddEntriesToPdf(XGraphics gfx);
        public abstract void LoadEntries(DbContext context);
        public abstract void ChangeEntryStatus(int entryId, string status);
        public abstract void ChangeAllEntriesStatus(string status);

    }
}