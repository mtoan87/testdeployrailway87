using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Category : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string? CateType { get; set; }
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<News> News { get; set; } = new List<News>();
}
