using FXTrade.MarginService.BLL.Models;
using System;
using System.Collections.ObjectModel;

namespace FXTrade.MarginService.ServiceCore.Contract
{
    public interface ISubscribeTradesService:IDisposable
    {
        void SubscribeMyTrades();
        ReadOnlyObservableCollection<Trade> MyTradesReadOnlyCollection { get; }
    }
}