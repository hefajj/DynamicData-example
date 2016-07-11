using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicData;

using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using FXTrade.MarginService.BLL.Models;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class CurrencyPositionPerClientUpdaterService : BaseService, ICurrencyPositionPerClientUpdaterService
    {
        private ISourceCache<CurPositionPerClient, string> curPositionPerClientQuoteUpdate;
        private IObservableCache<CurPositionPerClient, string> curPositionPerClientCache;

        public CurrencyPositionPerClientUpdaterService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClientQuoteUpdate,
                           IObservableCache<CurPositionPerClient, string> curPositionPerClientCache
                           )
            : base(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient )
        {
            this.curPositionPerClientQuoteUpdate = curPositionPerClientQuoteUpdate;
            this.curPositionPerClientCache = curPositionPerClientCache;
        }


        /// <summary>
        ///   //UPDATE ALL CurPositionPerClient WITH NEW QUOTES - TODO
        /// </summary>
        public void UpdateAllCurrenciesPositions()
        {
            //var clientBalancesLocker = new object();

            cleanUp = curPositionPerClientCache.Connect(q => q.Cur != "EUR")
                     .Group(t => t.Cur)
                     //.Throttle(TimeSpan.FromMilliseconds(250))
                     .SubscribeMany(
                         groupedData =>
                         {

                             var locker = new object();
                             double latestAskPrice = 0;

                            //subscribe to price and recalculate CurPositionPerClient in account currenty
                            var priceHasChanged = quotes.Connect(q => (q.Pair == "EUR/" + groupedData.Key))
                                        .Synchronize(locker)
                                         //.Throttle(TimeSpan.FromMilliseconds(250))
                                         .Subscribe(
                                         price =>
                                         {
                                             
                                             foreach (var newquote in price)
                                             {
                                                 latestAskPrice = newquote.Current.Ask;

                                                 foreach (var item in groupedData.Cache.Items)
                                                 {
                                                     //LogInfo("SetAmountInBase before:|" + item);
                                                     var changed = item;

                                                     changed.SetAmountInBase(latestAskPrice);

                                                     curPositionPerClientQuoteUpdate.AddOrUpdate(changed);
                                                     //LogInfo("SetAmountInBase after:|" + item);
                                                }

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
                                         var changed = item.Current;

                                         changed.SetAmountInBase(latestAskPrice);

                                         curPositionPerClientQuoteUpdate.AddOrUpdate(changed);
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