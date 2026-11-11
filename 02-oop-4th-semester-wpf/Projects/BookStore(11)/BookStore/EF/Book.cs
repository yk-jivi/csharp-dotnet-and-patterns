using System;
using System.Collections.Generic;

namespace BookStore.EF;

public partial class Book
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Author { get; set; }

    public decimal Price { get; set; }

    public int Year { get; set; }

    public int Pages { get; set; }

    public string? Description { get; set; }

    public byte[]? ImageData { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }
}
