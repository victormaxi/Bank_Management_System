using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using _Core.Models;
using _Core.ViewModels;
using Bank_Management_System_Web.Models.API;
using Bank_Management_System_Web.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Bank_Management_System_Web.Controllers
{
    public class TransactionWebController : BaseController
    {
        private readonly ApiRequestUri _apiRequestUri;
        private readonly IHttpContextAccessor _HttpContext;
        private readonly IResources _resource;
        private readonly IHostingEnvironment _appEnvironment;



        public TransactionWebController(IOptionsSnapshot<ApiRequestUri> options, IHttpContextAccessor _httpContext, IResources resource, IHostingEnvironment appEnvironment) : base(_httpContext.HttpContext)
        {
            _resource = resource;
            _appEnvironment = appEnvironment;
            _apiRequestUri = options.Value;
            _HttpContext = _httpContext;

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            var listOfBills = await _resource.GetBills();

            ViewData["Bills"] = new SelectList(listOfBills,"Id", "Bill_Name");

          //  ViewBag.BillLists = new SelectList(listOfBills, "Id", "Name");

            //ViewBag.message = listOfBills;
            return View();
        }

        public async Task<IActionResult> Index2()
        {

            var listOfBills = await _resource.GetBills();

            return Json(listOfBills);
        }

        [HttpGet("Index3/{id}")]
        public async Task<IActionResult> Index3(int id)
        {

            var bills = await _resource.GetBillDetailsAsync(id);
            var newBill = new Bill_Types()
            {
                Id = bills.Id,
                Bill_Name = bills.Bill_Name,
                Amount = bills.Amount
            };

            return Json(newBill);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var bills = await _resource.GetBillDetailsAsync(id);
            return View(bills);
        }

        [HttpGet("_BillDetails")]
        public async Task<PartialViewResult> _BillDetails(int id)
        {
            var bills = await _resource.GetBillDetailsAsync(id);
            return PartialView(bills);
        }

        [HttpGet("GetBIll/{id}")]
        public async Task<JsonResult> GetBill (int id)
        {
            var selectBill = await _resource.GetBillDetailsAsync(id);
            return Json
                (selectBill);
        }
        [HttpPost]
        public async Task<IActionResult>BillPayment(Bill_Types bill2)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View();

                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(_apiRequestUri.BaseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var userId2 = GetUserId();

                var bill = new BillPayment()
                {
                    userId = userId2,
                    BillId = bill2.Id
                };

                var uri = string.Format(_apiRequestUri.BillPayment);

                HttpResponseMessage response = (HttpResponseMessage)null;
                response = await httpClient.PostAsJsonAsync(uri, bill);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("UserProfile", "Account", new { userId = userId2 });
                }
                if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                }
                return Json(bill);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentHistory()
        {
            try
            {
                if (!ModelState.IsValid)
                    return View();

                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(_apiRequestUri.BaseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var userId = GetUserId();
                var uri = string.Format(_apiRequestUri.PaymentHistory, userId);
                HttpResponseMessage response = (HttpResponseMessage)null;
                response = await httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var apiTassk = response.Content.ReadAsStringAsync();
                    var responseString = apiTassk.Result;
                    var model = JsonConvert.DeserializeObject<List<PaymentLogsVM>>(responseString);

                    return View(model);
                }

                return View();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
