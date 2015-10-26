using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Allows the player to purchase a number of oxen to pull their vehicle.
    /// </summary>
    public sealed class BuyOxenState : ModeState<StoreReceiptInfo>
    {
        private StringBuilder _oxenBuy;

        public BuyOxenState(IMode gameMode, StoreReceiptInfo userData) : base(gameMode, userData)
        {
            _oxenBuy = new StringBuilder();
            _oxenBuy
        }

        public override string GetStateTUI()
        {
            return _oxenBuy.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            
        }
    }
}