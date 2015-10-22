using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ITradingMode : IMode<TradingCommands>
    {
        ReadOnlyCollection<IItem> PossibleTrades { get; }
        void TradeAttempt(IItem item);
    }
}