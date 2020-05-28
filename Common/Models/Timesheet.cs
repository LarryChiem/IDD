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
         *  Adds the TimeEntries to the specified XGraphics
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
        }

        /*
         *  Loads the TimeEntries from the database
         *  Parameters: DbContext
         *  Returns nothing
         */
        public override void LoadEntries(DbContext context)
        {
            context.Entry(this).Collection(t => t.TimeEntries).Load();
        }

        /*
         *  Changes the status of the specified TimeEntry
         *  Parameters: int entryId, string status
         *  Returns nothing
         */
        public override void ChangeEntryStatus(int entryId, string status)
        {
            TimeEntries.First(e => e.Id == entryId).Status = status;
        }
        
        /*
         *  Changes the status of all TimeEntries
         *  Parameters: string status
         *  Returns nothing
         */
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
