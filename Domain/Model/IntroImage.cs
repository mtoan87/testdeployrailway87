using System;
using System.Collections.Generic;

namespace Domain.Model;


public partial class IntroImage : BaseEntity
{
    public int Id { get; set; }

    public string ImageUrl { get; set; } = null!;

   
}
