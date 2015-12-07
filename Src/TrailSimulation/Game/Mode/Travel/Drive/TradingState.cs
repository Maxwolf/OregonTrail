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
    [RequiredMode(Mode.Travel)]
    public sealed class TradingState : StateProduct<TravelInfo>
    {
        private readonly HashSet<SimItem> _possibleTrades;

        public IEnumerable<SimItem> PossibleTrades
        {
            get { return _possibleTrades; }
        }

        public void TradeAttempt(SimItem item)
        {
            // Cannot trade if there are no offers.
            if (_possibleTrades.Count <= 0)
                return;
        }

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public TradingState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            throw new NotImplementedException();
        }
    }
}