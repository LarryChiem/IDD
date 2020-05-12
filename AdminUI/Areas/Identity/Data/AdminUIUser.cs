using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AdminUI.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the AdminUIUser class
    public class AdminUIUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }
        [PersonalData]
        public IList<Filter> Filters { get; set; }
    }
}
