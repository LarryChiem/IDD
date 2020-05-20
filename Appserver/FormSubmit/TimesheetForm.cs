using Newtonsoft.Json;
using System.Collections.Generic;
using Appserver.TextractDocument;
using System;
using System.Linq;

public class TimesheetForm: AbstractFormObject{
    
    private List<TimesheetRowItem> times = new List<TimesheetRowItem>();

    public TimesheetForm(){
    }

    public int units { get; set; }
    public string type { get; set; }
    public string frequency { get; set; }
    
    [JsonProperty("timesheet")]
    [JsonConverter(typeof(TimesheetRowConverter))]
    internal List<TimesheetRowItem> Times { get => times; set => times = value; }
    public string totalHours { get; set; }
    
    public void addTimeRow(string date, string start, string end, string total, string group) => 
        this.Times.Add(new TimesheetRowItem(date, start, end, total, group));

    protected override void AddTables(List<Table> tables)
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
            addTimeRow(row[0].ToString().Trim(),
              FixHours(row[1].ToString()).ToString().Trim(),
              FixHours(row[2].ToString()).ToString().Trim(),
              FixHours(row[3].ToString()).ToString().Trim(),
              ConvertInt(row[4].ToString()).ToString().Trim());
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
    protected override void AddBackForm(Page page)
    {
        var formitems = page.GetFormItems();

        serviceGoal = formitems[6].Value.ToString().Trim();
        progressNotes = formitems[7].Value.ToString().Trim();
        employerSignDate = formitems[8].Value.ToString().Trim();
        employerSignature = !string.IsNullOrEmpty(employerSignDate);
        providerSignDate = formitems[10].Value.ToString().Trim();
        providerSignature = !string.IsNullOrEmpty(providerSignDate);
    }
}