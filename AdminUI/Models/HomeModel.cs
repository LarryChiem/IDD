using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminUI.Models
{
    public class HomeModel
    {
        public IList<Timesheet> Sheets;


        //keep track of the current sort order
        public string SortOrder;

        //keep track of current filters
        public string PName; 
        public int? Id;
        public string CName;
        public string Prime;
        public string DateFrom;
        public string DateTo;
        public string ProviderId;
        public string Status;
    }
}
