using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

namespace Common.Models
{
    public class MileageForm : Submission
    {
        public double TotalMiles { get; set; }
        public ICollection<MileageEntry> MileageEntries { get; set; }

        /*
         *  Adds the MileageEntries to the specified XGraphics
         *  Parameters: XGraphics
         *  Returns nothing
         */
        protected override void AddEntriesToPdf(XGraphics gfx)
        {
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
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("8cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Create the header of the table
            var row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightGreen;
            row.Cells[0].AddParagraph("Date");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].AddParagraph("Total Miles for Date");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[2].AddParagraph("Group? Yes/No");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[3].AddParagraph("Purpose of Trip/Service Goal");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;

            foreach (var entry in MileageEntries)
            {
                row = table.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Cells[0].AddParagraph(entry.Date.ToShortDateString());
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].AddParagraph(entry.Miles.ToString(CultureInfo.CurrentCulture));
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].AddParagraph(entry.Group ? "Yes" : "No");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].AddParagraph(entry.PurposeOfTrip);
                row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

            }

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Cells[0].AddParagraph("Total");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[1].AddParagraph(TotalMiles + " Miles");
            row.Cells[1].MergeRight = 2;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            table.SetEdge(0, 0, 4, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

            // Create a renderer and prepare (=layout) the document
            var docRenderer = new MigraDoc.Rendering.DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(13), "12cm", table);

            //END OF TABLE
        }
        /*
         *  Loads all MileageEntries from the database
         *  Parameters: DbContext
         *  Returns nothing
         */
        public override void LoadEntries(DbContext context)
        {
            context.Entry(this).Collection(m => m.MileageEntries).Load();
        }
        /*
         *  Changes the status of a single MileageEntry
         *  Parameters: int EntryId, string status
         *  Returns nothing
         */
        public override void ChangeEntryStatus(int entryId, string status)
        {
            MileageEntries.First(e => e.Id == entryId).Status = status;
        }
        /*
         *  Changes the status of all MileageEntries
         *  Parameters: string status
         *  Returns nothing
         */
        public override void ChangeAllEntriesStatus(string status)
        {
            foreach (var e in MileageEntries)
            {
                if (string.IsNullOrEmpty(e.Status) ||
                    e.Status.Equals("Pending", StringComparison.CurrentCultureIgnoreCase))
                    e.Status = status;
            }
        }
    }
}
