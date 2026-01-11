using System;
using System.Collections.Generic;

namespace Infrastructures.Models;

public partial class Image
{
    public int Id { get; set; }

    public string? UrlPath { get; set; }

    public int? ProductId { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public int? NewsId { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual News? News { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
