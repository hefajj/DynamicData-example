using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXTrade.MarginService.BLL.Models
{
    public class CurPairPositionPerClient
    {
        public string ClientPair { get; set; }
        public long ClientId { get; set; }
        public string Pair { get; set; }
        public string Cur1 { get; set; }
        public string Cur2 { get; set; }
        public double OpenPrice { get; set; }
        public double Amount1 { get; set; }
        public double Amount2 { get; set; }
        //public double ProfitLoss;

        public override string ToString()
        {
            return string.Format("CurPairPositionPerClient:|Client|{0}|Pair|{1}|Amount1|{2}|Amount2|{3}",
                                  ClientId, Pair, Amount1, Amount2);
        }
    }
}
