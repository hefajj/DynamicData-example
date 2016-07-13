using DynamicData;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ConsoleClient.SubscriberCommunication;
using FXTrade.MarginService.ServiceCore.Contract;
using FXTrade.MarginService.ServiceCore.Services;
using FXTrade.MarginService.ServiceCore.SubscriberCommunication;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FXTrade.MarginService.ConsoleClient
{
    class Program
    {
        private static ISourceCache<Quote, string> quotes;
        private static ISourceCache<Trade, long> myTrades;
        private static ISourceCache<Trade, long> myTradesQuoteUpdate;

        private static ISourceCache<BalancePerClient, long> clientBalances;
        private static ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient;

        private static ISourceCache<CurPositionPerClient, string> curPositionPerClient;
        private static IObservableCache<CurPositionPerClient, string> curPositionPerClientCache;

        private static ISourceCache<CurPositionPerClient, string> curPositionPerClientQuoteUpdate;
        

        private static object myTradesLocker;
        private static object CurPositionPerClientLocker;


        static void Main(string[] args)
        {

            XmlConfigurator.Configure();

            quotes = new SourceCache<Quote, string>(quote => quote.Pair);
            myTrades = new SourceCache<Trade, long>(trade => trade.Id);
            myTradesQuoteUpdate = new SourceCache<Trade, long>(trade => trade.Id);
            clientBalances = new SourceCache<BalancePerClient, long>(Balance => Balance.ClientID);
            curPairPositionPerClient = new SourceCache<CurPairPositionPerClient, string>(CurPairPositionPerClient => CurPairPositionPerClient.ClientPair);
            curPositionPerClient = new SourceCache<CurPositionPerClient, string>(CurPositionPerClient => CurPositionPerClient.ClientIdCur);
            curPositionPerClientCache = curPositionPerClient.AsObservableCache();

            curPositionPerClientQuoteUpdate = new SourceCache<CurPositionPerClient, string>(CurPositionPerClient => CurPositionPerClient.ClientIdCur);



            myTradesLocker = new object();
            CurPositionPerClientLocker = new object();

            // services Initialization
            ITradesModifierService tradesModifierService = new TradesModifierService(myTrades);
            ICurrencyConverterService currencyConverterService = new CurrencyConverterService(quotes);

            IQuoteExtractorService quoteExtractor = new QuoteExtractorService(myTrades, quotes, clientBalances, tradesModifierService);
            new Thread(quoteExtractor.ExtractData).Start();

            ILogPrinterService logPrinterService = new LogPrinterService(myTrades, clientBalances, curPairPositionPerClient, curPositionPerClient, curPositionPerClientCache, curPositionPerClientQuoteUpdate, myTradesQuoteUpdate);
            logPrinterService.PrintClientBalances();
            logPrinterService.PrintcurPairPositionPerClient();
            logPrinterService.PrintmyTrades();
            logPrinterService.PrintcurPositionPerClientCache();
            logPrinterService.PrintmyTradesQuoteUpdate();
            logPrinterService.PrintcurPositionPerClientQuoteUpdate();

            IStopOutExecutorService stopOutExecutorService = new StopOutExecutorService(quotes, clientBalances, curPairPositionPerClient, tradesModifierService, currencyConverterService);
            stopOutExecutorService.ManageStopOuts();

            IPositionPerCurrencyPairCalculatorService positionPerCurrencyPairCalculatorService = new PositionPerCurrencyPairCalculatorService(myTrades, curPairPositionPerClient);
            positionPerCurrencyPairCalculatorService.CalculatePosistionPerCurrencyPairPerCustomer();

            IPositionPerCurrencyCalculatorService positionPerCurrencyCalculatorService = new PositionPerCurrencyCalculatorService(myTrades, curPositionPerClient, currencyConverterService);
            positionPerCurrencyCalculatorService.CalculatePosistionPerCurrencyPerCustomer();

            IMarginCalculatorService marginCalculatorService = new MarginCalculatorService(clientBalances, curPositionPerClient);
            marginCalculatorService.CalculateRequiredMargin();

            IPAndLUpdaterService pAndLUpdaterService = new PAndLUpdaterService(myTrades, clientBalances);
            pAndLUpdaterService.UpdatePandLPerClient();

            ICurrencyPositionPerClientUpdaterService currencyPositionPerClientUpdaterService = new CurrencyPositionPerClientUpdaterService(quotes, curPositionPerClient, curPositionPerClientQuoteUpdate, curPositionPerClientCache);
            currencyPositionPerClientUpdaterService.UpdateAllCurrenciesPositions();

            IUpdateTradesService updateTradesService = new UpdateTradesService(myTrades, quotes, currencyConverterService, myTradesQuoteUpdate);
            updateTradesService.UpdateAllTradesAndQuotes();
            
        }
    }
}
