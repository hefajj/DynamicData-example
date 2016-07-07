using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXTrade.MarginService.BLL.Models
{
    public class Trade
    {
        public long Id { get; set; }
        public string Pair { get; set; }
        public string Cur1 { get; set; }
        public string Cur2 { get; set; }
        public long ClientId { get; set; }
        public double OpenPrice { get; set; }
        public double CurrentPrice { get; set; }
        public double Amount1 { get; set; }
        public double Amount2 { get; set; }
        public double ProfitLoss { get; set; }
        public string Status { get; set; }

        public override string ToString()
        {
            return string.Format("Trade:|Id|{0}|Client|{1}|Pair|{4}|Amount1|{5}|Amount2|{8}|Status|{7}|Open|{2}|Current|{3}|P&L|{6}|Cur1|{9}|Cur2|{10}",
                                   Id, ClientId, OpenPrice, CurrentPrice, Pair, Amount1, ProfitLoss, Status, Amount2, Cur1, Cur2);
        }
    }
}
