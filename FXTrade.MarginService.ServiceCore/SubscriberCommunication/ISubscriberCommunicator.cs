using FXTrade.MarginService.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXTrade.MarginService.ServiceCore.SubscriberCommunication
{
    /// <summary>
    /// Contract provides functionality to push object to subscriber
    /// </summary>
    public interface ISubscriberCommunicator
    {
        #region Client Balanes Pushes
        void PushClientBalanceCreate(BalancePerClient clientBalance);

        void PushClientBalanceUpdate(BalancePerClient clientBalance);

        void PushClientBalanceRemove(BalancePerClient clientBalance);
        #endregion

        #region My Trades Pushes
        void PushTradeUpdate(Trade trade);

        void PushTradeCreate(Trade trade);

        void PushTradeRemove(Trade trade);
        #endregion

        #region Quotes Pushes

        void PushQuoteUpdate(Quote quote);

        void PushQuoteCreate(Quote quote);

        #endregion

        #region CurPairPositionPerClient
        void PushCurPairPositionPerClientCreate(CurPairPositionPerClient curPairPositionPerClient);

        void PushCurPairPositionPerClientUpdate(CurPairPositionPerClient curPairPositionPerClient);

        void PushCurPairPositionPerClientRemove(CurPairPositionPerClient curPairPositionPerClient);
        #endregion

        #region CurPositionPerClient
        void PushCurPositionPerClientCreate(CurPositionPerClient curPositionPerClient);

        void PushCurPositionPerClientUpdate(CurPositionPerClient curPositionPerClient);

        void PushCurPositionPerClientRemove(CurPositionPerClient curPositionPerClient);
        #endregion
    }
}
