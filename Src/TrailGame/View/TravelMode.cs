using TrailEntities;

namespace TrailGame
{
    public class TravelMode : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.TravelMode" /> class.
        /// </summary>
        public TravelMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override string Name
        {
            get { return "Traveling"; }
        }
    }
}