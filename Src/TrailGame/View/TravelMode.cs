using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class TravelMode : Mode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.TravelMode" /> class.
        /// </summary>
        public TravelMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override TrailCommon.GameMode ModeType
        {
            get { return TrailCommon.GameMode.Travel; }
        }
    }
}