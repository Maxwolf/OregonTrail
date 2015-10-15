using TrailEntities;

namespace TrailGame
{
    public class LandmarkMode : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.LandmarkWindow" /> class.
        /// </summary>
        public LandmarkMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override string Name
        {
            get { return "Landmark"; }
        }
    }
}