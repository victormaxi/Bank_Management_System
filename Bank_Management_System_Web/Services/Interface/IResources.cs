using _Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bank_Management_System_Web.Services.Interface
{
    public interface IResources
    {
        ApplicationUser GetUserDetails(string id);

       Task<Bill_Types> GetBillDetailsAsync(int id);
        Task<IEnumerable<Bill_Types>> GetBills();
    }
}
