using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class SettlementMode : TrailMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public SettlementMode(TrailVehicle trailVehicle) : base(trailVehicle)
        {
        }

        public override TrailModeType Mode
        {
            get { return TrailModeType.Settlement; }
        }
    }
}