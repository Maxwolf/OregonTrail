using System.Collections.Generic;
using TrailSimulation.Core;

namespace TrailSimulation.Game
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
        public override Mode Mode
        {
            get { return Mode.ForkInRoad; }
        }

        /// <summary>
        ///     Called after the mode has been added to list of modes and made active.
        /// </summary>
        public override void OnModePostCreate()
        {
        }
    }
}