using TrailEntities;

namespace TrailGame
{
    public class HuntMode : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.HuntWindow" /> class.
        /// </summary>
        public HuntMode(Vehicle vehicle) : base(vehicle)
        {
        }

        public override string Name
        {
            get { return "Hunting"; }
        }
    }
}