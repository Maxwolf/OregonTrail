using TrailCommon;

namespace TrailEntities
{
    public class TradeModel : Trade
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Trade" /> class.
        /// </summary>
        public TradeModel(IVehicle vehicle) : base(vehicle)
        {
        }
    }
}