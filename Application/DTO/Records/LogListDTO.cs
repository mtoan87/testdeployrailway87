using Application.DTO.BatchDetails;
using Application.DTO.Orders;
using Application.DTO.Users;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Records
{
    public class LogListDTO
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? Quantity { get; set; }
        public int? BatchDetailId { get; set; }

        public int? BatchId { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public string? Note { get; set; }

        public DateTime? CreateDate { get; set; }
        public string? Type { get; set; }
      
        public virtual ProductOrderDTO? Product { get; set; }

        public virtual BatchDetailNProductDTO? BatchDetail { get; set; }
    }
}
