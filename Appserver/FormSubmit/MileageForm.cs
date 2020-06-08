using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;
using Appserver.TextractDocument;
using System.Text.RegularExpressions;

namespace Appserver.FormSubmit
{
    public class MileageForm : AbstractFormObject
    {
        /*******************************************************************************
        /// Constructor
        *******************************************************************************/
        public MileageForm() { }


        /*******************************************************************************
        /// Properties
        *******************************************************************************/
        [JsonProperty("mileagesheet")]
        [JsonConverter(typeof(MileageRowConverter))]
        internal List<MileageRowItem> Mileage { get => miles; set => miles = value; }
        public string totalMiles { get; set; } = "0";
        private List<MileageRowItem> miles = new List<MileageRowItem>();

        /*******************************************************************************
        /// Methods
        *******************************************************************************/
        /// <summary>
        /// Adds a row to the mileage sheet.
        /// </summary>
        /// <param name="date">The date the worked was performed on.</param>
        /// <param name="miles">The number of miles driven.</param>
        /// <param name="group">Whether this was a group or not.</param>
        /// <param name="purpose">The purpose of the trip.</param>
        public void addMileRow(string date, string miles, string group, string purpose) =>
            this.Mileage.Add(new MileageRowItem(date, miles, group, purpose));

        /// <summary>
        /// This takes a Table from a textract document and adds the information to the mileage
        /// form.
        /// </summary>
        /// <param name="tables"></param>
        protected override void AddTables(List<Table> tables)
        {
            var table = tables[0].GetTable();
            // Remove first row
            table.RemoveAt(0);

            // Grab last row for total
            var lastrow = table.Last();

            // If lastrow is a total mileage row, remove it
            if (isTotalMilesRow(lastrow))
            {
                table.RemoveAt(table.Count - 1);
            }

            // Only remove last row if it's a total Miles
            if (lastrow.Count < 3)
            {
                table.RemoveAt(table.Count - 1);
            }

            foreach (var row in table)
            {
                if(!isEmptyMileageRow(row))
                {
                    addMileRow(
                      ConvertDate(row[0].ToString().Trim()), // Date
                      row[1].ToString().Trim(), // Miles
                      IsGroup(row[2].ToString()).ToString().Trim(), // Group
                      row[3].ToString().Trim() // Purpose
                    );
                }
            }

            if (lastrow.Count > 3)
            {
                totalMiles = lastrow[1].ToString().Trim();
            }
            else
            {
                totalMiles = "0";
            }
        }

        /// <summary>
        /// Checks if the row is the total row or a normal row of the mileage sheet.
        /// </summary>
        /// <param name="lastrow">Takes a candidiate for the last row</param>
        /// <returns>True if this is the total row.</returns>
        private bool isTotalMilesRow(List<Cell> lastrow)
        {
            Regex rxNumberGroup = new Regex(@"([0-9])+");
            int confidence = 0;
            bool foundDate = false;

            foreach (var entry in lastrow)
            {
                var s = entry.ToString().Trim().ToLower();

                // If we can parse the entry into a valid
                // DateTime format, probably areen't dealing
                // with a total miles row.
                try
                {
                    var date = DateTime.Parse(s).ToString("yyyy-MM-dd");
                    foundDate = true;
                }
                catch (FormatException) { }

                // Increase confidence we have a total miles row
                if (s.Contains("total"))
                {
                    confidence++;
                }

                if (s.Contains("miles"))
                {
                    confidence++;
                }

                var matches = rxNumberGroup.Matches(s);
                confidence += matches.Count;
            }

            // Most likely a mileage entry
            if (foundDate)
            {
                return false;
            }

            // Did we at least find a contains words
            // and at least one group of numbers that could
            // be a mileage total?
            return confidence >= 2;
        }
    }
}
