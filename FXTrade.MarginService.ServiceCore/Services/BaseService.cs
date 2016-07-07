using DynamicData;
using FXTrade.MarginService.BLL.Models;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public abstract class BaseService : IDisposable
    {
        
        private log4net.ILog logger;
        private static object myTrades_Lock = new object();
        
        private static object curPairPositionPerClient_Lock = new object();
        private static long trade_counter = 0;

        protected IDisposable cleanUp;

        #region Observed Collections
        protected ISourceCache<Quote, string> quotes;
        protected ISourceCache<Trade, long> myTrades;
        protected ISourceCache<BalancePerClient, long> clientBalances;
        protected ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient;
        protected ISourceCache<CurPositionPerClient, string> curPositionPerClient;
        #endregion
       // protected object curPositionPerClient_Lock;


        public BaseService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient
            //               object CurPositionPerClient_Lock
            )
        {
            this.myTrades = myTrades;
            this.quotes = quotes;
            this.clientBalances = clientBalances;
            this.curPairPositionPerClient = curPairPositionPerClient;
            this.curPositionPerClient = curPositionPerClient;
            //this.curPositionPerClient_Lock = CurPositionPerClient_Lock;
            this.logger = LogManager.GetLogger("MarginTrader");
        }
        public void Dispose()
        {
            cleanUp?.Dispose();
        }
        public void AppendLog(string txt)
        {

            logger.Info(txt + "\r");
        }


        #region HelperMethods
        protected void AddMyTrade(Trade mynewtrade)
        {
            lock (myTrades_Lock)
            {
                trade_counter++;
                mynewtrade.Id = trade_counter;
                myTrades.AddOrUpdate(mynewtrade);
            }
        }

        //protected void AddOrUpdate_curPositionPerClient(IEnumerable<CurPositionPerClient> newvalue)
        //{
        //    lock (CurPositionPerClient_Lock)
        //    {
        //        curPositionPerClient.AddOrUpdate(newvalue);
        //    }
        //}

        //protected void AddOrUpdate_curPositionPerClient(CurPositionPerClient newvalue)
        //{
        //    lock (CurPositionPerClient_Lock)
        //    {
        //        curPositionPerClient.AddOrUpdate(newvalue);
        //    }
        //}

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