using System.Collections.Generic;

namespace TrailEntities
{
    /// <summary>
    ///     Numbers events and allows them to propagate through it and to other parts of the simulation. Lives inside of the
    ///     game simulation normally.
    /// </summary>
    public sealed class EventSim
    {
        public delegate void EventAdd(IEventItem eventItem);

        /// <summary>
        /// Reference to the last event that the dice rolled on, this is cleared and reset to next value each time the dice are rolled.
        /// </summary>
        private int _eventCounter;

        /// <summary>
        ///     References all of the events that have been triggered by the system in chronological order they occurred.
        /// </summary>
        private List<IEventItem> _events;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.EventSim" /> class.
        /// </summary>
        public EventSim()
        {
            _events = new List<IEventItem>();
            _eventCounter = 0;
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

        private int RollDice()
        {
            _eventCounter = 0;
            _eventCounter++;
            return GameSimApp.Instance.Random.Next(100);
        }

        /// <summary>
        ///     Loops through all of the registered events with the director and begins rolling the virtual dice to see if any of
        ///     them trigger.
        /// </summary>
        public void CheckRandomEvents()
        {
            // Roll the virtual dice!
            var diceRoll = RollDice();
            if (diceRoll > _events.Count)
            {
                _eventCounter++;
            }

            // Executes the event that the dice roll selected.
            _events[_eventCounter].Execute();
        }
    }
}