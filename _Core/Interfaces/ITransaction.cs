using _Core.Models;
using _Core.Utility;
using _Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _Core.Interfaces
{
    public interface ITransaction
    {
        Task<ResponseManager> TransferMoney(TransactionVM transactionVM);
        Task<object> TransactionDetails(string userId);

        Task<object> BillTypes();
        Task<object> BillDetails(int id);
        Task<object> BillPayment(BillPayment payment);
        Task<object> PaymentHistory(string userId);
    }
}
