using System;
using System.Collections.Generic;

namespace Infrastructures.Models;

public partial class Log
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? UserId { get; set; }

    public int? Quantity { get; set; }

    public string? Type { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int? OrderId { get; set; }

    public decimal? OldSellingPrice { get; set; }

    public decimal? NewSellingPrice { get; set; }

    public decimal? OldImportCost { get; set; }

    public decimal? NewImportCost { get; set; }

    public int? BatchDetailId { get; set; }

    public int? BatchId { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public string? Note { get; set; }

    public virtual Batch? Batch { get; set; }

    public virtual BatchDetail? BatchDetail { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
