using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.ViewModels
{
    public class PaymentLogsVM
    {

        public int BillId { get; set; }
        public string BillName { get; set; }
        public string Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid ReferenceNumber { get; set; }
    }
}
