using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Orders
{
    public class OrderNoLogDTO
    {
        public int Id { get; set; }

        //public int? UserId { get; set; }
        public string? PaymentMethod { get; set; }

        public decimal? OrderAmount { get; set; }

        public DateTime? OrderDate { get; set; }

        public string? OrderStatus { get; set; }
        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Message { get; set; }
        public virtual ICollection<OrderDetailss> OrderDetails { get; set; } = new List<OrderDetailss>();
    }
}
