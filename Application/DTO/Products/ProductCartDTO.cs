using Application.DTO.BatchDetails;
using Application.DTO.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Products
{
    public class ProductCartDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public virtual ICollection<CartBatchDetailDTO> BatchDetails { get; set; } = new List<CartBatchDetailDTO>();
        public virtual ICollection<ImageDTO> Images { get; set; } = new List<ImageDTO>();
    }
}
