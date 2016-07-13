using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicData;

using System.Reactive.Linq;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ServiceCore.SubscriberCommunication;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class StopOutExecutorService : BaseService, IStopOutExecutorService
    {
        private ISourceCache<Quote, string> quotes;
        private ISourceCache<BalancePerClient, long> clientBalances;
        private ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient;
        private ITradesModifierService tradesModifierService;
        private ICurrencyConverterService currencyConverterService;

        public StopOutExecutorService(ISourceCache<Quote, string> quotes,
                                      ISourceCache<BalancePerClient, long> clientBalances,
                                      ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                                      ITradesModifierService tradesModifierService,
                                      ICurrencyConverterService currencyConverterService, 
                                      ISubscriberCommunicator communicator = null)
            : base(communicator)
        {
            this.quotes = quotes;
            this.clientBalances = clientBalances;
            this.curPairPositionPerClient = curPairPositionPerClient;
            this.tradesModifierService = tradesModifierService;
            this.currencyConverterService = currencyConverterService;
        }
        /// <summary>
        /// trigger if balance is smaller then 50
        /// </summary>
        public void ManageStopOuts()
        {
            cleanUp = clientBalances.Connect(dssds => dssds.BalanceLeft < 80)
                      //  .Synchronize(myTrade_locker)
                      .WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
                      //TODO ADD FILTER - total possition > 0 - to go hear agregated possition of the customer should be > then 0
                      //.Filter(f => f.BalanceLeft < 80)
                      .Subscribe(
                              c =>
                              {
                                  foreach (var item in c)
                                  {
                                      LogInfo("Check if we can clos POSITION check - BALANCE IS BELOW 50 for client: " + item.Current.ClientID + " item.Current.BalanceLeft:" + item.Current.BalanceLeft + " reson" + item.Reason.ToString());

                                      var positionSToclose = curPairPositionPerClient.Items.Where(r => (r.ClientId == item.Current.ClientID && r.Amount1 != 0)).OrderByDescending(d => currencyConverterService.ConvertToBaseCcy(d.Amount1, d.Cur1));//;

                                      if (positionSToclose.Count() > 0)
                                      {
                                          var positionToclose = positionSToclose.First();

                                          LogInfo("CLOSE THE POSITION - " + positionToclose);

                                          double lastquote = 0;
                                          if (positionToclose.Amount1 > 0)
                                              lastquote = quotes.Items.Where(r => r.Pair == positionToclose.Pair).First().Bid;
                                          else
                                              lastquote = quotes.Items.Where(r => r.Pair == positionToclose.Pair).First().Ask;

                                          Trade GenerateClosingTrade = new Trade
                                          {
                                              //Id = myTrades.Count,
                                              ClientId = positionToclose.ClientId,
                                              Pair = positionToclose.Pair,
                                              Amount1 = positionToclose.Amount1 * (-1),
                                              Status = Status.Pending,
                                              //Status = Status.Open,
                                              OpenPrice = lastquote,
                                              CurrentPrice = lastquote,
                                              Amount2 = positionToclose.Amount1 * (-1) * lastquote,
                                              Cur1 = positionToclose.Pair.Substring(0, 3),
                                              Cur2 = positionToclose.Pair.Substring(positionToclose.Pair.Length - 3, 3),
                                          };

                                          tradesModifierService.AddMyTrade(GenerateClosingTrade);

                                      }



                                  }
                              }
                      );
        }
    }
}