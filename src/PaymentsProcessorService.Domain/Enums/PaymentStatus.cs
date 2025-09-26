using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Domain.Enums
{
    public enum PaymentStatus
    {
        Processing = 1,
        Processed = 2,
        Failed = 3
    }
}
