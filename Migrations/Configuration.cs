namespace PayPal.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<PayPal.Models.PayPalContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PayPal.Models.PayPalContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var p1 = new Product { Name = "book", Price = 2.99m };
            var p2 = new Product { Name = "album", Price = 3.39m };
            if(!context.Products.Any())
            {
                context.Products.Add(p1);
                context.Products.Add(p2);
                context.SaveChanges();
            }
            
        }
    }
}
