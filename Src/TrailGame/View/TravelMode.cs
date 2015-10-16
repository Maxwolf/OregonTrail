using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class TravelMode : TrailMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.TravelMode" /> class.
        /// </summary>
        public TravelMode(TrailVehicle trailVehicle) : base(trailVehicle)
        {
        }

        public override TrailModeType Mode
        {
            get { return TrailModeType.Travel; }
        }
    }
}