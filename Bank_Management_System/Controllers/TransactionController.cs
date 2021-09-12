using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Core.Interfaces;
using _Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {

        private readonly ITransaction _transaction;

        public TransactionController(ITransaction transaction)
        {
            _transaction = transaction;
        }

        [Route("GetBills")]
        public async Task<ActionResult<IEnumerable<Bill_Types>>> GetBills()
        {
            try
            {
                var bills = await _transaction.BillTypes();
                return Ok(bills);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Route("GetBillDetails/{id}")]
        public async Task<ActionResult<object>> GetBillDetails(int id)
        {
            try
            {
                var bill = await _transaction.BillDetails(id);
               
                return Json(bill);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
