using System.Collections.Generic;

namespace Common.Models
{
    public class MileageForm : Submission
    {
        public double TotalMiles { get; set; }
        public ICollection<MileageEntry> MileageEntries { get; set; }
    }
}
