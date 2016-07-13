using DynamicData;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ServiceCore.Contract;
using FXTrade.MarginService.ServiceCore.SubscriberCommunication;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public abstract class BaseService : IBaseService
    { 

        private log4net.ILog logger;
        protected ISubscriberCommunicator communicator;
        protected IDisposable cleanUp;

        public BaseService(ISubscriberCommunicator communicator = null)
        {
            this.communicator = communicator;
            this.logger = LogManager.GetLogger("MarginTrader");
        }
        public void Dispose()
        {
            cleanUp?.Dispose();
        }
        public void LogInfo(string txt)
        {
            logger.Info(txt + "\r");
        }

        public void LogError(string txt)
        {
            logger.Info(txt + "\r");
        }       
    }
}