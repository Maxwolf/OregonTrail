using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class RandomEventMode : Mode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public RandomEventMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override GameMode ModeType
        {
            get { return GameMode.RandomEvent; }
        }
    }
}