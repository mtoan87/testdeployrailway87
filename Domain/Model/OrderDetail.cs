using Domain.Model;
using System;
using System.Collections.Generic;

namespace Domain.Model; 

public partial class OrderDetail : BaseEntity
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public decimal? TotalPrice { get; set; }
    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }
    public string? Name { get; set; }   

    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
