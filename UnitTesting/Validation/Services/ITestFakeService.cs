using _Core.Interfaces;
using _Core.Models;
using _Core.Utility;
using _Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting.Validation.Services
{
    public class ITestFakeService : ITransactionTest
    {
        private readonly List<Bill_Types> _Types;
        public ITestFakeService()
        {
            _Types = new List<Bill_Types>()
          {
            new Bill_Types {Id=1, Bill_Name = "GOTV", Amount = "#3500"},
                new Bill_Types {Id=2,Bill_Name = "DSTV", Amount = "#6000"},
                new Bill_Types {Id=3,Bill_Name = "EKECD", Amount = "#10000"},
                new Bill_Types {Id=4,Bill_Name = "AIR TIME", Amount = "#4000"}
            };
        }

        public Bill_Types BillDetails(int id)
        {
            return _Types.Where(a => a.Id == id).FirstOrDefault();
        }

        public IEnumerable<Bill_Types> GetBillTypes()
        {
            return _Types;
        }
    }
}
