using System.Collections.Generic;
using AdminUI.Areas.Identity.Data;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminUI.Models
{
    public class HomeModel
    {
        //submissions to display + pagination variables
        public IList<Submission> Submissions;
        public int TotalPages;
        public int TotalSubmissions;
        public int PerPage;
        public int Page;

        //keep track of the current sort order
        public string SortOrder;

        //keep track of current filters
        public string PName; 
        public string CName;
        public string Prime;
        public string DateFrom;
        public string DateTo;
        public string ProviderId;
        public string Status;
        public string FormType;

        //List of selectable filters
        public IList<Filter> Filters;

        //do we need to warn the users we're low on pay periods?
        public bool Warning;
    }
}
