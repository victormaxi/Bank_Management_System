using _Core.Interfaces;
using _Core.Models;
using _Core.Utility;
using _Core.ViewModels;
using _Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                var bill = await _dbContext.Bill_Types.FindAsync(id);

                return bill;
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
    }
}
