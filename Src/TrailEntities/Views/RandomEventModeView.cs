using TrailCommon;

namespace TrailEntities
{
    public class RandomEventModeView : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public RandomEventModeView(Vehicle vehicle) : base(vehicle)
        {
        }

        public override ModeType Mode
        {
            get { return ModeType.RandomEvent; }
        }
    }
}