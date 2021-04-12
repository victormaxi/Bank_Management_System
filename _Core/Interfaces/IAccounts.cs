using _Core.Utility;
using _Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _Core.Interfaces
{
   public interface IAccounts
    {
        Task<ResponseManager> Register(RegisterVM model);
        Task<ResponseManager> LoginAsync(LoginVM model);
        Task<ResponseManager> ConfirmEmailAsync(string userId, string token);
    }
}
