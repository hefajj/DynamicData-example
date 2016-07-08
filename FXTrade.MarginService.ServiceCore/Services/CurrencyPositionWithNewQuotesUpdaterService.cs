using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicData;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using FXTrade.MarginService.BLL.Models;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class CurrencyPositionWithNewQuotesUpdaterService : BaseService, ICurrencyPositionWithNewQuotesUpdaterService
    {
        public CurrencyPositionWithNewQuotesUpdaterService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient)
            : base(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient)
        {

        }


        /// <summary>
        /// UPDATE ALL CurPositionPerClient WITH NEW QUOTES 
        /// </summary>
        public void UpdateCurrencyPositionWithNewQuotes()
        {
            var clientBalancesLocker = new object();
            cleanUp = curPositionPerClient.Connect(q => q.Cur != "EUR")
                    .Group(t => t.Cur)
                    //.Throttle(TimeSpan.FromMilliseconds(250))
                    .SubscribeMany(
                        groupedData =>
                        {

                            var locker = new object();

                            double latestAskPrice = 0;

                            //subscribe to price and recalculate CurPositionPerClient in account currenty
                            var priceHasChanged = quotes.Connect(q => q.Cur2 == groupedData.Key)
                                        .Synchronize(clientBalancesLocker)
                                        //.Throttle(TimeSpan.FromMilliseconds(250))
                                        .Subscribe(
                                        price =>
                                        {
                                            foreach (var newquote in price)
                                            {
                                                latestAskPrice = newquote.Current.Ask;

                                                curPositionPerClient
                                                    .Edit(updater =>
                                                    {
                                                        foreach (var item in groupedData.Cache.Items)
                                                        {
                                                            var trade = item;
                                                            trade.AmountInBase = trade.Amount * latestAskPrice;
                                                            LogInfo("CurPositionPerClient.BatchUpdate: " + trade);

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