using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class RandomEventMode : TrailMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public RandomEventMode(TrailVehicle trailVehicle) : base(trailVehicle)
        {
        }

        public override TrailModeType Mode
        {
            get { return TrailModeType.RandomEvent; }
        }
    }
}