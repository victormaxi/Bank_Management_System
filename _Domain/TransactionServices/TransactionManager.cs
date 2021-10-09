using _Core.Interfaces;
using _Core.Models;
using _Core.Utility;
using _Core.ViewModels;
using _Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace _Domain.TransactionServices
{
    public class TransactionManager:ITransaction
    {
        private readonly ApplicationDbContext _dbContext;

        public TransactionManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<object> BillDetails(int id)
        {
            try
            {
                var bill = await _dbContext.Bill_Types.SingleOrDefaultAsync(c => c.Id==id);

                var newBill = new Bill_Types()
                {
                    Id = bill.Id,
                    Bill_Name = bill.Bill_Name,
                    Amount = bill.Amount
                };
                return newBill;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> BillPayment(BillPayment payment)
        {
            try
            {
                var billid = payment.BillId;
                var userid = payment.userId;
                var bill = await _dbContext.Bill_Types.FirstOrDefaultAsync(b => b.Id == billid);
                var user = await _dbContext.Users.FindAsync(userid);
                var strBill = bill.Amount.Substring(1);
                var strAmount = user.Amount.Substring(1);
                var convertToIntBill = Convert.ToInt32(strBill);
                var convertToIntBalance = Convert.ToInt32(strAmount);
                var userBalance = convertToIntBalance - convertToIntBill;
            
                user.Amount =  "#" + Convert.ToString(userBalance);

                var log = new PaymentLogs()
                {
                    
                    BillName = bill.Bill_Name,
                    Amount = bill.Amount,
                    Date = DateTime.Now.AddMinutes(0),
                    ReferenceNumber = Guid.NewGuid(),
                    UserId = user.Id
                };

                _dbContext.PaymentLogs.Update(log);
                var result = _dbContext.Users.Update(user);
                var save = await _dbContext.SaveChangesAsync();
                if (save != 0)
                {
                    var success = "Transaction was Successful";
                    return success;
                }
               
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> BillTypes()
        {
            try
            {
                var bills = await _dbContext.Bill_Types.ToListAsync();

                return bills;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> TransactionDetails(string userId)
        {
            try
            {
                var trans = await _dbContext.Transactions.FindAsync(userId);

                return trans;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseManager> TransferMoney(TransactionVM transactionVM)
        {
           try
            {

                var transMoney = new Transactions
                {
                    AccountNumber = transactionVM.AccountNumber,
                    Amount = transactionVM.Amount,
                    RecipientName = transactionVM.RecipientName,
                    UserId = transactionVM.UserId
                };
                    await _dbContext.Transactions.AddAsync(transMoney);
                await _dbContext.SaveChangesAsync();

                return new ResponseManager
                {
                    Message = "Transfer was successful",
                    IsSuccess = true
                };
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> PaymentHistory(string userId)
        {
            try
            {
                List<PaymentLogs> logs = (from paymentlog in _dbContext.PaymentLogs.Take(10) select paymentlog).ToList();

                return logs;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
