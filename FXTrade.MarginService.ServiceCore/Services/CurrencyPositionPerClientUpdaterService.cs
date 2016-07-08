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
        public CurrencyPositionPerClientUpdaterService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient)
            : base(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient)
        {

        }


        /// <summary>
        ///   //UPDATE ALL CurPositionPerClient WITH NEW QUOTES - TODO
        /// </summary>
        public void UpdateAllCurrenciesPositions()
        {
           var clientBalancesLocker = new object();

           cleanUp =  curPositionPerClient.Connect(q => q.Cur != "EUR")
                    .Group(t => t.Cur)
                    //.Throttle(TimeSpan.FromMilliseconds(250))
                    .SubscribeMany(
                        groupedData =>
                        {

                            var locker = new object();
                            double latestAskPrice = 0;

                            //subscribe to price and recalculate CurPositionPerClient in account currenty
                            //var priceHasChanged = quotes.Connect(q => q.Cur2 == groupedData.Key)
                            var priceHasChanged = quotes.Connect(q => (q.Pair == "EUR/" + groupedData.Key))
                                       .Synchronize(clientBalancesLocker)
                                        //.Throttle(TimeSpan.FromMilliseconds(250))
                                        .Subscribe(
                                        price =>
                                        {
                                            foreach (var newquote in price)
                                            {
                                                latestAskPrice = newquote.Current.Ask;

                                                foreach (var item in groupedData.Cache.Items)
                                                {
                                                    LogInfo("SetAmountInBase before:|" + item);
                                                    //TODO: Update Amount in Base position someplace else to not update main ISourceList

                                                    //item.SetAmountInBase(latestAskPrice);
                                                    LogInfo("SetAmountInBase after:|" + item);

                                                    //item.AmountInBase = item.Amount * latestAskPrice;
                                                    //AddOrUpdate_curPositionPerClient(item);
                                                }

                                                //curPositionPerClient.Edit(updater =>
                                                //{

                                                //    foreach (var item in groupedData.Cache.Items)
                                                //    {
                                                //        item.AmountInBase = item.Amount * latestAskPrice;
                                                //        //updater.AddOrUpdate(item);
                                                //    }
                                                //});

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