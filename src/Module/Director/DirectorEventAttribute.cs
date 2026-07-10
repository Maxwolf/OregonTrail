// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using OregonTrailDotNet.Event;

namespace OregonTrailDotNet.Module.Director
{
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
        /// <param name="eventProbability">
        ///     Relative weight used when the director picks a random event from a category. Higher numbers make the event more
        ///     likely; the default of 100 keeps every event equally likely (preserving the original uniform behavior). Negative
        ///     values are clamped to zero so a mis-tagged event simply never rolls.
        /// </param>
        public DirectorEventAttribute(
            EventCategory eventCategory,
            EventExecution eventExecutionType = EventExecution.RandomOrManual,
            int eventProbability = 100)
        {
            EventCategory = eventCategory;
            EventExecutionType = eventExecutionType;
            EventProbability = eventProbability < 0 ? 0 : eventProbability;
        }

        /// <summary>
        ///     References what type of event this event is going to register as, allows for easy sorting and filtering by event
        ///     director.
        /// </summary>
        public EventCategory EventCategory { get; }

        /// <summary>
        ///     Determines if this event will be selected for being chosen at random when events are fired by category and not
        ///     directly by their type.
        /// </summary>
        public EventExecution EventExecutionType { get; }

        /// <summary>
        ///     Relative weight used by the event factory when picking a random event from a category. Successive events are laid
        ///     out on a cumulative number line and a single roll selects one, so a larger value means the event occupies a wider
        ///     slice and fires more often. Defaults to 100 (all events equally weighted).
        /// </summary>
        public int EventProbability { get; }
    }
}