using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Appserver.FormSubmit
{
    public class MileageForm : AbstractFormObject
    {
        private List<MileageRowItem> miles = new List<MileageRowItem>();
        public MileageForm() { }

        [JsonProperty("mileagesheet")]
        [JsonConverter(typeof(MileageRowConverter))]
        internal List<MileageRowItem> Mileage { get => miles; set => miles = value; }
        public string totalMiles { get; set; } = "0";
        public void addMileRow(string date, string miles, string group, string purpose) =>
            this.Mileage.Add(new MileageRowItem(date, miles, group, purpose));

        protected override void AddTables(List<TextractDocument.Table> tables)
        {
            var table = tables[0].GetTable();
            // Remove first row
            table.RemoveAt(0);

            // Grab last row for total
            var lastrow = table.Last();
            // Now remove it
            table.RemoveAt(table.Count - 1);

            foreach (var row in table)
            {
                addMileRow(
                  ConvertDate(row[0].ToString().Trim()), // Date
                  row[1].ToString().Trim(), // Miles
                  ConvertInt(row[2].ToString()).ToString().Trim(), // Group
                  row[3].ToString().Trim() // Purpose
                );
            }

            if (lastrow.Count > 3)
            {
                totalMiles = lastrow[1].ToString().Trim();
            }
        }
    }
}
