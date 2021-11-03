using _Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Data
{
    public static class DataInitializer
    {
        #region SeedData
        public static void SeedData(/*IServiceProvider serviceProvider*/ApplicationDbContext dbContext)
        {
            //using (var context = new ApplicationDbContext (serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            //{

            
            //}
            dbContext.Database.EnsureCreated();

            if (dbContext.Bill_Types.Any())
            {
                return;
            }
            #endregion


            #region Bill Types
            var bills = new Bill_Types[]
            {
                new Bill_Types {Bill_Name = "GOTV", Amount = "#3500"}, 
                new Bill_Types {Bill_Name = "DSTV", Amount = "#6000"}, 
                new Bill_Types {Bill_Name = "EKECD", Amount = "#10000"}, 
                new Bill_Types {Bill_Name = "AIR TIME", Amount = "#4000"}
            };

            dbContext.Bill_Types.AddRange(bills);
            dbContext.SaveChanges();

            #endregion


        }
    }
}
