using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace Domain.Model;

public partial class Image : BaseEntity
{
    public int Id { get; set; }

    public string? UrlPath { get; set; }

    public int? ProductId { get; set; }
    public int? UserId { get; set; }
    public int? CategoryId { get; set; }   
    public int? NewsId { get; set; }
    [JsonIgnore]
    public virtual News? News { get; set; }

    public virtual Product? Product { get; set; }
    public virtual Category? Category { get; set; }
    public virtual User? User { get; set; }
}
