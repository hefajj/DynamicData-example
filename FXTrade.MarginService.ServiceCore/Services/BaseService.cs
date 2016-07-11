using DynamicData;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ServiceCore.SubscriberCommunication;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public abstract class BaseService : IDisposable
    {

        private log4net.ILog logger;
        protected ISubscriberCommunicator communicator;
        protected IDisposable cleanUp;

        #region Observed Collections
        protected ISourceCache<Quote, string> quotes;
        protected ISourceCache<Trade, long> myTrades;
        protected ISourceCache<BalancePerClient, long> clientBalances;
        protected ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient;
        protected ISourceCache<CurPositionPerClient, string> curPositionPerClient;
        #endregion

        #region Static Fields
        private static object myTradesLock = new object();
        private static long tradeCounter = 0;
        #endregion




        public BaseService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient,
                           ISubscriberCommunicator communicator = null
            )
        {
            this.myTrades = myTrades;
            this.quotes = quotes;
            this.clientBalances = clientBalances;
            this.curPairPositionPerClient = curPairPositionPerClient;
            this.curPositionPerClient = curPositionPerClient;
            this.communicator = communicator;
            this.logger = LogManager.GetLogger("MarginTrader");
        }
        public void Dispose()
        {
            cleanUp?.Dispose();
        }
        public void LogInfo(string txt)
        {
            logger.Info(txt + "\r");
        }

        public void LogError(string txt)
        {
            logger.Info(txt + "\r");
        }

        #region HelperMethods
        protected void AddMyTrade(Trade myNewTrade)
        {
            //lock (myTradesLock)
            //{
                tradeCounter++;
                myNewTrade.Id = tradeCounter;
                myTrades.AddOrUpdate(myNewTrade);
            //}
        }


        protected double ConvertToBaseCcy(double amount, string quoteDccy)
        {
            string baseccy = "EUR";
            if (quoteDccy == baseccy)
            {
                return amount;
            }
            var quote = quotes.Items.Where(a => a.Pair == (baseccy + "/" + quoteDccy)).First();
            double converted = amount * quote.Ask;
            return converted;
        }

        #endregion
    }
}