
using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicData;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using FXTrade.MarginService.BLL.Models;

namespace FXTrade.MarginService.ServiceCore.Services
{
    /// <summary>
    /// UPDATE ALL TRADES WITH NEW QUOTES AND CALCULATE MTM on the trade
    /// </summary>
    public class UpdateTradesService : BaseService, IUpdateTradesService
    {
        private ISourceCache<Trade, long> myTradesQuoteUpdate;

        public UpdateTradesService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient,
                           ISourceCache<Trade, long> myTradesQuoteUpdate
                        )
            : base(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient)
        {
            this.myTradesQuoteUpdate = myTradesQuoteUpdate;
        }

        public void UpdateAllTradesAndQuotes()
        {
            var cleanUp = myTrades.Connect(trade => (trade.Status == Status.Open || trade.Status == Status.Pending))
                    .Group(t => t.Pair)
                    .SubscribeMany(
                            groupedData =>
                            {
                                var locker = new object();
                                double latestAskPrice = 0;
                                double latestBidPrice = 0;

                                //subscribe to price and update trades with the latest price
                                var priceHasChanged = quotes.Connect(q => q.Pair == groupedData.Key)
                                            .Synchronize(locker)
                                            .Subscribe(
                                            price =>
                                            {
                                                foreach (var newquote in price)
                                                {
                                                    latestAskPrice = newquote.Current.Ask;
                                                    latestBidPrice = newquote.Current.Bid;

                                                    //myTrades.Edit(updater =>
                                                    //    {
                                                    foreach (var item in groupedData.Cache.Items)
                                                    {
                                                        var trade = item;
                                                        if (trade.Amount1 > 0)
                                                            trade.CurrentPrice = latestBidPrice;
                                                        else
                                                            trade.CurrentPrice = latestAskPrice;

                                                        var Profitloss_in_quoted = (trade.OpenPrice - trade.CurrentPrice) * trade.Amount1; // Update P&L on the trade
                                                        trade.ProfitLoss = ConvertToBaseCcy(Profitloss_in_quoted, trade.Cur2);

                                                        myTradesQuoteUpdate.AddOrUpdate(trade);

                                                        //_marketPriceChangedSubject.OnNext(marketPrice);
                                                        //updater.AddOrUpdate(trade);
                                                    }
                                                    //    }
                                                    //);
                                                }
                                            }
                                        );

                                //connect to data changes and update with the latest price
                                var dataHasChanged = groupedData.Cache.Connect()
                                    //.WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
                                    .Synchronize(locker)
                                    .Subscribe(changes =>
                                    {

                                        foreach (var item in changes)
                                        {
                                            var trade = item.Current;
                                            if (trade.Amount1 > 0)
                                                trade.CurrentPrice = latestBidPrice;
                                            else
                                                trade.CurrentPrice = latestAskPrice;

                                            var Profitloss_in_quoted = (trade.OpenPrice - trade.CurrentPrice) * trade.Amount1; // Update P&L on the trade
                                            trade.ProfitLoss = ConvertToBaseCcy(Profitloss_in_quoted, trade.Cur2);

                                            myTradesQuoteUpdate.AddOrUpdate(trade);
                                        }

                                        
                                    }
                                    );


                                return new CompositeDisposable(priceHasChanged);
                            
                            }
                        )
                    .Subscribe();
        }

        private void UpdateTradesWithPrice(IEnumerable<Trade> trades, decimal price)
        {
            //trades.ForEach(t => t.SetMarketPrice(price));
        }

    }
}