using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AdminUI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Common.Data;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using System.IO;
using Common.Models;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace AdminUI.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly ILogger<SubmissionController> _logger;
        private readonly SubmissionContext _scontext;


        public SubmissionController(ILogger<SubmissionController> logger, SubmissionContext scontext)
        {
            _logger = logger;
            _scontext = scontext;
        }

        public async Task<IActionResult> GenPDF(int id)
        {
            //find the timesheet code
            var ts = await _scontext.Timesheets
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ts == null)
                return NotFound();

            //Load the timesheet's entries
            _scontext.Entry(ts).Collection(t => t.TimeEntries).Load();

            //http://www.pdfsharp.net/wiki/Unicode-sample.ashx
            string pdfString = "eXPRS Plan of Care - Services Delivered Report Form\n\n" +
                               "Timesheet ID: " + ts.Id + "\n" +
                               "Status of Timesheet: " + ts.Status + "\n" +
                               "Customer Name: " + ts.ClientName + "\n" +
                               "Prime: " + ts.ClientPrime + "\n" +
                               "Provider Name: " + ts.ProviderName + "\n" +
                               "Provider Num: " + ts.ProviderId + "\n" +
                               "CM Organization: Multnomah Case Management\n" +
                               "Form Type: " + ts.FormType + "\n\n" +
                               "Service Goal: " + ts.ServiceGoal + "\n\n" +
                               "Progress Notes: " + ts.ProgressNotes + "\n\n" +
                               "Submitted on: " + ts.Submitted + "\n";


            // Create new document
            PdfDocument document = new PdfDocument();

            // Set font encoding to unicode
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

            XFont font = new XFont("Times New Roman", 12, XFontStyle.Regular, options);


            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            tf.Alignment = XParagraphAlignment.Left;

            tf.DrawString(pdfString, font, XBrushes.Black,
                new XRect(100, 100, page.Width - 200, 600), XStringFormats.TopLeft);

            // TABLE
            Document doc = new Document();
            Section section = doc.AddSection();
            Table table = section.AddTable();
            table.Style = "Table";
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;

            // Before you can add a row, you must define the columns
            Column column = table.AddColumn("2.5cm");
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
            Row row = table.AddRow();
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

            double totalHours = 0;
            foreach (var entry in ts.TimeEntries)
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
                totalHours += entry.Hours;
                row.Cells[3].AddParagraph(entry.Hours.ToString());
                row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
                string group;
                if (entry.Group)
                {
                    group = "Yes";
                }
                else
                {
                    group = "No";
                }

                row.Cells[4].AddParagraph(group);
                row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            }

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Cells[0].AddParagraph("Total");
            row.Cells[0].MergeRight = 2;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[3].AddParagraph(totalHours.ToString() + " Hours");
            row.Cells[3].MergeRight = 1;
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

            table.SetEdge(0, 0, 5, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new MigraDoc.Rendering.DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(13), "12cm", table);



            //END OF TABLE



            //This code section will add new page with image.
            PdfPage page2 = document.AddPage();
            //TODO: This will eventually get the image from the database, whether that's S3 or something else.
            XImage front = XImage.FromFile("wwwroot/images/timesheet-front.PNG");
            XGraphics gfx2 = XGraphics.FromPdfPage(page2);
            gfx2.DrawImage(front, 10, page2.Height / 8, front.Width * .5, front.Height * .5);

            PdfPage page3 = document.AddPage();
            XImage back = XImage.FromFile("wwwroot/images/timesheet-back.PNG");
            XGraphics gfx3 = XGraphics.FromPdfPage(page3);
            gfx3.DrawImage(back, 10, page3.Height / 8, back.Width * .5, back.Height * .5);

            MemoryStream stream = new MemoryStream();
            // Save the document
            document.Save(stream, true);
            string fileDownloadName = ts.ClientName + "_" + ts.ClientPrime + "_" + ts.ProviderId + "_" +
                                      ts.ProviderName + "_" + ts.Submitted + "_" + ts.FormType + ".pdf";
            return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, fileDownloadName);
        }

        [HttpPost]
        public async Task<IActionResult> Process(int id, string Status, string RejectionReason)
        {
            var submission = _scontext.Submissions.Find(id);

            if (submission == null)
                return NotFound();
            
            _scontext.Entry(submission).Reference(t => t.LockInfo).Load();
            
            if (submission.LockInfo == null || !submission.LockInfo.User.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase))
                return View("NoPermission");
            
            submission.Status = Status;
            submission.RejectionReason = RejectionReason;
            submission.UserActivity = Status + " by " + submission.LockInfo.User + " on " + DateTime.Now;
            submission.LockInfo = null;
            
            if (ModelState.IsValid)
            {
                try
                {
                    _scontext.Update(submission);
                    await _scontext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!_scontext.Submissions.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("Index","Home");
        }
    }
}
