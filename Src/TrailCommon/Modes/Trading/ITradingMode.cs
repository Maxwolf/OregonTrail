using System.Collections.Generic;

namespace TrailCommon
{
    public interface ITradingMode : IMode
    {
        IEnumerable<Item> PossibleTrades { get; }
        void TradeAttempt(Item item);
    }
}