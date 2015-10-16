using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ITrade : ITrailMode
    {
        ReadOnlyCollection<IItem> PossibleTrades { get; }
        void TradeAttempt(IItem item);
    }
}