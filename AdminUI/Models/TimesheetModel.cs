using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminUI.Models
{
    public class Timesheet
    {
        //unique ID for each timesheet
        public int TimesheetID { get; set; }
        
        //front page header info
        public string ClientName { get; set; }
        public string ClientPrime { get; set; }
        public string ProviderName { get; set; }
        public string ProviderID { get; set; }
        public string CMOrg { get; set; }
        public string SCPAName { get; set; }
        public string Service { get; set; }
        public string ModCD { get; set; }
        public string Units { get; set; }
        public string Type { get; set; }
        public string Freq { get; set; }
        
        //hours worked
        public IList<Shift> Shifts = new List<Shift>();
        public double Hours { get; set; }

        //back page info
        public string ServiceGoal { get; set; }
        public string ProgressNotes { get; set; }
        public bool ClientSignature { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ClientSigned { get; set; }
        public bool ProviderSignature { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ProviderSigned { get; set; }
        public DateTime Submitted { get; set; }
        public string Status { get; set; }
        public string RejectionReason { get; set; }
        public string AdminActivity { get; set; }
        [NotMapped]
        public LockTableRow Lock { get; set; }
    }

    public class Shift
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        
        [DataType(DataType.Time)]
        public DateTime In { get; set; }
        
        [DataType(DataType.Time)] 
        public DateTime Out { get; set; }
        public double Hours { get; set; }
        public bool Group { get; set; }
    }


}