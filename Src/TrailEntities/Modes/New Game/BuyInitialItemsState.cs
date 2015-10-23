using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class BuyInitialItemsState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public BuyInitialItemsState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
        }

        public override void TickState()
        {
            throw new NotImplementedException();
        }

        public override string GetStateTUI()
        {
            throw new NotImplementedException();
        }

        public override void OnInputBufferReturned(string input)
        {
            // Nothing to see here, move along...
        }
    }
}