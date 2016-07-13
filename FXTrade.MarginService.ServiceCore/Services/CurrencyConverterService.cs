using DynamicData;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class CurrencyConverterService : BaseService, ICurrencyConverterService
    {
        private ISourceCache<Quote, string> quotes;

        public CurrencyConverterService(ISourceCache<Quote, string> quotes)
        {
            this.quotes = quotes;
        }

        public double ConvertToBaseCcy(double amount, string quoteDccy)
        {
            string baseccy = "EUR";
            if (quoteDccy == baseccy)
            {
                return amount;
            }
            var quote = quotes.Items.Where(a => a.Pair == (baseccy + "/" + quoteDccy)).First();
            double converted = amount * quote.Ask;
            return converted;
        }
    }
}
