using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public class TradeModel : ITrade
    {
        public ReadOnlyCollection<IItem> PossibleTrades
        {
            get { throw new System.NotImplementedException(); }
        }

        public IVehicle Vehicle
        {
            get { throw new System.NotImplementedException(); }
        }

        public void TradeAttempt(IItem item)
        {
            throw new System.NotImplementedException();
        }
    }
}