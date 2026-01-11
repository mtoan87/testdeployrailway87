using System;
using System.Collections.Generic;

namespace Domain.Model;


public partial class BatchDetail : BaseEntity
{
    public int Id { get; set; }

    public int BatchId { get; set; }

    public int ProductId { get; set; }

    public string? SourceOfProductName { get; set; }

    public decimal? SellingPrice { get; set; }

    public decimal? ImportCosts { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public int? Quantity { get; set; }

  
    public bool? IsExpiredLogged { get; set; }

    public int? RemainingQuantity { get; set; }

    public int? DaysUntilExpiration { get; set; }

    public int? BatchDetailParentId { get; set; }

    public virtual Batch Batch { get; set; } = null!;

  

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual Product Product { get; set; } = null!;
}
