using System;
using TrailSimulation.Event;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Used to tag the base event item class so we can grab all inheriting types that use it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class DirectorEventAttribute : Attribute
    {
        /// <summary>
        ///     Defines what type of event this will be recognized as in the director when it tells the factory to create a list of
        ///     all known events it can call and sort by.
        /// </summary>
        public DirectorEventAttribute(EventCategory eventCategory)
        {
            EventCategory = eventCategory;
        }

        /// <summary>
        ///     References what type of event this event is going to register as, allows for easy sorting and filtering by event
        ///     director.
        /// </summary>
        public EventCategory EventCategory { get; private set; }
    }
}