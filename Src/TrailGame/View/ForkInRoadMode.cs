using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class ForkInRoadMode : Mode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public ForkInRoadMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override GameMode ModeType
        {
            get { return GameMode.ForkInRoad; }
        }
    }
}