using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ITradingMode : IMode
    {
        ReadOnlyCollection<IItem> PossibleTrades { get; }
        void TradeAttempt(IItem item);
    }
}