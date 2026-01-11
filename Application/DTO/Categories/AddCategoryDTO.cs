using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Categories
{
    public class AddCategoryDTO
    {
        public string? Name { get; set; }
        public string? CateType { get; set; }

        public List<string>? CateImages { get; set; } = new List<string>();
    }
}
