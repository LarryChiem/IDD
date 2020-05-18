class TimesheetRowItem : AbstractFormObject
{
    public string date="2000-01-01";
    public string starttime="9:00";
    public string endtime="10:00";
    public string totalHours="12";
    public string group;

    public TimesheetRowItem(string date, string starttime, string endtime, string totalHours, string group) => 
        (this.date, this.starttime, this.endtime, this.totalHours, this.group) = 
        ( date, starttime, endtime, totalHours, group);
}