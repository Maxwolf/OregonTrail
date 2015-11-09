using System;
using System.Collections.Generic;
using TrailEntities.Entity;
using TrailEntities.Mode;

namespace TrailEntities.Game.Trade
{
    /// <summary>
    ///     Handles the interaction of the player party and another AI controlled party that offers up items for trading which
    ///     the player can choose to accept or not.
    /// </summary>
    public sealed class TradingGameMode : ModeProduct
    {
        private readonly HashSet<SimItem> _possibleTrades;

        public TradingGameMode() : base(false)
        {
            _possibleTrades = new HashSet<SimItem>();
        }

        public override GameMode ModeType
        {
            get { return GameMode.Trade; }
        }

        public IEnumerable<SimItem> PossibleTrades
        {
            get { return _possibleTrades; }
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game gameMode.
        /// </summary>
        public override void TickMode()
        {
            throw new NotImplementedException();
        }

        public void TradeAttempt(SimItem simItem)
        {
            // Cannot trade if there are no offers.
            if (_possibleTrades.Count <= 0)
                return;
        }

        /// <summary>
        ///     Fired when this game gameMode is removed from the list of available and ticked GameMode in the simulation.
        /// </summary>
        /// <param name="modeType"></param>
        protected override void OnModeRemoved(GameMode modeType)
        {
            throw new NotImplementedException();
        }
    }
}