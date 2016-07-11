using DynamicData;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ServiceCore.Contract;
using FXTrade.MarginService.ServiceCore.SubscriberCommunication;
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
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient, ISubscriberCommunicator communicator)
            :base(myTrades,quotes, clientBalances, curPairPositionPerClient, curPositionPerClient, communicator)
        {
           
        }      

        public void SubscribeMyTrades()
        {
            cleanUp = myTrades.Connect()
                    .Bind(out myTradesReadOnlyCollection)
                    .DisposeMany()
                    .Subscribe(trades =>
                    {
                        foreach (var trade in trades.ToList())
                        {
                            if (trade.Reason == ChangeReason.Add)
                                communicator.PushTradeCreateToSubscriber(trade.Current);
                            else if (trade.Reason == ChangeReason.Remove)
                                communicator.PushTradeRemoveToSubscriber(trade.Current);
                            else if (trade.Reason == ChangeReason.Update)
                                communicator.PushTradeUpdateToSubscriber(trade.Current);
                        }
                    });
        }
    }
}