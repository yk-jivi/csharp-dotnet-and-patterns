using System;
using System.Collections.Generic;

namespace BookStore
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public List<CartItem>? Items { get; set; }
        public decimal TotalAmount { get; set; }
    }
}