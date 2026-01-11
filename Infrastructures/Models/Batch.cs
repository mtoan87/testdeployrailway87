using System;
using System.Collections.Generic;

namespace Infrastructures.Models;

public partial class Batch
{
    public int Id { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<BatchDetail> BatchDetails { get; set; } = new List<BatchDetail>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
}
