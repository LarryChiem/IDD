using Newtonsoft.Json;
using System.Collections.Generic;
public class TimesheetForm: AbstractFormObject{
    
    private List<TimesheetRowItem> times = new List<TimesheetRowItem>();

    public TimesheetForm(){
    }

    public int id { get; set; }
    public string clientName {get; set;}
    public string prime { get; set; }
    public string providerName { get; set; }
    public string providerNum { get; set; }
    public string brokerage { get; set; }
    public string scpaName { get; set; }
    public string serviceAuthorized { get; set; }
    public int units { get; set; }
    public string type { get; set; }
    public string frequency { get; set; }
    
    [JsonProperty("timesheet")]
    [JsonConverter(typeof(TimesheetRowConverter))]
    internal List<TimesheetRowItem> Times { get => times; set => times = value; }
    public string totalHours { get; set; }
    public string serviceGoal { get; set; }
    public string progressNotes { get; set; }
    public bool employerSignature { get; set; }
    public string employerSignDate { get; set; }
    public bool providerSignature { get; set; }
    public string providerSignDate { get; set; }
    public bool authorization { get; set; }
    public bool approval { get; set; }
    public string review_status { get; set; } = "Pending";
    public void addTimeRow(string date, string start, string end, string total, string group) => 
        this.Times.Add(new TimesheetRowItem(date, start, end, total, group));
}