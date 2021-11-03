using _Core.Interfaces;
using _Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace _Domain.TransactionServices
{
    public class TransactionTestManager : ITransactionTest
    {
        public Bill_Types BillDetails(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Bill_Types> GetBillTypes()
        {
            throw new NotImplementedException();
        }
    }
}
