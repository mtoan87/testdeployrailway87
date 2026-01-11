using Application.DTO.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Categories
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CateType { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<ImageDTO> Images { get; set; } = new List<ImageDTO>();
    }
}
