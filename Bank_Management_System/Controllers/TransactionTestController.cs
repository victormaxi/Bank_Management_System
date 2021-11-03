using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Core.Interfaces;
using _Domain.TransactionServices;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_System.Controllers
{
    [Route("api/[controller")]
    [ApiController]
    public class TransactionTestController : ControllerBase
    {

        private readonly ITransactionTest  _manager;

        public TransactionTestController (ITransactionTest test)
        {
            _manager = test;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var items = _manager.GetBillTypes();
            return Ok(items);
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var item = _manager.BillDetails(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }
    }
}
