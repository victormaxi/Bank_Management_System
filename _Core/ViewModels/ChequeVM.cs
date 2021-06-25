using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace _Core.ViewModels
{
    public class ChequeVM
    {
        [Required(ErrorMessage ="Please Cheque number can not empty")]
        [Display(Name ="Cheque Number")]
        public int ChequeNumber { get; set; }
        [Required]
        [Display(Name ="Amount")]
        public decimal Amount { get; set; }
        [Required]
        [Display(Name ="Cheque Type")]
        public string ChequeType { get; set; }
    }
}
