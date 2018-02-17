using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PayPal.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal ProductPrice { get; set; }
        public Product Product { get; set; }
        public int Count { get; set; }
        public decimal Cost
        {
            get
            {
                return (decimal)Math.Round(this.ProductPrice * this.Count,2);
            }
            set
            {
                this.Cost = value;
            }
        }
    }
}