using System.Collections.Generic;

namespace TrailEntities
{
    /// <summary>
    ///     Numbers events and allows them to propagate through it and to other parts of the simulation. Lives inside of the
    ///     game simulation normally.
    /// </summary>
    public sealed class EventSim
    {
        /// <summary>
        ///     Reference to the last event that the dice rolled on, this is cleared and reset to next value each time the dice are
        ///     rolled.
        /// </summary>
        private int _eventCounter;

        /// <summary>
        ///     References all of the events that have been triggered by the system in chronological order they occurred.
        /// </summary>
        private OrderedDictionary<string, EventItem> _events;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.EventSim" /> class.
        /// </summary>
        public EventSim()
        {
            _events = new OrderedDictionary<string, EventItem>();
            _eventCounter = 0;
        }

        /// <summary>
        ///     Stores all previous and current events in the system, saved and loaded with simulation.
        /// </summary>
        public IDictionary<string, EventItem> Events
        {
            get { return _events; }
        }

        /// <summary>
        ///     Adds a new event to the list of current events.
        /// </summary>
        public void AddEvent(EventItem eventItem)
        {
            // Adds the event to the list, no duplicate events are allowed.
            _events.Add(eventItem.Name, eventItem);
        }

        /// <summary>
        ///     Rolls the virtual dice!
        /// </summary>
        /// <returns>Random number based on current party status.</returns>
        private int RollDice()
        {
            // TODO: Use formula to determine random number used to select event.
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
            // Check if there are any events we can process.
            if (_events.Count <= 0)
                return;

            // Roll the virtual dice!
            var diceRoll = RollDice();
            if (diceRoll > _events.Count)
            {
                _eventCounter++;
            }

            // Executes the event that the dice roll selected.
            _events[_eventCounter].Execute();
        }

        /// <summary>
        ///     Forcefully triggers an event that has been added to the active list by it's key.
        /// </summary>
        public void TriggerEvent(string eventName)
        {
            // Check if the inputted event name is null or not.
            if (string.IsNullOrEmpty(eventName) ||
                string.IsNullOrWhiteSpace(eventName))
                return;

            // Check event names for matching
        }
    }
}