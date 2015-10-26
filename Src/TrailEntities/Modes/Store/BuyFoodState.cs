using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Allows the player to purchase some food, it comes in pounds and does not delineate between sugar, bacon, eggs, etc.
    ///     Food is just food that people consume everyday.
    /// </summary>
    public sealed class BuyFoodState : ModeState<StoreReceiptInfo>
    {
        public BuyFoodState(IMode gameMode, StoreReceiptInfo userData) : base(gameMode, userData)
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