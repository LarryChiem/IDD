using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class MileageEntry
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public bool Group { get; set; }
        public string Status { get; set; }
        public double Miles { get; set; }
        public string PurposeOfTrip { get; set; }
    }
}
