using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Appserver.Models
{
    public class SubmissionStaging
    {
        public int Id { get; set; }
        public string UriString { get; set; }
        public string ParsedTextractJSON { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public AbstractFormObject.FormType formType { get; set; }

    }
}
