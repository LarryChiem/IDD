using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using Common.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

namespace Common.Models
{
    public class Timesheet : Submission
    {
        public double TotalHours { get; set; }
        public ICollection<TimeEntry> TimeEntries { get; set; }

        /*
         *  Creates a PDF representation of the Timesheet
         *  No Parameters
         *  Returns a PDF
         */
        public override PdfDocument ToPdf()
        {
            var watch = new Stopwatch();
            watch.Start();
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

            // TABLE
            var doc = new Document();
            var section = doc.AddSection();
            var table = section.AddTable();
            table.Style = "Table";
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;

            // Before you can add a row, you must define the columns
            var column = table.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("3.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Create the header of the table
            var row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightGreen;
            row.Cells[0].AddParagraph("Date");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].AddParagraph("Start/Time IN");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[2].AddParagraph("End/Time OUT");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[3].AddParagraph("Total hours for entry");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[4].AddParagraph("Group? (yes/no)");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;

            foreach (var entry in TimeEntries)
            {
                row = table.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Cells[0].AddParagraph(entry.Date.ToShortDateString());
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].AddParagraph(entry.In.ToShortTimeString());
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[2].AddParagraph(entry.Out.ToShortTimeString());
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[3].AddParagraph(entry.Hours.ToString(CultureInfo.CurrentCulture));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[4].AddParagraph(entry.Group ? "Yes" : "No");
                row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            }

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Cells[0].AddParagraph("Total");
            row.Cells[0].MergeRight = 2;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[3].AddParagraph(TotalHours + " Hours");
            row.Cells[3].MergeRight = 1;
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

            table.SetEdge(0, 0, 5, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

            // Create a renderer and prepare (=layout) the document
            var docRenderer = new MigraDoc.Rendering.DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(13), "12cm", table);

            //END OF TABLE

            //This code section will add new pages with images.
            foreach (var uri in UriList)
            {
                using var wc = new WebClient();
                using var objImage = XImage.FromStream(wc.OpenRead(uri));
                //do stuff with the image
                var newPage = document.AddPage();
                var gfx2 = XGraphics.FromPdfPage(newPage);
                gfx2.DrawImage(objImage,0,0, newPage.Width, newPage.Height);
            }

            watch.Stop();
            Console.WriteLine("Time to Generate PDF: " + watch.Elapsed.Seconds + "." + watch.Elapsed.Milliseconds + " seconds");
            return document;
        }

        public override void LoadEntries(DbContext context)
        {
            context.Entry(this).Collection(t => t.TimeEntries).Load();
        }

        public override void ChangeEntryStatus(int entryId, string status)
        {
            TimeEntries.First(e => e.Id == entryId).Status = status;
        }
        public override void ChangeAllEntriesStatus(string status)
        {
            foreach (var e in TimeEntries)
            {
                if (string.IsNullOrEmpty(e.Status) ||
                    e.Status.Equals("Pending", StringComparison.CurrentCultureIgnoreCase))
                    e.Status = status;
            }
        }
    }
}
