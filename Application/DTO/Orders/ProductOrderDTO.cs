using Application.DTO.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Orders
{
    public class ProductOrderDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;


        public virtual CategoryProductDTO? CategoryDTO { get; set; }

      
    }
}
