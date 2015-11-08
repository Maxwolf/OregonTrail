using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

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
        private SortedDictionary<string, Type> _events;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.EventSim" /> class.
        /// </summary>
        public EventSim()
        {
            // Create a new dictionary of events, set counter to zero for random event selector.
            _events = new SortedDictionary<string, Type>();
            PopulateEvents();

            // Test event!
            TriggerEvent("DeathPlayer");
        }

        /// <summary>
        ///     Stores all previous and current events in the system, saved and loaded with simulation.
        /// </summary>
        public IDictionary<string, Type> Events
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
            var eventTypes = GetTypesWith<RandomEventAttribute>(true);

            // Loop through all the types we got from reflection.
            foreach (var eventType in eventTypes)
            {
                // Check if the class is abstract base class, we don't want to add that.
                if (eventType.IsAbstract)
                    continue;

                // Adds the event item to the list, no duplicate events are allowed.
                if (!_events.ContainsKey(eventType.Name))
                    _events.Add(eventType.Name, eventType);
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

            // Check if the event key is empty or null.
            if (string.IsNullOrEmpty(foundEvent.Key) ||
                string.IsNullOrWhiteSpace(foundEvent.Key))
                return;

            // Check if the event value is null or default value.
            if (foundEvent.Equals(default(KeyValuePair<string, Type>)))
                return;

            Debug.Print("Executing event: " + foundEvent.Key);
            TriggerEvent(foundEvent.Key);
        }

        /// <summary>
        ///     Forcefully triggers an event that has been added to the active list by it's key.
        /// </summary>
        public void TriggerEvent(string eventName)
        {
            // Check if event name is null or empty whitespace.
            if (string.IsNullOrEmpty(eventName) ||
                string.IsNullOrWhiteSpace(eventName))
                return;

            // Check event object is null.
            if (_events[eventName] == null)
                return;

            // Get the constructor and create an instance of event item.
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var instantiatedType = Activator.CreateInstance(
                _events[eventName],
                flags,
                null,
                new object[] { _events[eventName].Name },
                CultureInfo.InvariantCulture);

            // Get the event item method by name manually.
            var eventMethod = _events[eventName].GetMethod("Execute");

            // Invoke with a parameter string builder object we will get a string back from execute method.
            var eventTUI = new StringBuilder();
            var eventValue = eventMethod.Invoke(instantiatedType, new object[] {eventTUI});
            Debug.Print("Event spat back string: " + eventValue);
        }
    }
}