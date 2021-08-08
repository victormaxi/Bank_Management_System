using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace _Core.Models
{
    public class Transactions
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public string Amount { get; set; }
        public string RecipientName { get; set; }
        public DateTime DateTime { get; set; }

        //[ForeignKey("UserId")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
