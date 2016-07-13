using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXTrade.MarginService.BLL.Models
{
    public class BalancePerClient
    {
        public long ClientID { get; set; }
        public double SettledBalance { get; set; }
        public double InicialMargin { get; set; }
        public double ProfilLoss { get; set; }
        public double TotalMargin { get; set; } // InicialMargin - ProfilLoss
        public double BalanceLeft { get; set; } //SettledBalance - TotalMargin 

        public BalancePerClient()
        {
            this.ClientID = 0;
            this.SettledBalance = 0;
            this.InicialMargin = 0;
            this.ProfilLoss = 0;
            this.TotalMargin = 0;
            this.BalanceLeft = 0;
        }

        public BalancePerClient(long clientID, double settledBalance, double inicialMargin, double profilLossMargin)
        {
            this.ClientID = clientID;
            this.SettledBalance = settledBalance;
            this.InicialMargin = inicialMargin;
            this.ProfilLoss = ProfilLoss;
            this.TotalMargin = inicialMargin + profilLossMargin;
            this.BalanceLeft = settledBalance - TotalMargin;
        }

        public override string ToString()
        {
            return string.Format("BalancePerClient:|Client|{0}|SettledBalance|{1}|InicialMargin|{2}|ProfilLoss|{3}|TotalMargin|{4}|BalanceLeft|{5}",
                                  ClientID, SettledBalance, InicialMargin, ProfilLoss, TotalMargin, BalanceLeft);
        }
    }
}
