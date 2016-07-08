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
    public class PositionPerCurrencyPairCalculatorService : BaseService, IPositionPerCurrencyPairCalculatorService
    {
        public PositionPerCurrencyPairCalculatorService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient                           
                    )
            : base(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient)
        {

        }
        /// <summary>
        ///  Caluclate position per currency pair per customer
        /// </summary>
        public void CalculatePosistionPerCurrencyPairPerCustomer()
        {
            var myTradeLocker = new object();
            var clientBalancesLocker = new object();
            cleanUp = myTrades.Connect(trade => (trade.Status == Status.Open || trade.Status == Status.Pending))
                     .WhereReasonsAre(ChangeReason.Add, ChangeReason.Remove)
                     .Synchronize(myTradeLocker)
                     .Group(t => t.Pair)
                     .SubscribeMany(groupedData =>
                     {
                         var PositionPerPairPerCustomer = groupedData.Cache.Connect()
                             .WhereReasonsAre(ChangeReason.Add, ChangeReason.Remove)
                             .Group(t => t.ClientId)
                             .Synchronize(clientBalancesLocker)
                             .SubscribeMany(clienttrades =>
                             {
                                 var positionpercustomer = clienttrades.Cache.Connect(q => q.Pair == groupedData.Key && q.ClientId == clienttrades.Key)
                                                 .Synchronize(clientBalancesLocker)
                                                 .QueryWhenChanged(query =>
                                                 {
                                                     var amount1buy = clienttrades.Cache.Items.Where(trade => trade.Amount1 > 0).Sum(trade => trade.Amount1);
                                                     var amount1sell = clienttrades.Cache.Items.Where(trade => trade.Amount1 < 0).Sum(trade => trade.Amount1);
                                                     var amount2buy = clienttrades.Cache.Items.Where(trade => trade.Amount2 > 0).Sum(trade => trade.Amount2);
                                                     var amount2sell = clienttrades.Cache.Items.Where(trade => trade.Amount2 < 0).Sum(trade => trade.Amount2);

                                                     var pairpos = new CurPairPositionPerClient
                                                     {
                                                         ClientPair = clienttrades.Key + "_" + groupedData.Key,
                                                         ClientId = clienttrades.Key,
                                                         Pair = groupedData.Key,
                                                         Amount1 = amount1buy + amount1sell,
                                                         Amount2 = amount2buy + amount2sell,
                                                         Cur1 = groupedData.Key.Substring(0, 3),
                                                         Cur2 = groupedData.Key.Substring(groupedData.Key.Length - 3, 3),
                                                     };


                                                     curPairPositionPerClient.AddOrUpdate(pairpos);
                                                     //Console.WriteLine(pairpos); 
                                                     LogInfo("positionpercustomer- " + pairpos.ToString());
                                                     return pairpos;
                                                 }
                                                 )
                                                 .Subscribe();
                                 return new CompositeDisposable(positionpercustomer);
                             })
                             .Subscribe();

                         return new CompositeDisposable(PositionPerPairPerCustomer);
                     })
                     .Subscribe();
        }
    }
}