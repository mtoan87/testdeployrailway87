using Application.DTO.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Orders
{
    public class CheckOutDTO
    {
      

        public decimal? OrderAmount { get; set; }

        public DateTime? OrderDate { get; set; }

        public string? PaymentMethod { get; set; }

        public virtual ICollection<CheckOutOrderDetailDTO> OrderDetails { get; set; } = new List<CheckOutOrderDetailDTO>();

      
    }
}
