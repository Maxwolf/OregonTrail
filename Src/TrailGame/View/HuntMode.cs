using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class HuntMode : Mode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public HuntMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override GameMode ModeType
        {
            get { return GameMode.Hunt; }
        }
    }
}