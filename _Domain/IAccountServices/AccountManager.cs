using _Core.Interfaces;
using _Core.Models;
using _Core.Utility;
using _Core.ViewModels;
using _Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
//using iTextSharp.text.html.simpleparser;

namespace _Domain.IAccountServices
{
    public class AccountManager : IAccounts
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContext;
        private IConfiguration _configuration;

        public AccountManager(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
            _httpContext = httpContext;
            _configuration = configuration;
        }

        public async Task<ResponseManager> CaptureImage(string name)
        {
            try
            {
                var files = _httpContext.HttpContext.Request.Form.Files;
               
                if(files != null)
                {
                    foreach( var file in files)
                    {
                        if(file.Length > 0)
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
                            if(!string.IsNullOrEmpty(filePath))
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
            catch(Exception ex)
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

                if(exist == null)
                {
                   if(model.Password != model.ConfirmPassword)
                    {
                        return new ResponseManager()
                        {
                            Message = "Password dosen't match with Confirm Password",
                            IsSuccess = false,
                        };
                    }
                    string uniqueFileName = UploadedFile(model);
                   
                    var checkCount = _dbContext.Users.Count() == 0;
                    

                    if(checkCount == true)
                    {
                        var IdentityUser1 = new ApplicationUser()
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            UserName = model.Username,
                            Email = model.Email,
                            PasswordHash = model.Password,
                            ImageUrl = uniqueFileName,
                            AccountType = AccountType.Admin

                        };
                        var result = await _userManager.CreateAsync(IdentityUser1, model.Password);
                        if(result.Succeeded)
                        {
                            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(IdentityUser1);
                            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmationToken);
                            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                            var callbackUrl = $"{_configuration["WebUrl"]}/account/ConfirmEmail?userid={IdentityUser1.Id}&token={validEmailToken}";

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
                        ImageUrl = uniqueFileName,
                        AccountType = AccountType.Customer

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
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseManager> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if(user == null)
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
;            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<ResponseManager> LoginAsync (LoginVM model)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(model.Email);
                if(userExist == null)
                {
                    return new ResponseManager
                    {
                        Message = "User doesn't exist",
                        IsSuccess = false
                    };
                }

                var result = await _userManager.CheckPasswordAsync(userExist,model.Password);
                if(!result)
                {
                    return new ResponseManager
                    {
                        Message = "Invalid Password",
                        IsSuccess = false
                    };
                }

                var claims = new[]
                {
                    new Claim ("UserName", userExist.FullName),
                    new Claim(ClaimTypes.NameIdentifier,userExist.Id)
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
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Upload Image from folder 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        #region 
        private string UploadedFile (RegisterVM model)
        {
            try
            {
                string uniqueFileName = null;

                if (model.Image != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath,FileMode.Create))
                    {
                        model.Image.CopyToAsync(fileStream);
                    }
                }
                return uniqueFileName;

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
        private async void StoreInDatabase(byte[] imageBytes)
        {
            try
            {
                if(imageBytes != null)
                {
                    string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                    string imageUrl = string.Concat("data:image/jpg;base64,", base64String);

                    ImageStore imageStore = new ImageStore()
                    {
                        CreateDate = DateTime.Now,
                        ImageBase64String = imageUrl,
                        ImageId = 0
                    };

                   await _dbContext.ImageStores.AddAsync(imageStore);
                   await  _dbContext.SaveChangesAsync();
                }

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

       
    }
}
