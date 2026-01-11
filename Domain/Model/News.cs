using System;
using System.Collections.Generic;

namespace Domain.Model;


public partial class News : BaseEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Writer { get; set; }

    public string? Cover { get; set; }

    public int? CategoryId { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual Category? Category { get; set; }
}
