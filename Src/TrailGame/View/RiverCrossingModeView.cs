using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class RiverCrossingModeView : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public RiverCrossingModeView(Vehicle vehicle) : base(vehicle)
        {
        }

        public override ModeType Mode
        {
            get { return ModeType.RiverCrossing; }
        }
    }
}