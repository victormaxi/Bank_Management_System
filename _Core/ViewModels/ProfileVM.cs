using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace _Core.ViewModels
{
    public class ProfileVM
    {
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Display(Name ="Profile Image")]
        public string Image { get; set; }
        [Display(Name ="User Name")]
        public string UserName { get; set; }
        [Display(Name ="Account Type")]
        public string AccountType { get; set; }
        [Display(Name ="Account Balance")]
        public decimal AccountBalance { get; set; }
    }
}
