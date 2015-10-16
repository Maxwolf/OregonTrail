using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class ForkInRoadMode : TrailMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public ForkInRoadMode(TrailVehicle trailVehicle) : base(trailVehicle)
        {
        }

        public override TrailModeType Mode
        {
            get { return TrailModeType.ForkInRoad; }
        }
    }
}