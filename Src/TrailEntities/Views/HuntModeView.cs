using TrailCommon;

namespace TrailEntities
{
    public class HuntModeView : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.HuntModeView" /> class.
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