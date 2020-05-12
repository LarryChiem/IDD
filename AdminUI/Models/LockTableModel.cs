using System.Collections.Generic;
using Common.Models;

namespace AdminUI.Models
{
    public class LockTableModel
    {
        
        public IList<Submission> Submissions;
        public int TotalPages;
        public int TotalSubmissions;
        public int PerPage;
        public int Page;

        //keep track of the current sort order
        public string SortOrder;
    }
}