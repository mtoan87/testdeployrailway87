using Application.DTO.BatchDetails;
using Application.DTO.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Batches
{
    public class CreateBatchDTO
    {
        public List<CreateBatchDetailDTO> BatchDetailsDTO { get; set; }
    }
}
