using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.News
{
    public class UpdateNewDTO
    {
       

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Writer { get; set; }

        public string? Cover { get; set; }

        public int? CategoryId { get; set; }


        public List<string>? NewsImages { get; set; } = new List<string>();

    }
}
