using Application.DTO.BatchDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Batches
{
    public class BatchDetailGroupDTO
    {
        public BatchDetailDTO BatchdetailParent { get; set; } = null!;
        public List<BatchDetailDTO> BatchdetailChild { get; set; } = new();
    }
}
