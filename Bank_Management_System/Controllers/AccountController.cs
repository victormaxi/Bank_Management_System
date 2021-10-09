using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using _Core.Interfaces;
using _Core.Models;
using _Core.Utility;
using _Core.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccounts _account;
        private readonly IHttpContextAccessor _accessor;
        public AccountController(IAccounts accounts, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor accessor)
        {
            _account = accounts;
            _signInManager = signInManager;
            _accessor = accessor;
        }
        [HttpPost]
        [Route("RegisterAsync")]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterVM model)
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
                    return BadRequest(result);
                    
                }
                return BadRequest("Error in Saving user");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("ConfirmEmailAsync/{userId}/{token}")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var userType = _accessor.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).SingleOrDefault();

                if(string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(token))
                {
                    return NotFound();
                }

                var result = await _account.ConfirmEmailAsync(userId, token);

                if(result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("loginasync")]
        public async Task<IActionResult> LoginAsync (LoginVM model)
        {
            try
            {
                var check = await _account.LockOutUser(model);
                 if (check.IsSuccess)
                {
                    //RedirectToAction("ForgotPassword",model);
                    return BadRequest(new ResponseManager {
                    Message = "User is Locked out. Please Change your password.",
                    IsSuccess = false
                    });
                }
                var result = await _account.CheckRequiresTWA(model);
                if (!result.IsSuccess)
                {
                   var user =  await _account.LoginAsync(model);
                    return Ok(user);
                }

                if (result.IsSuccess)
                {
                    var twa = new TwoStepAuthenticate();
                   

                    RedirectToAction("LoginTwoStep",twa);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("LoginTwoStep")]
        public async Task<IActionResult> LoginTwoStep(TwoStepAuthenticate model)
        {
            try
            {
                
                var result = await _account.TwoStepAuthentication(model);

                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                if (result.IsSuccess == true)
                {
                    return Ok(result);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return NotFound();
                var result = await _account.ForgotPassword(email);

                if (result.IsSuccess)
                    return Ok(result);//200

                return BadRequest(result); //400
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("UserProfile/{userId}")]
        public async Task<IActionResult> UserProfile(string userId)
        {
            try
            {
                var result = await _account.UserProfile(userId);

                if (result != null)
                {
                    return Ok(result);
                }

                return BadRequest(result);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile(EditUser user)
        {
            try
            {
                var result = await _account.UpdateUserProfile(user);

                if(result != null)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
