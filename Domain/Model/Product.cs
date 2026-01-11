using Domain.Model;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Product : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? BoxId { get; set; }

    public int? PacksPerUnit { get; set; }

    public string Language { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public string? Cover { get; set; }

    public int? CategoryId { get; set; }

    public virtual ICollection<BatchDetail> BatchDetails { get; set; } = new List<BatchDetail>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
