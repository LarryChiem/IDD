using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class PWAsubmission
    {
        public PWAsubmissionVals customerName { get; set; }
        public PWAsubmissionVals prime { get; set; }
        public PWAsubmissionVals submissionDate { get; set; }
        public PWAsubmissionVals providerName { get; set; }
        public PWAsubmissionVals providerNumber { get; set; }
        public PWAsubmissionVals service { get; set; }
        public PWAsubmissionVals totalHours { get; set; }
        public PWAsubmissionVals serviceGoal { get; set; }
        public PWAsubmissionVals progressNotes { get; set; }
        public PWAsubmissionVals employerSignDate { get; set; }
        public PWAsubmissionVals employerSignature { get; set; }
        public PWAsubmissionVals providerSignDate { get; set; }
        public PWAsubmissionVals providerSignature { get; set; }
        public PWAsubmissionVals authorization { get; set; }
        public PWAsubmissionVals providerInitials { get; set; }
        public PWAsubmissionVals scpa_name { get; set; }
        public PWAsubmissionVals cmorg { get; set; }
        public PWAserviceDeliveredVals serviceDeliveredOn { get; set; }
    }

    public class PWAsubmissionVals
    {
        public string value { get; set; }
        public bool wasEdited { get; set; }
    }

    public class PWAserviceDeliveredVals
    {
        public bool wasEdited { get; set; }
        public List<PWAserviceDeliveredListVals> value { get; set; }
    }

    public class PWAserviceDeliveredListVals
    {
        public string date { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string totalHours { get; set; }
        public string group { get; set; }
        public bool wasEdited { get; set; }
    }
}
