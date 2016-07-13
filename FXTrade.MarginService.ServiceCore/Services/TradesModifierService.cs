using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FXTrade.MarginService.BLL.Models;
using DynamicData;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class TradesModifierService : BaseService, ITradesModifierService
    {
        private ISourceCache<Trade, long> myTrades;

        #region Static Fields
        private static object myTradesLock = new object();
        private static long tradeCounter = 0;
        #endregion

        public TradesModifierService(ISourceCache<Trade, long> myTrades) : base()
        {
            this.myTrades = myTrades;
        }

        public void AddMyTrade(Trade myNewTrade)
        {
            lock (myTradesLock)
            {
                tradeCounter++;
                myNewTrade.Id = tradeCounter;
                myTrades.AddOrUpdate(myNewTrade);
            }
        }
    }
}
