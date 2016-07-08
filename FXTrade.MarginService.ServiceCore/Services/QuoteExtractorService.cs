using DynamicData;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class QuoteExtractorService : BaseService, IQuoteExtractorService
    {

        public QuoteExtractorService(ISourceCache<Trade, long> myTrades,
                           ISourceCache<Quote, string> quotes,
                           ISourceCache<BalancePerClient, long> clientBalances,
                           ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient,
                           ISourceCache<CurPositionPerClient, string> curPositionPerClient)
            : base(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient)
        {

        }
        public void ExtractData()
        {
            Random rand = new Random();

            var pairs = new[] { "EUR/GBP", "EUR/USD", "EUR/CHF", "GBP/USD", "USD/CHF", "USD/HUF", "EUR/HUF" };
            var customers = new[] { "CUST1", "CUST2", "CUST3", "CUST4", "CUST5", "CUST6", "CUST7" };

            long TradeIdnum = 0;

            for (int i = 0; i < 7; i++)
            {
                var newquote = new Quote
                {
                    Pair = pairs[i],
                    Ask = Math.Round(rand.NextDouble(), 5),
                    Bid = Math.Round(rand.NextDouble(), 5),
                    Cur1 = pairs[i].Substring(0, 3),
                    Cur2 = pairs[i].Substring(pairs[i].Length - 3, 3),
                };
                ////Console.WriteLine(newquote);
                LogInfo(newquote.ToString());
                quotes.AddOrUpdate(newquote);
            }

            for (int i = 0; i < 6; i++)
            {
                //var newcustomer = new Balance(i, rand.Next(100), rand.Next(100));
                var newcustomer = new BalancePerClient(i, 100, 0, 0);

                clientBalances.AddOrUpdate(newcustomer);
                /////Console.WriteLine("added customer: " + newcustomer);
                //appendlog("added customer: " + newcustomer);
            }

            while (true)
            {

                var pair = pairs[rand.Next(6)];

                var newquote = new Quote
                {
                    Pair = pair,
                    Ask = Math.Round(rand.NextDouble(), 5),
                    Bid = Math.Round(rand.NextDouble(), 5),
                    Cur1 = pair.Substring(0, 3),
                    Cur2 = pair.Substring(pair.Length - 3, 3)
                };
                ////Console.WriteLine(newquote);
                LogInfo(newquote.ToString());
                quotes.AddOrUpdate(newquote);

                var currentprice = Math.Round(rand.NextDouble(), 5);
                var amount = rand.Next(-100, 100);
                if (amount == 0)
                {
                    amount = 1;
                }
                var Pair = pairs[rand.Next(6)];

                var newtrade = new Trade
                {
                    Id = myTrades.Count,
                    ClientId = rand.Next(6),
                    Pair = Pair,
                    Status = Status.Open,
                    Amount1 = amount,
                    Amount2 = amount * currentprice * (-1),
                    Cur1 = Pair.Substring(0, 3),
                    Cur2 = Pair.Substring(Pair.Length - 3, 3),
                };

                if (newtrade.Amount1 > 0)
                {
                    newtrade.OpenPrice = quotes.Items.Where(r => r.Pair == newtrade.Pair).First().Ask;
                    newtrade.CurrentPrice = quotes.Items.Where(r => r.Pair == newtrade.Pair).First().Bid;
                }
                else
                {
                    newtrade.OpenPrice = quotes.Items.Where(r => r.Pair == newtrade.Pair).First().Bid;
                    newtrade.CurrentPrice = quotes.Items.Where(r => r.Pair == newtrade.Pair).First().Ask;
                }


                if (TradeIdnum < 20)
                {

                    TradeIdnum = TradeIdnum + 1;
                    string printtext = newtrade.ToString();

                    LogInfo(printtext);
                    AddMyTrade(newtrade);


                }
                Thread.Sleep(500);
            }
        }
    }
}