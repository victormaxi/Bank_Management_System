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
            public DbSet<Cheque> Cheques { get; set; }

        public DbSet<Bill_Types> Bill_Types { get; set; }
           // public DbSet<ImageStore> ImageStores { get; set; }
        public DbSet<Picture> Pictures { get; set; }

        public DbSet<ImageFile> ImageFiles { get; set; }

        public DbSet<PaymentLogs> PaymentLogs { get; set; }

        // public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                
                .HasOne(a => a.ImageFile)
                .WithOne(b => b.ApplicationUser)
                
                .HasForeignKey<ImageFile>(b => b.Id)
                
                ;
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(a => a.Transactions)
                .WithOne(b => b.ApplicationUser);
            //.HasForeignKey<Transactions>(c => c.UserId);

            modelBuilder.Entity<Transactions>()

                ;
        }

    }
}
