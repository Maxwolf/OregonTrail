using System.Collections.Generic;
using TrailEntities.Simulation;

namespace TrailEntities.Mode
{
    [GameMode(ModeCategory.ForkInRoad, typeof(ForkInRoadCommands), typeof(ForkInRoadInfo))]
    public sealed class ForkInRoadGameMode : GameMode
    {
        private readonly HashSet<Location> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public ForkInRoadGameMode() : base(false)
        {
            _skipChoices = new HashSet<Location>();
        }

        public IEnumerable<Location> SkipChoices
        {
            get { return _skipChoices; }
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeCategory ModeCategory
        {
            get { return ModeCategory.ForkInRoad; }
        }
    }
}