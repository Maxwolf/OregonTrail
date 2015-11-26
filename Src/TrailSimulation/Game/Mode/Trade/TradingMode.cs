using System;
using System.Collections.Generic;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Handles the interaction of the player party and another AI controlled party that offers up items for trading which
    ///     the player can choose to accept or not.
    /// </summary>
    public sealed class TradingMode : ModeProduct<TradingCommands, TradingInfo>
    {
        private readonly HashSet<SimItem> _possibleTrades;

        public override Mode Mode
        {
            get { return Mode.Trade; }
        }

        public IEnumerable<SimItem> PossibleTrades
        {
            get { return _possibleTrades; }
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public override void TickMode()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Called after the mode has been added to list of modes and made active.
        /// </summary>
        public override void OnModePostCreate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Called when the mode manager in simulation makes this mode the currently active game mode. Depending on order of
        ///     modes this might not get called until the mode is actually ticked by the simulation.
        /// </summary>
        public override void OnModeActivate()
        {
            throw new NotImplementedException();
        }

        public void TradeAttempt(SimItem item)
        {
            // Cannot trade if there are no offers.
            if (_possibleTrades.Count <= 0)
                return;
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        /// <param name="mode"></param>
        protected override void OnModeRemoved(Mode mode)
        {
            throw new NotImplementedException();
        }
    }
}