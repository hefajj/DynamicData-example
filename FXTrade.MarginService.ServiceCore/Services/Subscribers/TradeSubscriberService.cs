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
    public class TradeSubscriberService : BaseService, ITradeSubscriberService
    {
        private ISourceCache<Trade, long> myTrades;

        private ReadOnlyObservableCollection<Trade> myTradesReadOnlyCollection;
        public ReadOnlyObservableCollection<Trade> MyTradesReadOnlyCollection { get { return myTradesReadOnlyCollection; } }


        public TradeSubscriberService(ISourceCache<Trade, long> myTrades,
                                      ISubscriberCommunicator communicator = null)
            : base(communicator)
        {
            this.myTrades = myTrades;
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
                                communicator.PushTradeCreate(trade.Current);
                            else if (trade.Reason == ChangeReason.Remove)
                                communicator.PushTradeRemove(trade.Current);
                            else if (trade.Reason == ChangeReason.Update)
                                communicator.PushTradeUpdate(trade.Current);
                        }
                    });
        }
    }
}