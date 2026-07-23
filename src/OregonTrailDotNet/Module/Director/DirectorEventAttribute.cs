// Created by Maxwolf (bigmaxwolf.com) 
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
        /// <param name="dailyChance">
        ///     Odds out of a hundred that this event happens on any given day, for the handful of events whose real frequency
        ///     is known rather than merely relative. An event that declares this is rolled on its own each day and takes no
        ///     part in the shared category roll, which is how the original worked: it kept a table of per-event daily odds
        ///     and walked the whole of it every day rather than picking one event out of a hat. Zero (the default) leaves the
        ///     event in the hat with everything else.
        /// </param>
        public DirectorEventAttribute(
            EventCategoryEnum eventCategory,
            EventExecutionEnum eventExecutionType = EventExecutionEnum.RandomOrManual,
            int eventProbability = 100,
            double dailyChance = 0)
        {
            EventCategory = eventCategory;
            EventExecutionType = eventExecutionType;
            EventProbability = eventProbability < 0 ? 0 : eventProbability;
            DailyChance = dailyChance < 0 ? 0 : dailyChance;
        }

        /// <summary>
        ///     Odds out of a hundred that this event happens on any given day, or zero if it takes its chances in the shared
        ///     category roll like most events do.
        /// </summary>
        public double DailyChance { get; }

        /// <summary>
        ///     Whether this event is rolled on its own each day rather than competing for the category's single roll.
        /// </summary>
        public bool HasOwnOdds => DailyChance > 0;

        /// <summary>
        ///     References what type of event this event is going to register as, allows for easy sorting and filtering by event
        ///     director.
        /// </summary>
        public EventCategoryEnum EventCategory { get; }

        /// <summary>
        ///     Determines if this event will be selected for being chosen at random when events are fired by category and not
        ///     directly by their type.
        /// </summary>
        public EventExecutionEnum EventExecutionType { get; }

        /// <summary>
        ///     Relative weight used by the event factory when picking a random event from a category. Successive events are laid
        ///     out on a cumulative number line and a single roll selects one, so a larger value means the event occupies a wider
        ///     slice and fires more often. Defaults to 100 (all events equally weighted).
        /// </summary>
        public int EventProbability { get; }
    }
}