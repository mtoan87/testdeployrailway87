using System;
using System.Collections.Generic;

namespace Domain.Model;


public partial class Cart : BaseEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
