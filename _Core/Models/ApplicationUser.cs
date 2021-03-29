using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string AccountType { get; set; }
    }
}
