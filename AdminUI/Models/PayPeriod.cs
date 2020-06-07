using System;
using System.ComponentModel.DataAnnotations;

namespace AdminUI.Models
{
    public class PayPeriod
    {
        public int Id { get; set; }
        [Display (Name="Date From")]
        [DataType(DataType.Date)] 
        public DateTime DateFrom { get; set; }
        [Display (Name="Date To")]
        [DataType(DataType.Date)]
        public DateTime DateTo { get; set; }
        [Display (Name="Current Pay Period")]
        public bool Current { get; set; }
    }
}
