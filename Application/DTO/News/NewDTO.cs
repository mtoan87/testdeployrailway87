using Application.DTO.Categories;
using Application.DTO.Images;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.News
{
    public class NewDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Writer { get; set; }

        public string? Cover { get; set; }
        public bool IsDeleted { get; set; }
        //public int? CategoryId { get; set; }
        
        public virtual ICollection<ImageDTO> Images { get; set; } = new List<ImageDTO>();

        public virtual CategoryDTO? Category { get; set; }
    }
}
