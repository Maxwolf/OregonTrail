using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ITrade : IMode
    {
        ReadOnlyCollection<IItem> PossibleTrades { get; }
        void TradeAttempt(IItem item);
    }
}