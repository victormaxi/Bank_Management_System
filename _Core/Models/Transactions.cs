using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.Models
{
    public class Transactions
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public Decimal Amount { get; set; }
        public string RecipientName { get; set; }
        public DateTime DateTime { get; set; }
    }
}
