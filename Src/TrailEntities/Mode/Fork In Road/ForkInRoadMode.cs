using System.Collections.Generic;

namespace TrailEntities
{
    public sealed class ForkInRoadMode : GameMode<ForkInRoadCommands>, IForkInRoadMode
    {
        private readonly HashSet<PointOfInterest> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public ForkInRoadMode() : base(false)
        {
            _skipChoices = new HashSet<PointOfInterest>();
        }

        public IEnumerable<PointOfInterest> SkipChoices
        {
            get { return _skipChoices; }
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.ForkInRoad; }
        }
    }
}