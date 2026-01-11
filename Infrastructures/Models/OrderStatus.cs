using System;
using System.Collections.Generic;

namespace Infrastructures.Models;

public partial class OrderStatus
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int? AccountId { get; set; }

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual User? Account { get; set; }

    public virtual Order Order { get; set; } = null!;
}
