using _Core.Models;
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
        Task<object> LoginAsync(LoginVM model);
        Task<ResponseManager> ConfirmEmailAsync(string userId, string token);
        Task<ResponseManager> TwoStepAuthentication(TwoStepAuthenticate model);
        Task<ResponseManager> CheckRequiresTWA(LoginVM model);
        Task<ResponseManager> LockOutUser(LoginVM model);
        Task<ResponseManager> ForgotPassword(string email);
        Task<object> GetUserProfile(string userId);
        Task<object> UserProfile(string userId);
        Task<object> UpdateUserProfile(EditUser user);
    }
}
