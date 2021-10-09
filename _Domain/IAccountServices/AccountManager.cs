using _Core.Interfaces;
using _Core.Models;
using _Core.Utility;
using _Core.ViewModels;
using _Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace _Domain.IAccountServices
{
    public class AccountManager : IAccounts
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IImageManager _imageManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConfiguration _configuration;

        public AccountManager(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IImageManager imageManager, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _imageManager = imageManager;
            _webHostEnvironment = webHostEnvironment;
            _httpContext = httpContext;
            _configuration = configuration;
        }

        public async Task<ResponseManager> CaptureImage(string name)
        {
            try
            {
                var files = _httpContext.HttpContext.Request.Form.Files;

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            //Getting FileName
                            var fileName = file.FileName;
                            //Unique filename "Guid"
                            var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                            //Getting Extension
                            var fileExtension = Path.GetExtension(fileName);
                            //Concating filname + fileExtension (uniques filename)
                            var newFileName = string.Concat(myUniqueFileName, fileExtension);
                            //Generating Path to store Photo
                            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "CameraPhoto") + $@"\{newFileName}";
                            var imageBytes1 = System.IO.File.ReadAllBytes(filePath);
                            if (!string.IsNullOrEmpty(filePath))
                            {
                                //Store to database
                                StoreInDatabase(imageBytes1);
                            }
                        }

                    }
                    return new ResponseManager()
                    {
                        Message = "Image Captured Successfully",
                        IsSuccess = true
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ResponseManager> Register(RegisterVM model)
        {
            try
            {
                if (model == null)

                    throw new NullReferenceException("Information Field is Empty!!!!");


                // Please do remeber to change unique search value "firstname" to "username"
                var exist = await _dbContext.Users.FindAsync(model.FirstName);

                if (exist == null)
                {
                    if (model.Password != model.ConfirmPassword)
                    {
                        return new ResponseManager()
                        {
                            Message = "Password dosen't match with Confirm Password",
                            IsSuccess = false,
                        };
                    }
                    //  string uniqueFileName = UploadedFile(model);

                   // var checkCount = _dbContext.Users.Count() == 0;

                    if (model.Roles == "Cashier")
                    {
                        var IdentityUser1 = new ApplicationUser()
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            UserName = model.Username,
                            Email = model.Email,
                            PasswordHash = model.Password,
                            //PhotoPath = model.PhotoPath,
                           // AccountType = model.AccountType,
                            FullName = model.FirstName + model.LastName,
                            Roles = Roles.Cashier
                          


                        };
                        var result = await _userManager.CreateAsync(IdentityUser1, model.Password);
                        if (result.Succeeded)
                        {
                            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(IdentityUser1);
                            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmationToken);
                            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                            var callbackUrl = $"{_configuration["WebUrl"]}/account/ConfirmEmail?userId={IdentityUser1.Id}&token={validEmailToken}";

                            var message = new Message(new string[] { model.Email }, "Confirm Email", callbackUrl, null);

                            _emailSender.SendConfirmEmailAsync(message);


                        }
                        //await _dbContext.Users.AddAsync(IdentityUser1);
                        //await _dbContext.SaveChangesAsync();
                    }

                    var IdentityUser2 = new ApplicationUser()
                    {

                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserName = model.Username,
                        Email = model.Email,
                        PasswordHash = model.Password,
                        //PhotoPath = model.PhotoPath,
                        AccountType = model.AccountType,
                        Roles = Roles.Customer
                         

                    };

                    var result1 = await _userManager.CreateAsync(IdentityUser2, model.Password);
                    if (result1.Succeeded)
                    {
                        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(IdentityUser2);
                        var encodedEmailToken = Encoding.UTF8.GetBytes(confirmationToken);
                        var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                        var callbackUrl = $"{_configuration["WebUrl"]}/account/ConfirmEmail?userid={IdentityUser2.Id}&token={validEmailToken}";

                        var message = new Message(new string[] { model.Email }, "Confirm Email", callbackUrl, null);

                        _emailSender.SendConfirmEmailAsync(message);


                    }
                    //await _dbContext.Users.AddAsync(IdentityUser2);
                    //await _dbContext.SaveChangesAsync();



                    return new ResponseManager()
                    {
                        Message = "New User Created Successful",
                        IsSuccess = true
                    };

                }
                return new ResponseManager()
                {
                    Message = "Registration Failed",
                    IsSuccess = false
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseManager> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return new ResponseManager
                    {
                        Message = "User not found",
                        IsSuccess = false
                    };
                }

                var decodedToken = WebEncoders.Base64UrlDecode(token);
                string normalToken = Encoding.UTF8.GetString(decodedToken);

                var result = await _userManager.ConfirmEmailAsync(user, normalToken);

                if (result.Succeeded)
                {
                    return new ResponseManager
                    {
                        Message = "Email confirmed successfully",
                        IsSuccess = true
                    };
                }
                else
                {
                    return new ResponseManager
                    {
                        Message = "Email did not successfully confirm",
                        IsSuccess = false,
                        Errors = result.Errors.Select(e => e.Description)
                    };
                }
;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<object> LoginAsync(LoginVM model)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(model.Email);
                if (userExist == null)
                {
                    return new ResponseManager
                    {
                        Message = "User doesn't exist",
                        IsSuccess = false
                    };
                }
                //var checkIfLockedOut = await LockOutUser(model);
                //if (checkIfLockedOut.IsSuccess)
                //    return new ResponseManager()
                //    {
                //        Message = "User is Locked Out. Please do change your password",
                //        IsSuccess = false
                //    };

                var result = await _userManager.CheckPasswordAsync(userExist, model.Password);

                if (!result)
                {
                    return new ResponseManager
                    {
                        Message = "Invalid Password",
                        IsSuccess = false
                    };
                }

                //var res2 = await CheckRequiresTWA(model);
                

                //if (res2.IsSuccess)
                //{

                //    var user1 = new TwoStepAuthenticate()
                //    {
                //        RememberMe = model.RememberMe,

                //    };

                //  await TwoStepAuthentication(user1);
                //    return new ResponseManager
                //    {
                //        Message = "Two step Authentication",
                //        IsSuccess = true
                //    };

                //}


                var claims = new[]
                {
                    new Claim ("FullName",userExist.LastName + userExist.FirstName),
                    new Claim ("UserName", userExist.UserName),
                    new Claim(ClaimTypes.NameIdentifier,userExist.Id)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["AuthSettings:Issuer"],
                    audience: _configuration["AuthSettings:Audince"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );

                string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

                var resp = new AuthResponse
                {
                    fullName = userExist.FirstName + " " + userExist.LastName,
                    userId = userExist.Id,
                    username = userExist.UserName,
                    token = tokenAsString,
                    email = userExist.Email,

                };

              


                return resp;
                //return new ResponseManager
                //{
                //    Message = tokenAsString,
                //    IsSuccess = true,
                //    ExpireDate = token.ValidTo
                //};
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseManager> CheckRequiresTWA(LoginVM model)
        {
            try
            {
                // var user = await _userManager.Users.SingleOrDefaultAsync(c => c.Email == model.Email);


                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return new ResponseManager()
                    {
                        Message = "User dosen't require Two factor authentication",
                        IsSuccess = false
                    };
                }

                if (result.RequiresTwoFactor)
                {
                    return new ResponseManager()
                    {
                        Message = "User requires Two factor authentication",
                        IsSuccess = true
                    };
                }
                return new ResponseManager()
                {
                    Message = "",
                    IsSuccess = false
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseManager> TwoStepAuthentication(TwoStepAuthenticate model)
        {
            try
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    return new ResponseManager()
                    {
                        Message = "",
                        IsSuccess = false
                    };
                }

                var result = await _signInManager.TwoFactorSignInAsync("Email",
                                                                                    model.TwoFactorCode,
                                                                                    model.RememberMe,
                                                                                    rememberClient: false);
                if (result.Succeeded)
                {
                    var claims = new[]
               {
                    new Claim ("UserName", user.FullName),
                    new Claim(ClaimTypes.NameIdentifier,user.Id)
                };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["AuthSettings:Issuer"],
                        audience: _configuration["AuthSettings:Audince"],
                        claims: claims,
                        expires: DateTime.Now.AddHours(1),
                        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                        );

                    string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

                    return new ResponseManager
                    {
                        Message = tokenAsString,
                        IsSuccess = true,
                        ExpireDate = token.ValidTo
                    };
                }

                return new ResponseManager()
                {
                    Message = "User is Lockedout",
                    IsSuccess = result.IsLockedOut
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseManager> LockOutUser(LoginVM model)
        {
            try
            {
                var checkIfLockOut = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (checkIfLockOut.IsLockedOut)
                {
                    return new ResponseManager()
                    {
                        Message = "User is Locked out",
                        IsSuccess = true
                    };
                }

                return new ResponseManager()
                {
                    Message = "User is not Locked out",
                    IsSuccess = false
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseManager> ForgotPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new ResponseManager()
                    {
                        Message = "No user associated with this email.",
                        IsSuccess = false
                    };
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = Encoding.UTF8.GetBytes(token);
                var validToken = WebEncoders.Base64UrlEncode(encodedToken);

                string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

                var message = new Message(new string[] { email }, "Reset your password", url, null);
                _emailSender.SendConfirmEmailAsync(message);


                return new ResponseManager()
                {
                    Message = "Please check email for link to change password",
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> UserProfile(string userId)
        {
            try
            {


                var result = await _dbContext.Users.Include(s => s.ImageFile).SingleOrDefaultAsync(s => s.ImageFile.Id == userId);

                var newResult = new ProfileVM
                {
                    FullName = result.LastName + " " + result.FirstName,
                    Image = result.ImageFile.ImagePath,
                    UserName = result.UserName,
                    AccountType = result.AccountType,
                    Account = result.Amount
                };

                if (result != null)
                {
                    var res = new ResponseManager()
                    {
                        Message = "User Profile",
                        IsSuccess = true
                    };
                    return (newResult);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        private async void StoreInDatabase(byte[] imageBytes)
        {
            try
            {
                if (imageBytes != null)
                {
                    string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                    string imageUrl = string.Concat("data:image/jpg;base64,", base64String);

                    ImageStore imageStore = new ImageStore()
                    {
                        CreateDate = DateTime.Now,
                        ImageBase64String = imageUrl,
                        ImageId = 0
                    };

                    //await _dbContext.ImageStores.AddAsync(imageStore);
                    //await  _dbContext.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<object> GetUserProfile(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<object> UpdateUserProfile(EditUser user)
        {
            try
            {
                var updateUser = await _dbContext.Users.FindAsync(user.UserId);
                if(updateUser != null)
                {
                   
                 
                    updateUser.Amount = "#" + user.Amount;
                    var result =  _dbContext.Users.Update(updateUser);
                    var save = await _dbContext.SaveChangesAsync();
                    if(save == 1)
                    {
                        return result;
                    }
                    return null;
                }

                return null;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
