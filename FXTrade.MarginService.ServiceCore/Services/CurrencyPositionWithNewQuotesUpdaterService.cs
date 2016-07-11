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
            

        }
    }
}