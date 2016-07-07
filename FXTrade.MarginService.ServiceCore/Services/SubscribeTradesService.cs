using DynamicData;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ServiceCore.Contract;
//using FXTrade.Web.SignalR.Hubs;
//using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Web;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class SubscribeTradesService : BaseService, ISubscribeTradesService
    {
        private ReadOnlyObservableCollection<Trade> myTradesReadOnlyCollection;
        public ReadOnlyObservableCollection<Trade> MyTradesReadOnlyCollection { get { return myTradesReadOnlyCollection; } }
               

        public SubscribeTradesService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient)
            :base(myTrades,quotes, clientBalances, curPairPositionPerClient, curPositionPerClient)
        {
           
        }      

        public void SubscribeMyTrades()
        {
            var cleanUp = myTrades.Connect()
                    .Bind(out myTradesReadOnlyCollection)
                    .Subscribe(trades =>
                    {
                        // inform client..... 
                        //var hubContext = GlobalHost.ConnectionManager.GetHubContext<MarginHub>();
                        //foreach (var trade in trades.ToList())
                        //{
                        //    if (trade.Reason == ChangeReason.Add)
                        //        hubContext.Clients.All.createTrade(trade.Current);
                        //    else if (trade.Reason == ChangeReason.Remove)
                        //        hubContext.Clients.All.destroyTrade(trade.Current);
                        //    else if (trade.Reason == ChangeReason.Update)
                        //        hubContext.Clients.All.updateTrade(trade.Current);
                        //}
                    });
        }
    }
}