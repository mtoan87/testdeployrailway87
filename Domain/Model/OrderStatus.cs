using System;
using System.Collections.Generic;
namespace Domain.Model;


public partial class OrderStatus : BaseEntity
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int AccountId { get; set; }

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    

    public virtual User Account { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
