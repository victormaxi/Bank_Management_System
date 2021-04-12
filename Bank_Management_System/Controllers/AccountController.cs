using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Core.Interfaces;
using _Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_System.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly IAccounts _account;
        public AccountController(IAccounts accounts)
        {
            _account = accounts;
        }
        public async Task<object> Register(RegisterVM model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var result = await _account.Register(model);
                    if(result.IsSuccess)
                    {
                        return Ok(result);
                    }
                    
                }
                return BadRequest("Error in Saving user");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
