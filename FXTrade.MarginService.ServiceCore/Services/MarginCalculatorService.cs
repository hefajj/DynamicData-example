using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicData;

using System.Reactive.Disposables;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ServiceCore.SubscriberCommunication;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class MarginCalculatorService : BaseService, IMarginCalculatorService
    {
        private ISourceCache<BalancePerClient, long> clientBalances;
        private ISourceCache<CurPositionPerClient, string> curPositionPerClient;

        public MarginCalculatorService(ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient,
                           ISubscriberCommunicator communicator = null)
            : base(communicator)
        {
            this.clientBalances = clientBalances;
            this.curPositionPerClient = curPositionPerClient;
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