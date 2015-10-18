using TrailCommon;

namespace TrailEntities
{
    public class LandmarkModeView : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public LandmarkModeView(Vehicle vehicle) : base(vehicle)
        {
        }

        public override ModeType Mode
        {
            get { return ModeType.Landmark; }
        }
    }
}