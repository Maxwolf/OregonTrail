using System.Collections.Generic;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    public sealed class ForkInRoadMode : ModeProduct<ForkInRoadCommands, ForkInRoadInfo>
    {
        private readonly HashSet<Location> _skipChoices;

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