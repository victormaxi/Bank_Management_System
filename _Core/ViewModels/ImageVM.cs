using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace _Core.ViewModels
{
    public class ImageVM
    {
        public string UserId { get; set; }
        public string ImagePath { get; set; }
        

        [Display(Name = "Profile Image")]
        public IFormFile? Photo { get; set; } = null;
        public RegisterVM RegisterVM { get; set; }
    }
}
