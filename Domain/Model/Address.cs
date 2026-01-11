using System;
using System.Collections.Generic;

namespace Domain.Model;


public partial class Address : BaseEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? RecipientName { get; set; }

    public string? Phone { get; set; }

    public string? Province { get; set; }

    public string? District { get; set; }

    public string? Ward { get; set; }

    public string? Street { get; set; }

    public bool? IsDefault { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual User User { get; set; } = null!;
}
