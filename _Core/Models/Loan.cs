using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.Models
{
    public class Loan
    {
        public int LoanId { get; set; }
        public decimal Amount { get; set; }
        public string LoanType { get; set; }
    }
}
