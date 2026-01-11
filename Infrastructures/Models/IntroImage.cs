using System;
using System.Collections.Generic;

namespace Infrastructures.Models;

public partial class IntroImage
{
    public int Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
