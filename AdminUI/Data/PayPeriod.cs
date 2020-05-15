using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;

namespace AdminUI.Data
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
