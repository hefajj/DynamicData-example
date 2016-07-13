using System;

namespace FXTrade.MarginService.ServiceCore.Contract
{
    public interface IBaseService: IDisposable
    {
        void LogError(string txt);
        void LogInfo(string txt);
    }
}