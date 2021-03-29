using _Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace _Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
            {

            }
            public DbSet<Loan> Loans { get; set; }
            public DbSet<Bank> Banks { get; set; }
            public DbSet<Transactions> Transactions { get; set; }


        // public DbSet<Role> Roles { get; set; }

    }
}
