using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public int? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}