using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.Models
{
    public class PaymentLogs
    {
        public int Id { get; set; }
        public string BillName { get; set; }
        public string Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid ReferenceNumber { get; set; }
        public string UserId { get; set; }
    }
}
