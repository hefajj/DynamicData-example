using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;
namespace FXTrade.MarginService.BLL.Models
{
    public class CurPositionPerClient
    {
        private readonly ISubject<double> _CurPositionPerClientChangedSubject = new ReplaySubject<double>(1);

        public string ClientIdCur { get; set; }
        public long ClientId { get; set; }
        public string Cur;
        public double Amount { get; set; }
        public double AmountInBase { get; set; }
        //public double AmountLong { get; set; }

        public override string ToString()
        {
            return string.Format("CurPositionPerClient:|ClientID_Cur|{3}|Client|{0}|Cur|{1}|Amount|{2}|Amount_in_Base|{4}",
                                  ClientId, Cur, Amount, ClientIdCur, AmountInBase);
        }

        public void SetAmountInBase(double Price)
        {
            AmountInBase = Amount * Price;

            _CurPositionPerClientChangedSubject.OnNext(Price);
        }

    }
}
