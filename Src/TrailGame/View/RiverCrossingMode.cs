using TrailEntities;

namespace TrailGame
{
    public class RiverCrossingMode : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.RiverCrossingWindow" /> class.
        /// </summary>
        public RiverCrossingMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override string Name
        {
            get { return "River Crossing"; }
        }
    }
}