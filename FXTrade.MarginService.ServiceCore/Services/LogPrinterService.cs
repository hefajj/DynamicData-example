using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicData;
using FXTrade.MarginService.BLL.Models;
using System.Reactive;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class LogPrinterService : BaseService, ILogPrinterService
    {
        public LogPrinterService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient)
            : base(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient)
        {

        }


        /// <summary>
        ///  print changed balances
        /// </summary>
        public void PrintClientBalances()
        {
            clientBalances.Connect()
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
        ///  print changed CurPositionPerClient
        /// </summary>
        public void PrintCurPositionPerClient()
        {
            curPositionPerClient.Connect()
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



    }
}