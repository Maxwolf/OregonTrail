using TrailCommon;

namespace TrailEntities
{
    public class ForkInRoadModeView : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public ForkInRoadModeView(Vehicle vehicle) : base(vehicle)
        {
        }

        public override ModeType Mode
        {
            get { return ModeType.ForkInRoad; }
        }
    }
}