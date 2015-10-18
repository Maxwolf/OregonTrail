using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class TravelModeView : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.TravelMode" /> class.
        /// </summary>
        public TravelModeView(Vehicle vehicle) : base(vehicle)
        {
        }

        public override ModeType Mode
        {
            get { return ModeType.Travel; }
        }
    }
}