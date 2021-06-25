using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.Utility
{
    public class AuthResponse
    {
        public string fullName { get; set; }
        public string userId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string token { get; set; }
       // public string role { get; set; }
        public DateTime expiryDate { get; set; }
    }
}
