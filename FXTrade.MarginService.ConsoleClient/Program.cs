using DynamicData;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ServiceCore.Contract;
using FXTrade.MarginService.ServiceCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.Reactive.Linq;

namespace FXTrade.MarginService.ConsoleClient
{

    
    

    class Program
    {
        private static ISourceCache<Quote, string> quotes;
        private static ISourceCache<Trade, long> myTrades;
        private static ISourceCache<BalancePerClient, long> clientBalances;
        private static ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient;
        private static ISourceCache<CurPositionPerClient, string> curPositionPerClient;
        private static object myTrades_Locker;
        private static object CurPositionPerClient_Locker;
        

        static void Main(string[] args)
        {

            XmlConfigurator.Configure();

            quotes = new SourceCache<Quote, string>(quote => quote.Pair);
            myTrades = new SourceCache<Trade, long>(trade => trade.Id);
            clientBalances = new SourceCache<BalancePerClient, long>(Balance => Balance.ClientID);
            curPairPositionPerClient = new SourceCache<CurPairPositionPerClient, string>(CurPairPositionPerClient => CurPairPositionPerClient.ClientPair);
            curPositionPerClient = new SourceCache<CurPositionPerClient, string>(CurPositionPerClient => CurPositionPerClient.ClientIdCur);
            myTrades_Locker = new object();
            CurPositionPerClient_Locker = new object();

            // services Initialization
            IQuoteExtractorService quoteExtractor = new QuoteExtractorService(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient);
            new Thread(quoteExtractor.ExtractData).Start();

            ILogPrinterService logPrinterService = new LogPrinterService(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient);
            logPrinterService.PrintClientBalances();
            logPrinterService.PrintcurPairPositionPerClient();
            logPrinterService.PrintmyTrades();
            logPrinterService.PrintCurPositionPerClient();

            IStopOutExecutorService stopOutExecutorService = new StopOutExecutorService(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient);
            stopOutExecutorService.ManageStopOuts();

            IPositionPerCurrencyPairCalculatorService positionPerCurrencyPairCalculatorService = new PositionPerCurrencyPairCalculatorService(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient);
            positionPerCurrencyPairCalculatorService.CalculatePosistionPerCurrencyPairPerCustomer();

            IPositionPerCurrencyCalculatorService positionPerCurrencyCalculatorService = new PositionPerCurrencyCalculatorService(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient);
            positionPerCurrencyCalculatorService.CalculatePosistionPerCurrencyPerCustomer();

            IMarginCalculatorService marginCalculatorService = new MarginCalculatorService(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient);
            marginCalculatorService.CalculateRequiredMargin();

            IPAndLUpdaterService pAndLUpdaterService = new PAndLUpdaterService(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient);
            pAndLUpdaterService.UpdatePandLPerClient();

            ICurrencyPositionPerClientUpdaterService currencyPositionPerClientUpdaterService = new CurrencyPositionPerClientUpdaterService(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient);
            currencyPositionPerClientUpdaterService.UpdateAllCurrenciesPositions();

            IUpdateTradesService updateTradesService = new UpdateTradesService(myTrades, quotes, clientBalances, curPairPositionPerClient, curPositionPerClient);
            updateTradesService.UpdateAllTradesAndQuotes();
        }
    }
}
