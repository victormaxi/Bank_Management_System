using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.ViewModels
{
   public class TransactionVM
    {
        public int AccountNumber { get; set; }
        public string Amount { get; set; }
        public string RecipientName { get; set; }
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }
    }
}
