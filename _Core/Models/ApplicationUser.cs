using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace _Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string AccountType { get; set; }
        public string ConfirmPassword { get; set; }
       

        public ApplicationUser()
        {
          FullName = FirstName + " " + LastName;
        }

        public ImageFile ImageFile { get; set; }
        public virtual List<Transactions> Transactions { get; set; } = new List<Transactions>();
        public string Roles { get; set; }
    }
}
 