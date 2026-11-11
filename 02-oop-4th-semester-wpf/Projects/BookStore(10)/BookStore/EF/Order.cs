using System;
using System.Collections.Generic;

namespace BookStore.EF;

public partial class Order
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
