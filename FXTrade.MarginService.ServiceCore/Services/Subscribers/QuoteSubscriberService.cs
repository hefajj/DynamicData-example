using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ServiceCore.SubscriberCommunication;
using System.Collections.ObjectModel;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class QuoteSubscriberService : BaseService, IQuoteSubscriberService
    {
        private ISourceCache<Quote, string> quotes;

        private ReadOnlyObservableCollection<Quote> quotesReadOnlyCollection;
        public ReadOnlyObservableCollection<Quote> QuotesReadOnlyCollection { get { return quotesReadOnlyCollection; } }


        public QuoteSubscriberService(ISourceCache<Quote, string> quotes,
                                      ISubscriberCommunicator communicator)
            : base(communicator)
        {
            this.quotes = quotes;
        }

        public void SubscribeQuotes()
        {
            cleanUp = quotes.Connect()
                    .Bind(out quotesReadOnlyCollection)                   
                    .Subscribe(quotes =>
                    {

                        foreach (var quote in quotes.ToList())
                        {
                            if (quote.Reason == ChangeReason.Add)
                                communicator.PushQuoteCreate(quote.Current);
                            else if (quote.Reason == ChangeReason.Update)
                                communicator.PushQuoteUpdate(quote.Current);
                        }
                    });
        }
    }
}
