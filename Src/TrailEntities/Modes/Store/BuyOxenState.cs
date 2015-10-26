using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Allows the player to purchase a number of oxen to pull their vehicle.
    /// </summary>
    public sealed class BuyOxenState : ModeState<StoreReceiptInfo>
    {
        public BuyOxenState(IMode gameMode, StoreReceiptInfo userData) : base(gameMode, userData)
        {
        }

        public override string GetStateTUI()
        {
            throw new NotImplementedException();
        }

        public override void OnInputBufferReturned(string input)
        {
            throw new NotImplementedException();
        }
    }
}