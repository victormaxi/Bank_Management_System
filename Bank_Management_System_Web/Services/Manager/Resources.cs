using _Core.Models;
using _Data;
using Bank_Management_System_Web.Models.API;
using Bank_Management_System_Web.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bank_Management_System_Web.Services.Manager
{
    public class Resources : IResources
    {
        private readonly ApiRequestUri _apiRequestUri;
        private readonly ApplicationDbContext _dataContext;
        private readonly IHostingEnvironment _env;

        public Resources(IOptionsSnapshot<ApiRequestUri> options, ApplicationDbContext dataContext, IHostingEnvironment env)
        {
            _apiRequestUri = options.Value;
            _dataContext = dataContext;
            _env = env;
        }
        public async Task<Bill_Types> GetBillDetailsAsync(int id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiRequestUri.BaseUri);
                    var responeState = await client.GetAsync(_apiRequestUri.GetBillBetails);
                    if (responeState.IsSuccessStatusCode)
                    {
                        var taskCR = responeState.Content.ReadAsStringAsync();
                        var responseStringCR = taskCR.Result;
                        var BKResource = JsonConvert.DeserializeObject<Bill_Types>(responseStringCR);
                        return BKResource;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        public async Task<IEnumerable<Bill_Types>> GetBills()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiRequestUri.BaseUri);
                    var responeState = await client.GetAsync(_apiRequestUri.GetBills);
                    if (responeState.IsSuccessStatusCode)
                    {
                        var taskCR = responeState.Content.ReadAsStringAsync();
                        var responseStringCR = taskCR.Result;
                        var BKResource = JsonConvert.DeserializeObject<List<Bill_Types>>(responseStringCR);
                        return BKResource.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        public ApplicationUser GetUserDetails(string id)
        {
            throw new NotImplementedException();
        }
    }
}
