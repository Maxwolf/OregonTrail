using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ITrade
    {
        ReadOnlyCollection<IItem> PossibleTrades { get; }
        IVehicle Vehicle { get; }
        void TradeAttempt(IItem item);
    }
}
