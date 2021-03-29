using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.Models
{
    public class Cheque
    {
        public int ChequeId { get; set; }
        public int ChequeNumber { get; set; }
        public decimal Amount { get; set; }
        public string ChequeType { get; set; }
    }
}
