using TrailEntities;

namespace TrailGame
{
    public class TradeMode : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.TradeWindow" /> class.
        /// </summary>
        public TradeMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override string Name
        {
            get { return "Trading"; }
        }
    }
}