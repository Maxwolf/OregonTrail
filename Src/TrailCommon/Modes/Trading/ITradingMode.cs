using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ITradingMode : IMode
    {
        IEnumerable<Item> PossibleTrades { get; }
        void TradeAttempt(Item item);
    }
}