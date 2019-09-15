using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace kudvenkat.DataAccess.Models {
    public class ApplicationUser : IdentityUser {
        public string City { get; set; }
    }
}
