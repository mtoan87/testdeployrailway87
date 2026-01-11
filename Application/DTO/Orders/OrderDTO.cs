using Application.DTO.Users;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Orders
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public decimal? OrderAmount { get; set; }

        public DateTime? OrderDate { get; set; } 
        public string? OrderStatus { get; set; }

        public virtual ICollection<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();

        public virtual UserOrderDetailDTO? User { get; set; }
    }
}
