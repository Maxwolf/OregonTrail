using System.Collections.Generic;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    public sealed class ForkInRoadMode : ModeProduct<ForkInRoadCommands>
    {
        private readonly HashSet<Location> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ModeProduct" /> class.
        /// </summary>
        public ForkInRoadMode() : base(false)
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
        public override ModeType ModeType
        {
            get { return ModeType.ForkInRoad; }
        }
    }
}