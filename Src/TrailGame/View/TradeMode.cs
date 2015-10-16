using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class TradeMode : TrailMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public TradeMode(TrailVehicle trailVehicle) : base(trailVehicle)
        {
        }

        public override TrailModeType Mode
        {
            get { return TrailModeType.Trade; }
        }
    }
}