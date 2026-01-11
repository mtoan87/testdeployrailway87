using Domain.Model;

using System;
using System.Collections.Generic;
using System.Net;

namespace Domain.Model;

public partial class User : BaseEntity
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Phone { get; set; }

   
    public int? RoleId { get; set; }

    public string? Status { get; set; }

   
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<OrderStatus> OrderStatuses { get; set; } = new List<OrderStatus>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role? Role { get; set; }
}
