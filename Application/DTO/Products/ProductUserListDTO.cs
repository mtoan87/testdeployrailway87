using Application.DTO.BatchDetails;
using Application.DTO.Categories;
using Application.DTO.Images;
using Application.DTO.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Products
{
    public class ProductUserListDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int? BoxId { get; set; }

        public int? PacksPerUnit { get; set; }

        public string Language { get; set; } = null!;

        public string? Description { get; set; }

        public string? Status { get; set; }

        public string? Cover { get; set; }
        
        public virtual CategoryProductDTO? Category { get; set; }
      
        public virtual ICollection<ImageDTO> Images { get; set; } = new List<ImageDTO>();
      

        public virtual ICollection<BatchDetailUserProductDTO> BatchDetails { get; set; } = new List<BatchDetailUserProductDTO>();
    }
}
