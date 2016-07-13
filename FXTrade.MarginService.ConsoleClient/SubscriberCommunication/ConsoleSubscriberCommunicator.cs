using FXTrade.MarginService.ServiceCore.SubscriberCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FXTrade.MarginService.BLL.Models;

namespace FXTrade.MarginService.ConsoleClient.SubscriberCommunication
{
    public class ConsoleSubscriberCommunicator : ISubscriberCommunicator
    {
        #region Quotes Pushes
        public void PushQuoteCreate(Quote quote)
        {
            Console.WriteLine("| Quote CREATED |  " + quote);
        }

        public void PushQuoteUpdate(Quote quote)
        {
            Console.WriteLine("| Quote UPDATED |  " + quote);
        }
        #endregion

        #region My Trades Pushes
        public void PushTradeCreate(Trade trade)
        {
            Console.WriteLine("| Trade CREATED |  " + trade);
        }

        public void PushTradeRemove(Trade trade)
        {
            Console.WriteLine("| Trade REMOVED |  " + trade);
        }

        public void PushTradeUpdate(Trade trade)
        {
            Console.WriteLine("| Trade UPDATED |  " + trade);
        }
        #endregion

        #region Client Balanes Pushes
        public void PushClientBalanceCreate(BalancePerClient clientBalance)
        {
            Console.WriteLine(clientBalance);
        }

        public void PushClientBalanceUpdate(BalancePerClient clientBalance)
        {
            Console.WriteLine(clientBalance);
        }

        public void PushClientBalanceRemove(BalancePerClient clientBalance)
        {
            Console.WriteLine(clientBalance);
        }

        #endregion

        #region CurPairPositionPerClient
        public void PushCurPairPositionPerClientCreate(CurPairPositionPerClient curPairPositionPerClient)
        {
            Console.WriteLine(curPairPositionPerClient);
        }

        public void PushCurPairPositionPerClientUpdate(CurPairPositionPerClient curPairPositionPerClient)
        {
            Console.WriteLine(curPairPositionPerClient);
        }

        public void PushCurPairPositionPerClientRemove(CurPairPositionPerClient curPairPositionPerClient)
        {
            Console.WriteLine(curPairPositionPerClient);
        }
        #endregion

        #region CurPairPositionPerClient
        public void PushCurPositionPerClientCreate(CurPositionPerClient curPositionPerClient)
        {
            Console.WriteLine(curPositionPerClient);
        }

        public void PushCurPositionPerClientUpdate(CurPositionPerClient curPositionPerClient)
        {
            Console.WriteLine(curPositionPerClient);
        }

        public void PushCurPositionPerClientRemove(CurPositionPerClient curPositionPerClient)
        {
            Console.WriteLine(curPositionPerClient);
        }
        #endregion
    }
}
