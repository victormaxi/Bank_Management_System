using _Core.Models;
using _Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.Interfaces
{
    public interface ITransactionTest
    {

        IEnumerable<Bill_Types> GetBillTypes();
        Bill_Types BillDetails(int id);
        
    }
}
