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
    public class MarginCalculatorService : BaseService, IMarginCalculatorService
    {
        public MarginCalculatorService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient)
            : base(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient)
        {

        }
        /// <summary>
        /// Calculate Required margin from NOP
        /// </summary>
        public void CalculateRequiredMargin()
        {
            //TODO calulate NOP per client
             cleanUp = curPositionPerClient.Connect(nop => nop.AmountInBase > 0)
                                        .WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
                                        .Group(t => t.ClientId)
                                        .SubscribeMany(CurPositionPerClient1 =>
                                        {


                                            var nopmargin1 = CurPositionPerClient1.Cache.Connect(q => q.ClientId == CurPositionPerClient1.Key)
                                                                .QueryWhenChanged(query =>
                                                                {
                                                                    var openamountlong = query.Items.Sum(trade => trade.AmountInBase);

                                                                    var updatedClient = clientBalances.Items.Where(t => t.ClientID == CurPositionPerClient1.Key).First();
                                                                    updatedClient.InicialMargin = openamountlong * 0.1;
                                                                    updatedClient.TotalMargin = updatedClient.ProfilLoss + updatedClient.InicialMargin;
                                                                    updatedClient.BalanceLeft = updatedClient.SettledBalance - updatedClient.TotalMargin;

                                                                    String message = " NOP per client - Nopmargin, ClientBalances ClientID: " + updatedClient.ClientID;
                                                                    LogInfo(message);

                                                                    clientBalances.AddOrUpdate(updatedClient);

                                                                        //Console.WriteLine(updatedClient);
                                                                        //appendlog(updatedClient.ToString());

                                                                        return openamountlong;
                                                                })
                                                                .Subscribe();

                                            return new CompositeDisposable(nopmargin1);
                                        })
                                        .Subscribe();
        }
    }
}