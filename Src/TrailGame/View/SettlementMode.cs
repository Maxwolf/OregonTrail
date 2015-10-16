using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class SettlementMode : Mode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public SettlementMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override TrailCommon.GameMode ModeType
        {
            get { return TrailCommon.GameMode.Settlement; }
        }
    }
}