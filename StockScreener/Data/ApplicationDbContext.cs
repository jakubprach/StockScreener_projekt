using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockScreener.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockScreener.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<StockPurchase> StockPurchase { get; set; }
    }
}
