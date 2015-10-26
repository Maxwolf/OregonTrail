using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ITradingMode : IMode
    {
        ReadOnlyCollection<Item> PossibleTrades { get; }
        void TradeAttempt(Item item);
    }
}