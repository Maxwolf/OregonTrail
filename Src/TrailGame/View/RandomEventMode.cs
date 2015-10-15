using TrailEntities;

namespace TrailGame
{
    public class RandomEventMode : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.RandomEventWindow" /> class.
        /// </summary>
        public RandomEventMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override string Name
        {
            get { return "Random Event"; }
        }
    }
}