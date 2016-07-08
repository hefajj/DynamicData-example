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
    public class PAndLUpdaterService : BaseService, IPAndLUpdaterService
    {
        public PAndLUpdaterService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient)
            : base(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient)
        {

        }

        /// <summary>
        /// UPDATE P&L per CLIENT - recalculate only clients that has been affected
        /// </summary>
        public void UpdatePandLPerClient()
        {
            var myTradeLocker = new object();

            cleanUp = myTrades.Connect((trade => trade.Status == Status.Open || trade.Status == Status.Pending))
                    .Group(t => t.ClientId)
                    .Synchronize(myTradeLocker)
                    .SubscribeMany(g =>
                    {
                        //calculate and update P&L per client
                        //var balancegroup = myTrades.Connect(t => (t.Status == Status.Open || t.Status == Status.Pending) && t.ClientID == g.Key)
                        var balancegroup = g.Cache.Connect(t => t.ClientId == g.Key)
                                                          .Synchronize(myTradeLocker)
                                                         //TODO: SYNC LOCK
                                                         .QueryWhenChanged(d =>
                                                         {
                                                             var ProfilLoss = d.Items.Where(trade => trade.ClientId == g.Key).Sum(trade => trade.ProfitLoss);
                                                             var updatedClient = clientBalances.Items.Where(t => t.ClientID == g.Key).First();
                                                             updatedClient.ProfilLoss = ProfilLoss;
                                                             updatedClient.TotalMargin = ProfilLoss + updatedClient.InicialMargin;
                                                             updatedClient.BalanceLeft = updatedClient.SettledBalance + ProfilLoss - updatedClient.InicialMargin;

                                                             String message = "balanceupdate - P&L per client update, ClientBalances ClientID: " + updatedClient.ClientID;
                                                             LogInfo(message);

                                                             clientBalances.AddOrUpdate(updatedClient);
                                                             return ProfilLoss;
                                                         })
                                                         .Subscribe();
                        return new CompositeDisposable(balancegroup);
                    }
                    )
                    .Subscribe();
        }
    }
}