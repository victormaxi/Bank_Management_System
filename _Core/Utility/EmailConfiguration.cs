using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.Utility
{
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port_SSL { get; set; }
        public int GmailSMTP_port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
