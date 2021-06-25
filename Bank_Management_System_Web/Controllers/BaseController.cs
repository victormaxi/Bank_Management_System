using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using _Core.Models;
using _Core.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_System_Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly HttpContext context;
    

        public BaseController(HttpContext httpContext)
        {
            this.context = httpContext;
        }

     
        

        // returns the current authenticated account (null if not logged in)
        public ApplicationUser ApplicationUser => (ApplicationUser)HttpContext.Items["ApplicationUser"];

        [NonAction]
        public string GetUserId()
        {
            var claimsIdentity = context.User.Identity as ClaimsIdentity;
            if (claimsIdentity.IsAuthenticated == false)
                return string.Empty;
            if (claimsIdentity.FindFirst("Id") != null)
            {
                return claimsIdentity.FindFirst("Id").Value.ToString();
            }
            return string.Empty;
        }

        //public string UploadImage()
        //{

        //}
       
        
    }
}
