using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Batch : BaseEntity
{
    //public int Id { get; set; }



    public virtual ICollection<BatchDetail> BatchDetails { get; set; } = new List<BatchDetail>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
}
