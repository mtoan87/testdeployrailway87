using Application.DTO.BatchDetails;
using Application.DTO.Orders;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Carts
{
    public class CreateCartDTO
    {


       
        public int ProductId { get; set; }
        public int Quantity { get; set; }

    }
}
