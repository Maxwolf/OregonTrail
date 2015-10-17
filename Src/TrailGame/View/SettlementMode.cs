using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class SettlementMode : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public SettlementMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override SimulationMode Mode
        {
            get { return SimulationMode.Settlement; }
        }
    }
}