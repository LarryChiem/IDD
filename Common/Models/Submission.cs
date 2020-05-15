using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using PdfSharp.Pdf;
using Microsoft.AspNetCore.Mvc;
using Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Common.Models
{
    public abstract class Submission
    {
        public int Id { get; set; }
        public DateTime Submitted { get; set; }
        public string FormType { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public string ClientName { get; set; }
        public string ClientPrime { get; set; }
        public string ServiceGoal { get; set; }
        public string ProgressNotes { get; set; }
        public string Status { get; set; } = "Pending";
        public string UserActivity { get; set; }
        public string RejectionReason { get; set; }
        public Lock LockInfo { get; set; }
        [NotMapped]
        public IList<string> UriList { get; set; }
        public string UriString
        {
            get => string.Join(",", UriList); 
            set => UriList = value.Split(',').ToList();
        }

        public abstract PdfDocument ToPdf();
        public abstract void LoadEntries(DbContext context);

    }
}