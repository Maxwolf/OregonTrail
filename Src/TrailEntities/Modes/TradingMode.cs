using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Handles the interaction of the player party and another AI controlled party that offers up items for trading which
    ///     the player can choose to accept or not.
    /// </summary>
    public sealed class TradingMode : GameMode<TradingCommands>, ITradingMode
    {
        private readonly List<IItem> _possibleTrades;

        public TradingMode()
        {
            _possibleTrades = new List<IItem>();
        }

        public override SimulationMode Mode
        {
            get { return SimulationMode.Trade; }
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public override void TickMode()
        {
            throw new NotImplementedException();
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

        /// <summary>
        ///     Called by the active game mode when the text user interface is called. This will create a string builder with all
        ///     the data and commands that represent the concrete handler for this game mode.
        /// </summary>
        protected override string OnGetModeTUI()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        public override void OnModeRemoved()
        {
            throw new NotImplementedException();
        }
    }
}