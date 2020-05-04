using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appserver.Models
{
    public class SubmissionStaging
    {
        public int Id { get; set; }
        public string UriString { get; set; }
        public string ParsedTextractJSON { get; set; }

    }
}
