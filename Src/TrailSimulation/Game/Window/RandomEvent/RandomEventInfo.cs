using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Random event Windows does not have any special information to carry around between states since it's sole purpose
    ///     in life is to execute events and print the information before removing itself.
    /// </summary>
    public sealed class RandomEventInfo : WindowData
    {
        /// <summary>
        ///     Determines what event we will be firing when the random event state is attached.
        /// </summary>
        public EventProduct DirectorEvent { get; set; }

        /// <summary>
        ///     Determines what entity is going to be affected by the event. Example if event was illness, then source would be
        ///     person entity.
        /// </summary>
        public IEntity SourceEntity { get; set; }

        /// <summary>
        ///     Defines the total number of days that need to be skipped due to an event triggering the mechanism to attach another
        ///     form and start ticking them away.
        /// </summary>
        public int DaysToSkip { get; internal set; }

        /// <summary>
        ///     Holds the rendered out event text that we got from the event about what action it performed so we can use it on
        ///     another form in the random event window.
        /// </summary>
        public string EventText { get; set; }
    }
}