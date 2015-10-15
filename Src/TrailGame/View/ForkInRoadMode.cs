using TrailEntities;

namespace TrailGame
{
    public class ForkInRoadMode : GameMode
    {
        private string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ForkInRoadWindow" /> class.
        /// </summary>
        public ForkInRoadMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override string Name
        {
            get { return "Fork In Road"; }
        }
    }
}