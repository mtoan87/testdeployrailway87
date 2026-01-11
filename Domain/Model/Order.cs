using Domain.Model;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Domain.Model;

public partial class Order : BaseEntity
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public decimal? OrderAmount { get; set; }
    public string? Note { get; set; }
    public DateTime? OrderDate { get; set; }
    public int? AddressId { get; set; }

    public virtual Address? Address { get; set; }
    public string? OrderStatus { get; set; }
    public string? PaymentMethod { get; set; }
    [JsonIgnore]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
    public virtual ICollection<OrderStatus> OrderStatuses { get; set; } = new List<OrderStatus>();
    public virtual User? User { get; set; }
}
