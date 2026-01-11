using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Products
{
    public class CreateChildProductDTO
    {
        //public int Id { get; set; }
        public int BatchDetailParentId { get; set; }
        public int BoxId { get; set; }

        public bool? isAddMorePacks { get; set; } 

        //public int ProductId { get; set; }
        //public string Name { get; set; } = null!;
        //public int? PacksPerUnit { get; set; }
        //public string Category { get; set; } = null!;
        //public string Language { get; set; } = null!;
        // public string? Description { get; set; }
        public decimal? SellingPrice { get; set; }

        //public decimal? ImportCosts { get; set; }
        //public decimal? OriginalPrice { get; set; }

        // public decimal? SellingPrice { get; set; }

        //public string? SourceOfProducts { get; set; }

        //public int CategoryId { get; set; }

        //public int SourceOfProductId { get; set; }
        //public string? UserName { get; set; }

        //public string? Phone { get; set; }

        //public string? Address { get; set; }
        //public decimal? ImportCosts { get; set; }

         public int Quantity { get; set; }
        //public string? Unit { get; set; }
      //  public string? Status { get; set; }

        //public List<string> ProductImages { get; set; } = new List<string>();
    }
}
