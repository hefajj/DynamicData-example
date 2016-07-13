
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FXTrade.MarginService.BLL.Models;
using System.Collections.ObjectModel;
using DynamicData;
using FXTrade.MarginService.ServiceCore.SubscriberCommunication;
using FXTrade.MarginService.ServiceCore.Contract;

namespace FXTrade.MarginService.ServiceCore.Services
{
    public class CurPairPositionPerClientSubscriberService : BaseService, ICurPairPositionPerClientSubscriberService
    {
        private ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient;


        private ReadOnlyObservableCollection<CurPairPositionPerClient> curPairPositionPerClientReadOnlyCollection;
        public ReadOnlyObservableCollection<CurPairPositionPerClient> CurPairPositionPerClientReadOnlyCollection { get { return curPairPositionPerClientReadOnlyCollection; } }

        public CurPairPositionPerClientSubscriberService(ISourceCache<CurPairPositionPerClient, string> curPairPositionPerClient, ISubscriberCommunicator communicator = null)
            : base(communicator)
        {
            this.curPairPositionPerClient = curPairPositionPerClient;
        }



        public void SubscribeCurPairPositionPerClient()
        {
            cleanUp = curPairPositionPerClient.Connect()
                                              .Bind(out curPairPositionPerClientReadOnlyCollection)
                                              .DisposeMany()
                                              .Subscribe(modifiedCurPairPositionPerClient =>
                                              {
                                                  foreach (var pairPosition in modifiedCurPairPositionPerClient.ToList())
                                                  {
                                                      if (pairPosition.Reason == ChangeReason.Add)
                                                          communicator.PushCurPairPositionPerClientCreate(pairPosition.Current);
                                                      else if (pairPosition.Reason == ChangeReason.Remove)
                                                          communicator.PushCurPairPositionPerClientRemove(pairPosition.Current);
                                                      else if (pairPosition.Reason == ChangeReason.Update)
                                                          communicator.PushCurPairPositionPerClientUpdate(pairPosition.Current);
                                                  }
                                              });
        }
    }
}
