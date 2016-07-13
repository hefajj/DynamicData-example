using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXTrade.MarginService.ServiceCore.Contract
{
   public interface IMarginCalculatorService: IBaseService
    {
        void CalculateRequiredMargin();
    }
}
