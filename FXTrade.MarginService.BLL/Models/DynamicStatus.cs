using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXTrade.MarginService.BLL.Models
{
    public struct Status
    {
        public const string Pending = "Pending";
        public const string Open = "Open";
        public const string Close = "Closed";
        public const string Rejected = "Rejected";
        public const string Settled = "Settled";
    }
}
