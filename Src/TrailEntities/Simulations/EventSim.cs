using System;
using System.Collections.Generic;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Numbers events and allows them to propagate through it and to other parts of the simulation. Lives inside of the
    ///     game simulation normally.
    /// </summary>
    public sealed class EventSim : IEventSimulation
    {
        /// <summary>
        ///     References all of the events that have been triggered by the system in chronological order they occurred.
        /// </summary>
        private HashSet<IEventItem> _events;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.EventSim" /> class.
        /// </summary>
        public EventSim()
        {
            _events = new HashSet<IEventItem>();
        }

        /// <summary>
        ///     Stores all previous and current events in the system, saved and loaded with simulation.
        /// </summary>
        public IEnumerable<IEventItem> Events
        {
            get { return _events; }
        }

        /// <summary>
        ///     Handles delegate for knowing when a new event has been added.
        /// </summary>
        public event EventAdd EventAdded;

        /// <summary>
        ///     Adds a new event to the list of current events.
        /// </summary>
        public void AddEvent(IEventItem eventItem)
        {
            // Adds the event to the list, no duplicate events are allowed.
            _events.Add(eventItem);

            // Fire event!
            EventAdded?.Invoke(eventItem);
        }

        public void AddEvent(object targetThing, object actionVerb, object resultNoun)
        {
            throw new NotImplementedException();
        }
    }
}