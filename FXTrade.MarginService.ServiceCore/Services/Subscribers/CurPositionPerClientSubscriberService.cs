using FXTrade.MarginService.ServiceCore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FXTrade.MarginService.ServiceCore.SubscriberCommunication;
using FXTrade.MarginService.BLL.Models;
using System.Collections.ObjectModel;
using DynamicData;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class CurPositionPerClientSubscriberService : BaseService, ICurPositionPerClientSubscriberService
    {
         private ISourceCache<CurPositionPerClient, string> curPositionPerClient;

        private ReadOnlyObservableCollection<CurPositionPerClient> curPositionPerClientReadOnlyCollection;
        public ReadOnlyObservableCollection<CurPositionPerClient> CurPositionPerClientReadOnlyCollection { get { return curPositionPerClientReadOnlyCollection; } }

        public CurPositionPerClientSubscriberService(ISourceCache<CurPositionPerClient, string> curPositionPerClient, ISubscriberCommunicator communicator = null) 
            : base(communicator)
        {
            this.curPositionPerClient = curPositionPerClient;
        }

       

        public void SubscribeCurPositionsPerClient()
        {
            cleanUp = curPositionPerClient.Connect()
                                          .Bind(out curPositionPerClientReadOnlyCollection)
                                          .DisposeMany()
                                          .Subscribe(modifiedCurPositionPerClient =>
                                          {
                                              foreach (var position in modifiedCurPositionPerClient.ToList())
                                              {
                                                  if (position.Reason == ChangeReason.Add)
                                                      communicator.PushCurPositionPerClientCreate(position.Current);
                                                  else if (position.Reason == ChangeReason.Remove)
                                                      communicator.PushCurPositionPerClientRemove(position.Current);
                                                  else if (position.Reason == ChangeReason.Update)
                                                      communicator.PushCurPositionPerClientUpdate(position.Current);
                                              }
                                          });
        }
    }
}
