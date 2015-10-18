using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class SettlementModeView : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public SettlementModeView(Vehicle vehicle) : base(vehicle)
        {
        }

        public override ModeType Mode
        {
            get { return ModeType.Settlement; }
        }
    }
}