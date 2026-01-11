using System;
using System.Collections.Generic;

namespace Infrastructures.Models;

public partial class Order
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public decimal? OrderAmount { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? OrderStatus { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public string? PaymentMethod { get; set; }

    public int? AddressId { get; set; }

    public string? Note { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderStatus> OrderStatuses { get; set; } = new List<OrderStatus>();

    public virtual User? User { get; set; }
}
