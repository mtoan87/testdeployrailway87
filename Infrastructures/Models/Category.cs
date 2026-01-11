using System;
using System.Collections.Generic;

namespace Infrastructures.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CateType { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
