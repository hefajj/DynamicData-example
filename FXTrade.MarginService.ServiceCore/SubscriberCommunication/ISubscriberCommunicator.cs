using FXTrade.MarginService.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXTrade.MarginService.ServiceCore.SubscriberCommunication
{
    public interface ISubscriberCommunicator
    {
        void PushTradeUpdateToSubscriber(Trade trade);

        void PushTradeCreateToSubscriber(Trade trade);

        void PushTradeRemoveToSubscriber(Trade trade);
    }
}
