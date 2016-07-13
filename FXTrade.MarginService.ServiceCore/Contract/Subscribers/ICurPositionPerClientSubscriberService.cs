using FXTrade.MarginService.BLL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXTrade.MarginService.ServiceCore.Contract
{
   public interface ICurPositionPerClientSubscriberService: IBaseService
    {
        ReadOnlyObservableCollection<CurPositionPerClient> CurPositionPerClientReadOnlyCollection { get; }
        void SubscribeCurPositionsPerClient();


    }
}
