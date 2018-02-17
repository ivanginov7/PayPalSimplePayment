using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayPal.Models
{
    public class Order
    {
        public int Id { get; set; }
        public virtual IEnumerable<OrderItem> OrderItems { get; set; }
        public decimal TotalCost { get; set; }
    }
}