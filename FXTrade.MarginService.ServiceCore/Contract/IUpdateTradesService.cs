using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FXTrade.MarginService.ServiceCore.Contract
{
    public interface IUpdateTradesService
    {
        void UpdateAllTradesAndQuotes();
    }
}