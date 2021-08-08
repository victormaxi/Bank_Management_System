using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bank_Management_System_Web.Models.API;
using Bank_Management_System_Web.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

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

        [HttpGet]
        public async Task<JsonResult> GetBill (int id)
        {
            var selectBill = await _resource.GetBillDetailsAsync(id);
            return Json
                (selectBill);
        }
    }
}
