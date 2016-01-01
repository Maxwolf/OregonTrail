// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:38 AM

namespace TrailSimulation.Module.Director
{
    using System;
    using Event;

    /// <summary>
    ///     Used to tag the base event item class so we can grab all inheriting types that use it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class DirectorEventAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectorEventAttribute" /> class.
        ///     Defines what type of event this will be recognized as in the director when it tells the factory to create a list of
        ///     all known events it can call and sort by.
        /// </summary>
        /// <param name="eventCategory">The event Category.</param>
        /// <param name="eventExecutionType">The event Execution Type.</param>
        public DirectorEventAttribute(
            EventCategory eventCategory,
            EventExecution eventExecutionType = EventExecution.RandomOrManual)
        {
            EventCategory = eventCategory;
            EventExecutionType = eventExecutionType;
        }

        /// <summary>
        ///     References what type of event this event is going to register as, allows for easy sorting and filtering by event
        ///     director.
        /// </summary>
        public EventCategory EventCategory { get; private set; }

        /// <summary>
        ///     Determines if this event will be selected for being chosen at random when events are fired by category and not
        ///     directly by their type.
        /// </summary>
        public EventExecution EventExecutionType { get; private set; }
    }
}