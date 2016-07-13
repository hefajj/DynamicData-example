using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicData;
using FXTrade.MarginService.BLL.Models;
using System.Reactive;
using FXTrade.MarginService.ServiceCore.SubscriberCommunication;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class LogPrinterService : BaseService, ILogPrinterService
    {
        private ISourceCache<Trade, long> myTrades;
        private ISourceCache<BalancePerClient, long> clientBalances;
        private ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient;
        private ISourceCache<CurPositionPerClient, string> curPositionPerClient;
        private IObservableCache<CurPositionPerClient, string> curPositionPerClientCache;
        private ISourceCache<Trade, long> myTradesQuoteUpdate;
        private ISourceCache<CurPositionPerClient, string> curPositionPerClientQuoteUpdate;


        public LogPrinterService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient,
                           IObservableCache<CurPositionPerClient, string> curPositionPerClientCache,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClientQuoteUpdate,
                           ISourceCache<Trade, long> myTradesQuoteUpdate,
        ISubscriberCommunicator communicator = null)
            : base(communicator)
        {
            this.myTrades = myTrades;
            this.clientBalances = clientBalances;
            this.curPairPositionPerClient = curPairPositionPerClient;
            this.curPositionPerClient = curPositionPerClient;
            this.curPositionPerClientCache = curPositionPerClientCache;
            this.myTradesQuoteUpdate = myTradesQuoteUpdate;
            this.curPositionPerClientQuoteUpdate = curPositionPerClientQuoteUpdate;
        }


        /// <summary>
        ///  print changed curPositionPerClientQuoteUpdate
        /// </summary>
        public void PrintcurPositionPerClientQuoteUpdate()
        {
            curPositionPerClientQuoteUpdate.Connect()
                   .Subscribe(
                           c =>
                           {
                               foreach (var item in c)
                               {
                                   LogInfo("curPositionPerClientQuoteUpdate:|" + item.Reason.ToString() + " | " + item.Current.ToString());
                               }
                           }
                   );
        }


        /// <summary>
        ///  print changed CurPairPositionPerClient
        /// </summary>
        public void PrintcurPairPositionPerClient()
        {
            curPairPositionPerClient.Connect()
                   .Subscribe(
                           c =>
                           {
                               foreach (var item in c)
                               {
                                   LogInfo(item.Reason.ToString() + "|" + item.Current.ToString());
                               }
                           }
                   );
        }



        /// <summary>
        ///  print changed curPositionPerClientCache
        /// </summary>
        public void PrintcurPositionPerClientCache()
        {
            curPositionPerClientCache.Connect()
                   .Subscribe(
                           c =>
                           {
                               foreach (var item in c)
                               {
                                   LogInfo("curPositionPerClientCache:|" + item.Reason.ToString() + " | " + item.Current.ToString());
                               }
                           }
                   );
        }


        /// <summary>
        ///  print changed myTrades
        /// </summary>
        public void PrintmyTrades()
        {
            myTrades.Connect()
                   .Subscribe(
                           c =>
                           {
                               foreach (var item in c)
                               {
                                   LogInfo(item.Reason.ToString() + "|" + item.Current.ToString());
                               }
                           }
                   );
        }

        /// <summary>
        ///  print changed myTrades
        /// </summary>
        public void PrintmyTradesQuoteUpdate()
        {
            myTradesQuoteUpdate.Connect()
                   .Subscribe(
                           c =>
                           {
                               foreach (var item in c)
                               {
                                   LogInfo("PrintmyTradesQuoteUpdate:|" + item.Reason.ToString() + " | " + item.Current.ToString());
                               }
                           }
                   );
        }

        /// <summary>
        ///  print changed clientbalances
        /// </summary>
        public void PrintClientBalances()
        {
            clientBalances.Connect()
                   .Subscribe(
                           c =>
                           {
                               foreach (var item in c)
                               {
                                   LogInfo("PrintclientBalances:|" + item.Reason.ToString() + " | " + item.Current.ToString());
                               }
                           }
                   );
        }


    }
}