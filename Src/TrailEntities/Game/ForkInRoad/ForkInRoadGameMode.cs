using System.Collections.Generic;
using TrailEntities.Mode;
using TrailEntities.Simulation.Trail;

namespace TrailEntities.Game.ForkInRoad
{
    /// <summary>
    ///     Defines a division in the trail where the player must decide about the direction they would like to keep traveling.
    ///     The choice is not optional and one of the selections must be made before the trip can continue.
    /// </summary>
    public sealed class ForkInRoadGameMode : ModeProduct
    {
        private readonly HashSet<Location> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ModeProduct" /> class.
        /// </summary>
        public ForkInRoadGameMode(IModeInfo userData, bool showCommandNamesInMenu, HashSet<Location> skipChoices)
            : base(userData, showCommandNamesInMenu)
        {
            _skipChoices = skipChoices;
        }

        public IEnumerable<Location> SkipChoices
        {
            get { return _skipChoices; }
        }

        /// <summary>
        ///     Defines the current game gameMode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameMode ModeType
        {
            get { return GameMode.ForkInRoad; }
        }
    }
}