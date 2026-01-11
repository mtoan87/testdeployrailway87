using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum ProductStatus
    {
        Available,
        UnAvailable,
        OutOfStock,
        Incoming,
        Discontinued,
        PendingApproval,
        Damaged,
        Expired
    }
}
