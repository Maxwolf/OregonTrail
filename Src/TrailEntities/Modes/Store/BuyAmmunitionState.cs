using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class BuyAmmunitionState : ModeState<StoreReceiptInfo>
    {
        public BuyAmmunitionState(IMode gameMode, StoreReceiptInfo userData) : base(gameMode, userData)
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