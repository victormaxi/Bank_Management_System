using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace _Core.ViewModels
{
   public class AddImageVM
    {
        [Display(Name = "Profile Image")]
        public IFormFile ? Photo { get; set; } = null;
    }
}
