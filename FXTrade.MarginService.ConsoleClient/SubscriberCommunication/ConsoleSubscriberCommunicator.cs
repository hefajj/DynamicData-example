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
        public void PushTradeCreateToSubscriber(Trade trade)
        {
            Console.WriteLine("| Trade CREATED |  " + trade);
        }

        public void PushTradeRemoveToSubscriber(Trade trade)
        {
            Console.WriteLine("| Trade REMOVED |  " + trade);
        }

        public void PushTradeUpdateToSubscriber(Trade trade)
        {
            Console.WriteLine("| Trade UPDATED |  " + trade);
        }
    }
}
