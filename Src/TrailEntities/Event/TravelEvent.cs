using System;
using System.Diagnostics.CodeAnalysis;

namespace TrailEntities
{
    /// <summary>
    ///     Represents an event that can occur to player, vehicle, or triggered by simulations such as climate and time.
    /// </summary>
    [TravelEvent]
    public sealed class TravelEvent
    {
        /// <summary>
        ///     Defines what type of event this will be. The rest is left up to implementation.
        /// </summary>
        public TravelEvent(EventCategory eventType, string name, Action eventAction) : this()
        {
            EventType = eventType;
            Name = name;
            EventAction = eventAction;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.TravelEvent" /> class.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public TravelEvent()
        {
            // Required for activator to be able to correctly create event item classes.
            EventType = EventCategory.Unknown;
        }

        /// <summary>
        ///     Time stamp from the simulation on when this event occurred in the time line of events that make up the players
        ///     progress on the trail and game in total.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public Date Timestamp { get; private set; }

        /// <summary>
        ///     Returns the type of event this is, typically this is set per implementation. Used by simulation to sort and quickly
        ///     access events of a certain type when dice are rolled.
        /// </summary>
        private EventCategory EventType { get; }

        /// <summary>
        ///     Used as a display name for the event, typically used to define what the action will do.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Actual action that will be run when the event is executed by the director random event selector.
        /// </summary>
        private Action EventAction { get; }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        public void Execute()
        {
            // Set timestamp for when the event occurred.
            Timestamp = GameSimApp.Instance.Time.Date;

            // Execute the event action.
            EventAction.Invoke();
        }
    }
}