using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicData;

using System.Reactive.Disposables;
using FXTrade.MarginService.BLL.Models;

namespace FXTrade.MarginService.ServiceCore.Services
{

    public class PositionPerCurrencyCalculatorService : BaseService, IPositionPerCurrencyCalculatorService
    {
        
        public PositionPerCurrencyCalculatorService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient)
            : base(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient)
        {

        }

        /// <summary>
        /// Caluclate position per currency per customer
        /// </summary>
        public void CalculatePosistionPerCurrencyPerCustomer()
        {
           cleanUp = myTrades.Connect(trade => (trade.Status == Status.Open || trade.Status == Status.Pending))
                    .WhereReasonsAre(ChangeReason.Add, ChangeReason.Remove)
                    .Group(t => t.ClientId)
                    .SubscribeMany(groupedData =>
                    {
                        var curpositionpercustomer = 
                                                    //groupedData.Cache.Connect(q => q.ClientId == groupedData.Key)
                                                    groupedData.Cache.Connect()
                                                    .QueryWhenChanged(query =>
                                                    {

                                                        var CurUnionQuery = ((from t in query.Items
                                                                              select new CurPositionPerClient()
                                                                              {

                                                                                  ClientIdCur = t.ClientId + "_" + t.Cur1,
                                                                                  Cur = t.Cur1,
                                                                                  ClientId = t.ClientId,
                                                                                  Amount = t.Amount1,
                                                                              }))
                                                                          .Union(((from t in query.Items
                                                                                   select new CurPositionPerClient()
                                                                                   {

                                                                                       ClientIdCur = t.ClientId + "_" + t.Cur2,
                                                                                       Cur = t.Cur2,
                                                                                       ClientId = t.ClientId,
                                                                                       Amount = t.Amount2,
                                                                                   })))
                                                                          ;


                                                        var CurQuery = ((from t in CurUnionQuery
                                                                         group t by t.Cur into g
                                                                         select new CurPositionPerClient()
                                                                         {
                                                                             ClientIdCur = g.Max(a => a.ClientId) + "_" + g.Key,
                                                                             Cur = g.Key,
                                                                             ClientId = g.Max(a => a.ClientId),
                                                                             Amount = g.Sum(a => a.Amount),
                                                                             AmountInBase = ConvertToBaseCcy(g.Sum(a => a.Amount), g.Key),
                                                                         }));



                                                        //AddOrUpdate_curPositionPerClient(CurQuery);
                                                        curPositionPerClient.AddOrUpdate(CurQuery);


                                                        //curPositionPerClient
                                                        //    .Edit(updater =>
                                                        //    {
                                                        //        foreach (var item in CurQuery)
                                                        //        {
                                                        //            AppendLog(item.ToString());
                                                        //            updater.AddOrUpdate(item);
                                                        //        }
                                                        //    }
                                                        //    );
                                                        
                                                        return CurQuery;
                                                    }
                                                    )
                                                    .Subscribe();

                        //TODO calculate required margin per client - calculation based on NOP - Update customer balances

                        return new CompositeDisposable(curpositionpercustomer);
                    })
                    .Subscribe();
        }
    }
}