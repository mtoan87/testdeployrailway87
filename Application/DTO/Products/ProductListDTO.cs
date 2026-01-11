using Application.DTO.BatchDetails;
using Application.DTO.Categories;
using Application.DTO.Images;
using Application.DTO.Records;
using Application.DTO.SourceOfProducts;
using Application.DTO.Users;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Products
{
    public class ProductListDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int? BoxId { get; set; }

        public int? PacksPerUnit { get; set; }

        public string Language { get; set; } = null!;

        public string? Description { get; set; }

        public string? Status { get; set; }

        public string? Cover { get; set; }
        public int? CategoryId { get; set; }
        //public string? Unit { get; set; }
        public bool IsDeleted { get; set; }
       // public int? CategoryId { get; set; }

       // public int? SourceOfProductId { get; set; }

        //public int? StockQuantity { get; set; }

       // public string? Status { get; set; }

        public DateTime? CreateDate { get; set; }
        public virtual CategoryProductDTO? Category { get; set; }
       // public virtual SourceDTO? SourceOfProduct { get; set; }
        public virtual ICollection<ImageDTO> Images { get; set; } = new List<ImageDTO>();
        public virtual ICollection<ProductLogListDTO> Logs { get; set; } = new List<ProductLogListDTO>();

        public virtual ICollection<BatchDetailProductDTO> BatchDetails { get; set; } = new List<BatchDetailProductDTO>();
    }
}
