using _Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace _Core.ViewModels
{
    public class RegisterVM
    {
        [Required (ErrorMessage = "Please your Enter First Name")]
        [Display(Name = " First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please your Enter Last Name")]
        [Display(Name = " Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please your Enter Username")]
        [Display(Name = " Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please your Enter Email Address")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage ="Please Choose an Account Type")]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage ="Must be between 5 and 255 characters", MinimumLength = 5)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage ="Confirm Password is requred")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name ="Compare Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string PhotoPath { get; set; }

        //public string ImageUrl { get; set; }

        //[Required(ErrorMessage = "Please upload a photo")]
        [Display(Name = "Profile Image")]
        public IFormFile? Photo { get; set; } = null;


    }
}
