using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class HuntModeView : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public HuntModeView(Vehicle vehicle) : base(vehicle)
        {
        }

        public override ModeType Mode
        {
            get { return ModeType.Hunt; }
        }
    }
}