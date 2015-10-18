using TrailCommon;

namespace TrailEntities
{
    public class LandmarkMode : GameMode, ILandmark
    {
        private readonly bool _canRest;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public LandmarkMode(IGameSimulation game) : base(game)
        {
            _canRest = true;
        }

        public override ModeType Mode
        {
            get { return ModeType.Landmark; }
        }

        public bool CanRest
        {
            get { return _canRest; }
        }
    }
}