using TrailEntities;

namespace TrailGame
{
    public class SettlementMode : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.SettlementWindow" /> class.
        /// </summary>
        public SettlementMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override string Name
        {
            get { return "Settlement"; }
        }
    }
}