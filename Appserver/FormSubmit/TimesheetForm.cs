using Newtonsoft.Json;
using System.Collections.Generic;
using Appserver.TextractDocument;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class TimesheetForm: AbstractFormObject
{
    
    /*******************************************************************************
    /// Constructor
    *******************************************************************************/
    public TimesheetForm(){}


    /*******************************************************************************
    /// Fields
    *******************************************************************************/
    public int units { get; set; }
    public string type { get; set; } = "";
    public string frequency { get; set; } = "";
    private List<TimesheetRowItem> times = new List<TimesheetRowItem>();
    
    [JsonProperty("timesheet")]
    [JsonConverter(typeof(TimesheetRowConverter))]
    internal List<TimesheetRowItem> Times { get => times; set => times = value; }
    public string totalHours { get; set; } = "";


    /*******************************************************************************
    /// Methods
    *******************************************************************************/

    public void addTimeRow(string date, string start, string end, string total, string group) => 
        this.Times.Add(new TimesheetRowItem(date, start, end, total, group));

    protected override void AddTables(List<Table> tables)
    {
        var table = tables[0].GetTable();
        // Remove first row
        table.RemoveAt(0);

        // Grab last row for total
        var lastrow = table.Last();

        // Check if lastrow is a total hours row.
        // If so, remove it.
        if (isTotalTimeRow(lastrow))
        {
            table.RemoveAt(table.Count - 1);
        }

        if (lastrow.Count < 3)
        {
            table.RemoveAt(table.Count - 1);
        }

        foreach (var row in table)
        {
            // Check for empty rows
            if (!isEmptyTimesheetRow(row))
            {
                addTimeRow(
                  ConvertDate(row[0].ToString().Trim()),
                  FixHours(row[1].ToString()).ToString().Trim(),
                  FixHours(row[2].ToString()).ToString().Trim(),
                  FixHours(row[3].ToString()).ToString().Trim(),
                  ConvertInt(row[4].ToString()).ToString().Trim());
            }
        }

        if (lastrow.Count > 3)
        {
            try
            {
                totalHours = FixHours(lastrow[3].ToString()).Trim();
            }
            catch (FormatException)
            {
                totalHours = lastrow[3].ToString();
            }
        }
    }

    // If one of the rows in the cell matches the regex
    // and there is also a row that can be parsed into hours,
    // then we probably have a total hours cell. 
    public static bool isTotalTimeRow(List<Cell> lastrow)
    {
        // The letters of "total/unit/hours" without
        // "yes/no" for group field.
        Regex rxTotal = new Regex(@"([uthrli])+");
        Regex rxTime = new Regex(@"([0-9])+");
        bool matchRex = false;
        bool matchTime = false;

        foreach (var entry in lastrow)
        {
            // TotalHours match?
            var s = entry.ToString().Trim().ToLower();
            var totalmatches = rxTotal.Matches(s);
            if (totalmatches.Count >= 1) { matchRex = true; }

            // Time match?
            var time = FixHours(entry.ToString()).ToString().Trim();
            var timeMatches = rxTime.Matches(time.ToLower());
            if (timeMatches.Count >= 1) { matchTime = true; }
        }

        return matchRex && matchTime;
    }
}