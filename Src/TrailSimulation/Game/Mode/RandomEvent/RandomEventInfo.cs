using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Randomizer event mode does not have any special information to carry around between states since it's sole purpose
    ///     in
    ///     life is to execute events and print the information before removing itself.
    /// </summary>
    public sealed class RandomEventInfo : ModeInfo
    {
        /// <summary>
        ///     Determines what event we will be firing when the random event state is attached.
        /// </summary>
        public DirectorEvent DirectorEvent { get; set; }

        /// <summary>
        ///     Determines what entity is going to be affected by the event. Example if event was illness, then source would be
        ///     person entity.
        /// </summary>
        public IEntity SourceEntity { get; set; }
    }
}