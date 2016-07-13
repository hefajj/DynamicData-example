using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicData;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ServiceCore.SubscriberCommunication;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class CurrencyPositionWithNewQuotesUpdaterService : BaseService, ICurrencyPositionWithNewQuotesUpdaterService
    {
        private ISourceCache<CurPositionPerClient, string> curPositionPerClient;
        private ISourceCache<Quote, string> quotes;


        public CurrencyPositionWithNewQuotesUpdaterService(ISourceCache<Quote, string> quotes,                           
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient, 
                           ISubscriberCommunicator communicator= null)
            : base(communicator)
        {
            this.curPositionPerClient = curPositionPerClient;
            this.quotes = quotes;
        }


        /// <summary>
        /// UPDATE ALL CurPositionPerClient WITH NEW QUOTES 
        /// </summary>
        public void UpdateCurrencyPositionWithNewQuotes()
        {
            

        }
    }
}