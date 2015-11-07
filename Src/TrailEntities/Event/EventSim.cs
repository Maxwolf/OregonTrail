using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TrailEntities
{
    /// <summary>
    ///     Numbers events and allows them to propagate through it and to other parts of the simulation. Lives inside of the
    ///     game simulation normally.
    /// </summary>
    public sealed class EventSim
    {
        /// <summary>
        ///     References all of the events that have been triggered by the system in chronological order they occurred.
        /// </summary>
        private SortedDictionary<string, TravelEvent> _events;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.EventSim" /> class.
        /// </summary>
        public EventSim()
        {
            // Create a new dictionary of events, set counter to zero for random event selector.
            _events = new SortedDictionary<string, TravelEvent>();
            PopulateEvents();
        }

        /// <summary>
        ///     Stores all previous and current events in the system, saved and loaded with simulation.
        /// </summary>
        public IDictionary<string, TravelEvent> Events
        {
            get { return _events; }
        }

        /// <summary>
        ///     Using reflection get all the event item types in the project so we can dynamically add them to the list of
        ///     available events for the simulation to roll the dice against.
        /// </summary>
        private void PopulateEvents()
        {
            // Get all the types marked with the random event attribute.
            var eventTypes = GetTypesWith<TravelEventAttribute>(true);

            // Loop through all the types we got from reflection.
            foreach (var eventType in eventTypes)
            {
                // Cast type as event item and create instance of it.
                var eventItem = Activator.CreateInstance(eventType) as TravelEvent;

                // Adds the event to the list, no duplicate events are allowed.
                if (eventItem != null && !_events.ContainsKey(eventItem.Name))
                    _events.Add(eventItem.Name, eventItem);
            }
        }

        /// <summary>
        ///     Find all the classes which have a custom attribute I've defined on them, and I want to be able to find them
        ///     on-the-fly when an application is using my library.
        /// </summary>
        /// <remarks>http://stackoverflow.com/a/720171</remarks>
        private static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit)
            where TAttribute : Attribute
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                where t.IsDefined(typeof (TAttribute), inherit)
                select t;
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

            // Rolls the virtual dice and executes event if one is found that matches index.
            var foundEvent = _events.ElementAtOrDefault(GameSimApp.Instance.Random.Next(100));
            Debug.Print("Executing event: " + foundEvent.Key);
            foundEvent.Value?.Execute();
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

            // Check event names for matching that of one in our dictionary.
            if (_events[eventName] != null)
            {
                // Fire the event if we found it in the dictionary.
                _events[eventName].Execute();
            }
        }
    }
}