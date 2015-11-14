using System.Collections.Generic;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    public sealed class ForkInRoadMode : ModeProduct<ForkInRoadCommands, ForkInRoadInfo>
    {
        private readonly HashSet<Location> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ModeProduct" /> class.
        /// </summary>
        public ForkInRoadMode(ForkInRoadInfo userData) : base(userData)
        {
            // TODO: Get skip choices from user data.
            //_skipChoices = skipChoices;
        }

        public IEnumerable<Location> SkipChoices
        {
            get { return _skipChoices; }
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameMode GameMode
        {
            get { return GameMode.ForkInRoad; }
        }
    }
}