using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class PWAsubmission
    {
        public int id { get; set; }
        public int formChoice { get; set; }
        public PWAsubmissionVals clientName { get; set; }
        public PWAsubmissionVals prime { get; set; }
        public PWAsubmissionVals submissionDate { get; set; }
        public PWAsubmissionVals providerName { get; set; }
        public PWAsubmissionVals providerNum { get; set; }
        public PWAsubmissionVals serviceAuthorized { get; set; }
        public PWAsubmissionVals totalHours { get; set; }
        public PWAsubmissionVals serviceGoal { get; set; }
        public PWAsubmissionVals progressNotes { get; set; }
        public PWAsubmissionVals employerSignDate { get; set; }
        public PWAsubmissionVals employerSignature { get; set; }
        public PWAsubmissionVals providerSignDate { get; set; }
        public PWAsubmissionVals providerSignature { get; set; }
        public PWAsubmissionVals authorization { get; set; }
        public PWAsubmissionVals approval { get; set; }
        public PWAsubmissionVals scpaName { get; set; }
        public PWAsubmissionVals brokerage { get; set; }
        public PWAtimesheetEntries timesheet { get; set; }
    }

    public class PWAsubmissionVals
    {
        public string value { get; set; }
        public bool wasEdited { get; set; }
    }

    public class PWAtimesheetEntries
    {
        public bool wasEdited { get; set; }
        public ICollection<PWAtimesheetVals> value { get; set; }
    }

    public class PWAtimesheetVals
    {
        public string date { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public string totalHours { get; set; }
        public string group { get; set; }
        public bool wasEdited { get; set; }
    }
}
