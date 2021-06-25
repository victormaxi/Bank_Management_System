using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_System.Controllers
{
    [Controller]
    public abstract class BaseController : ControllerBase
    {
        // returns the current authenticated account (null if not logged in)
        public ApplicationUser ApplicationUser => (ApplicationUser)HttpContext.Items["ApplicationUser"];
    }
}
