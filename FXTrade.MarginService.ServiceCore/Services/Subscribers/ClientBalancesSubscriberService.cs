using DynamicData;
using FXTrade.MarginService.BLL.Models;
using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FXTrade.MarginService.ServiceCore.SubscriberCommunication;
using System.Collections.ObjectModel;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class ClientBalancesSubscriberService : BaseService, IClientBalancesSubscriberService
    {
        private ISourceCache<BalancePerClient, long> clientBalances;
        
        private ReadOnlyObservableCollection<BalancePerClient> clientBalancesReadOnlyCollection;
        public ReadOnlyObservableCollection<BalancePerClient> ClientBalancesReadOnlyCollection { get { return clientBalancesReadOnlyCollection; } }


        public ClientBalancesSubscriberService(ISourceCache<BalancePerClient, long> clientBalances, ISubscriberCommunicator communicator = null)
            : base(communicator)
        {
            this.clientBalances = clientBalances;
        }

       
        public void SubscribeClientBalances()
        {
            cleanUp = clientBalances.Connect()
                                    .Bind(out clientBalancesReadOnlyCollection)
                                    .DisposeMany()
                                    .Subscribe(modifiedClientBalances =>
                                    {
                                        foreach (var balance in modifiedClientBalances.ToList())
                                        {
                                            if (balance.Reason == ChangeReason.Add)
                                                communicator.PushClientBalanceCreate(balance.Current);
                                            else if (balance.Reason == ChangeReason.Remove)
                                                communicator.PushClientBalanceRemove(balance.Current);
                                            else if (balance.Reason == ChangeReason.Update)
                                                communicator.PushClientBalanceUpdate(balance.Current);
                                        }
                                    });
        }
    }
}
