using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PayPal.Models
{
    public class PayPalContext : DbContext
    {
        public PayPalContext() : base("DefaultConnection"){}


        public DbSet<Product> Products {get;set;}
        public DbSet<Order> Orders { get; set; }
        //public DbSet<OrderItem> OrderItems { get; set; }
    }
}