using System;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Used to tag the base event item class so we can grab all inheriting types that use it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class EventDirectorAttribute : Attribute
    {
        /// <summary>
        ///     Defines what type of event this will be recognized as in the director when it tells the factory to create a list of
        ///     all known events it can call and sort by.
        /// </summary>
        public EventDirectorAttribute(EventType eventType)
        {
            EventType = eventType;
        }

        /// <summary>
        ///     References what type of event this event is going to register as, allows for easy sorting and filtering by event
        ///     director.
        /// </summary>
        public EventType EventType { get; private set; }
    }
}