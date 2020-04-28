using System.Collections;
using System.Collections.Generic;

namespace Common.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public ICollection<Filter> Filters { get; set; }
    }
}