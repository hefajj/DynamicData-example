
using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicData;
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
        public UpdateTradesService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient)
            : base(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient)
        {

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

                                                    myTrades.Edit(updater =>
                                                        {
                                                            foreach (var item in groupedData.Cache.Items)
                                                            {
                                                                var trade = item;
                                                                if (trade.Amount1 > 0)
                                                                    trade.CurrentPrice = latestBidPrice;
                                                                else
                                                                    trade.CurrentPrice = latestAskPrice;

                                                                var Profitloss_in_quoted = (trade.OpenPrice - trade.CurrentPrice) * trade.Amount1; // Update P&L on the trade
                                                                trade.ProfitLoss = ConvertToBaseCcy(Profitloss_in_quoted, trade.Cur2);
                                                                
                                                                updater.AddOrUpdate(trade);
                                                            }
                                                        }
                                                    );
                                                }
                                            }
                                        );
                                return new CompositeDisposable(priceHasChanged);
                            
                            }
                        )
                    .Subscribe();
        }        
    }
}