using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXTrade.MarginService.BLL.Models
{
    public class Quote
    {
        public int Id { get; set; }
        public string Pair { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public string Cur1 { get; set; }
        public string Cur2 { get; set; }

        //private static int InstanciesCount = 0;

        //public Quote()
        //{
        //    InstanciesCount++;
        //    Id = InstanciesCount;
        //}

        public override string ToString()
        {
            return string.Format("Quote:|Pair|{0}|Bid|{1}|Ask|{2}",
                                   Pair, Bid, Ask);
        }

    }
}
