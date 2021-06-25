﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bank_Management_System_Web.Models.API
{
    public class ApiRequestUri
    {
        public string BaseUri { get; set; }
        public string RegisterAsync { get; set; }
        public string LoginAsync { get; set; }
        public string ForgotPassword { get; set; }
        public string ConfirmEmailAsync { get; set; }
        public string Profile { get; set; }
        public string UserImageLocation { get; set; }
        public string ImageUpload { get; set; }
        public string UserProfile { get; set; }
    }
}
