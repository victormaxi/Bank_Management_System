using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string AccountType { get; set; }
        public string ImageUrl { get; set; }
        public int ImageStoreId { get; set; }
        public ImageStore ImageStore { get; set; }

        public ApplicationUser()
        {
          FullName = FirstName + " " + LastName;
        }
    }
}
 