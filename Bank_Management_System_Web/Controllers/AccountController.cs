using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using _Core.Models;
using _Core.Utility;
using _Core.ViewModels;
using Bank_Management_System_Web.Models.API;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Bank_Management_System_Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ApiRequestUri _apiRequestUri;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //private readonly HttpClient _client;

        
        public AccountController(IOptionsSnapshot<ApiRequestUri> options ,IHttpContextAccessor httpContext, IHostingEnvironment appEnvironment, IWebHostEnvironment webHostEnvironment) : base(httpContext.HttpContext)
        {

            _httpContext = httpContext;
            _apiRequestUri = options.Value;
            _appEnvironment = appEnvironment;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
       
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    //var result = await 
                    return View(model);
                }


            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_apiRequestUri.BaseUri);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

               

                string UniqueFileName = null;

                if(model.Photo != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "image");
                    UniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, UniqueFileName);
                   await  model.Photo.CopyToAsync(new FileStream(filePath, FileMode.Create));
                }

                var user = new RegisterVM()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Username = model.Username,
                    AccountType = model.AccountType,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword,
                    Roles = model.Roles,
                   // ImageUrl = url
                   PhotoPath = UniqueFileName,
                   ImageVM = new ImageVM
                   {
                       ImagePath = model.PhotoPath,
                       
                   }
                };


            var uri = string.Format(_apiRequestUri.RegisterAsync);

            // StringContent content = new StringContent(JsonConvert.SerializeObject());

            HttpResponseMessage response = (HttpResponseMessage)null;

            response = await httpClient.PostAsJsonAsync(uri, user);
                string json = JsonConvert.SerializeObject(user);

                if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ConfirmYourEmail");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
            }
        

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return View();
        }

        public IActionResult ConfirmYourEmail()
        {
            return View();
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Error");
                }

                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_apiRequestUri.BaseUri);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var uri = string.Format(_apiRequestUri.ConfirmEmailAsync, userId,token);

                    HttpResponseMessage response = await httpClient.GetAsync(uri);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("EmailConfirmed");
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                    }
                }
                return RedirectToAction("EmailConfirmationFailed");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_apiRequestUri.BaseUri);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    var user = new LoginVM()
                    {
                        Email = model.Email,
                        Password = model.Password,
                       
                    };

                    var uri = string.Format(_apiRequestUri.LoginAsync);

                    StringContent content = new StringContent(JsonConvert.SerializeObject(model));

                    HttpResponseMessage response = (HttpResponseMessage)null;

                    response = await httpClient.PostAsJsonAsync(uri, user);

                    if (response.IsSuccessStatusCode)
                    {
                        #region cookie Implementation
                        string stringJWT = response.Content.ReadAsStringAsync().Result;

                        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(stringJWT);

                        var resp = new AuthResponse
                        {
                            fullName = authResponse.fullName,
                            userId = authResponse.userId,
                            //role = authResponse.role,
                            username = authResponse.username,
                            email = authResponse.email,
                            token = authResponse.token,
                            expiryDate = authResponse.expiryDate
                        };
                        ViewBag.FullName =  resp.fullName;
                        var claims = new List<Claim>
                        {
                            new Claim("Id", authResponse.userId.ToString()),
                            new Claim(ClaimTypes.Name, authResponse.username),
                            new Claim("Fullname", authResponse.fullName),
                           // new Claim(ClaimTypes.Role, authResponse.role),
                            new Claim("JWT", authResponse.token),
                            new Claim("Email", authResponse.email)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            // Refreshing the authentication session should be allowed.

                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                            //The time at which the authentication ticket expries. A value set here overrides the ExpireTimeSpan option of CookieAuthenticationOptions set with AddCookie.

                            IsPersistent = true,
                            // Whether the authentication session is persisted across multiple requests. When used with cookies, controls. Whether th cookie's liftime is absolute (matching the lifetime of the authentication ticket) or session-based.
                        };

                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
                       // var checkUserImage = CheckUserImageExist(authResponse.userId);

                        return RedirectToAction( "CheckUserImageExist","Account", new {userId = authResponse.userId} );
                    }
                    #endregion
                    ViewBag.incorrectPassword = "Incorrect Password";
                    ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                }
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> CheckUserImageExist(string userId)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(_apiRequestUri.BaseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var uri = string.Format(_apiRequestUri.CheckUserImage, userId);

                HttpResponseMessage response = await httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                  return RedirectToAction("UserProfile", new { userId = userId });
                }
                else
                {
                    return RedirectToAction("ProfileImage");
                }

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IActionResult Welcome ()
        {
            return View();
        }
        public IActionResult EmailConfirmed()
        {
            return View();
        }

        public IActionResult EmailConfirmationFailed()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Profile(string userId)
        {
            try
            {
                userId = GetUserId();

                if (ModelState.IsValid)
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.BaseAddress = new Uri(_apiRequestUri.BaseUri);
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                        var uri = string.Format(_apiRequestUri.Profile, userId);
                        HttpResponseMessage res = await httpClient.GetAsync(uri);
                    
                    if (res.IsSuccessStatusCode)
                        {
                            var apiTask = res.Content.ReadAsStringAsync();
                            var responseString = apiTask.Result;
                            var model = JsonConvert.DeserializeObject<List<ProfileVM>>(responseString);
                            if (res.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                ViewBag.Message = "Unauthorized";
                            }
                            else
                            {
                                return View(model);
                            }

                        }
                            
                    }
                return View();
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IActionResult ProfileImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProfileImage(AddImageVM imageVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    
                    return View(imageVM);
                }

                //ImageFile image = new ImageFile();
                var userId = GetUserId();
                #region
                //long size = formFile.Sum(x => x.Length);

                //foreach(IFormFile file in formFile)
                //{
                //    FileInfo fi = new FileInfo(file.FileName);
                //    var newFilename = fi;

                //    string path = Path.Combine(_apiRequestUri.UserImageLocation + $"\\{userId}");

                //    if (!Directory.Exists(path))
                //    {
                //        Directory.CreateDirectory(path);
                //    }

                //    var uniqueNo = new Random().Next(0, 100000);

                //    string imagePath = path + "\\" + uniqueNo + newFilename;

                //    var stream = new FileStream(imagePath, FileMode.Create);
                //    await file.CopyToAsync(stream);

                //    image.ImagePath = imagePath.Replace("wwwroot", "").Replace("/\\", "/").Replace("\\", "/");

                //    image.UserId = userId;
                #endregion

                string UniqueFileName = null;

                if (imageVM.Photo != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "image");
                    UniqueFileName = Guid.NewGuid().ToString() + "_" + imageVM.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, UniqueFileName);
                    await imageVM.Photo.CopyToAsync(new FileStream(filePath, FileMode.Create));
                }

                var imageFile = new ImageVM();
                imageFile.ImagePath = UniqueFileName;
                imageFile.UserId = userId;
                

                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(_apiRequestUri.BaseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                

                var uri = string.Format(_apiRequestUri.ImageUpload);
                HttpResponseMessage response = (HttpResponseMessage)null;

                response = await httpClient.PostAsJsonAsync(uri,imageFile);

                String content = JsonConvert.SerializeObject(imageFile);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("UserProfile", new {userId = userId });
                }

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile(string userId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var httpClient = new HttpClient())

                    {
                        httpClient.BaseAddress = new Uri(_apiRequestUri.BaseUri);
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                        var uri = string.Format(_apiRequestUri.UserProfile, userId);

                        HttpResponseMessage res = await httpClient.GetAsync(uri);

                        if (res.IsSuccessStatusCode)
                        {
                            var apiTassk = res.Content.ReadAsStringAsync();
                            var responseString = apiTassk.Result;
                            var model = JsonConvert.DeserializeObject<ProfileVM>(responseString);
                            if (res.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                ViewBag.Message = "Unauthorized";
                            }
                            else
                            {
                                return View(model);
                            }
                        }
                    }
                }
        
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return View();
        }

      
    }
}

