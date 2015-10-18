using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Handles the interaction of the player party and another AI controlled party that offers up items for trading which
    ///     the player can choose to accept or not.
    /// </summary>
    public sealed class TradeMode : GameMode, ITrade
    {
        private readonly List<IItem> _possibleTrades;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public TradeMode(IGameSimulation game) : base(game)
        {
            _possibleTrades = new List<IItem>();
        }

        public override ModeType Mode
        {
            get { return ModeType.Trade; }
        }

        public ReadOnlyCollection<IItem> PossibleTrades
        {
            get { return new ReadOnlyCollection<IItem>(_possibleTrades); }
        }

        public void TradeAttempt(IItem item)
        {
            // Cannot trade if there are no offers.
            if (_possibleTrades.Count <= 0)
                return;
        }
    }
}