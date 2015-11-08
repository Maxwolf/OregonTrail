using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using TrailEntities.Entity;
using TrailEntities.Mode;

namespace TrailEntities.Simulation.Director
{
    /// <summary>
    ///     Numbers events and allows them to propagate through it and to other parts of the simulation. Lives inside of the
    ///     game simulation normally.
    /// </summary>
    public sealed class EventSim
    {
        /// <summary>
        ///     Fired when an event has been triggered by the director.
        /// </summary>
        public delegate void EventTriggered(IEntity simEntity, EventItem eventItem);

        /// <summary>
        ///     References all of the events that have been triggered by the system in chronological order they occurred.
        /// </summary>
        private SortedDictionary<string, EventItem> _events;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Simulation.Director.EventSim" /> class.
        /// </summary>
        public EventSim()
        {
            // Create a new dictionary of events, set counter to zero for random event selector.
            _events = new SortedDictionary<string, EventItem>();

            // Create a new list for event history.
            EventHistory = new List<EventHistoryItem>();

            // Use reflection to obtain and create list of all events the simulation will use.
            PopulateEvents();
        }

        /// <summary>
        ///     Contains history of events that have happened.
        /// </summary>
        public List<EventHistoryItem> EventHistory { get; }

        /// <summary>
        ///     Fired when an event has been triggered by the director.
        /// </summary>
        public event EventTriggered OnEventTriggered;

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

                // Get the constructor and create an instance of event item.
                var instantiatedType = Activator.CreateInstance(
                    eventType,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new object[] {eventType.Name},
                    CultureInfo.InvariantCulture);

                // Attempt to cash this instantiated type into an event item.
                var castedEventItem = instantiatedType as EventItem;
                if (castedEventItem == null)
                    continue;

                // Adds the event item to the dictionary, no duplicate events are allowed.
                if (!_events.ContainsKey(castedEventItem.Name))
                    _events.Add(castedEventItem.Name, castedEventItem);
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
        ///     Gathers all of the events by specified type and then rolls the virtual dice to determine if any of the events in
        ///     the enumeration should trigger.
        /// </summary>
        /// <param name="simEntity">Entity which will be affected by event if triggered.</param>
        /// <param name="eventType">Event type the dice will be rolled against and attempted to trigger.</param>
        public void TriggerEventByType(IEntity simEntity, EventCategory eventType)
        {
            // Create list we will use to store events of wanted type.
            var eventTypeList = new List<EventItem>();

            // Gather up all the events by the specified type.
            foreach (var valuePairEvent in _events)
            {
                if (valuePairEvent.Value.Category.Equals(eventType))
                {
                    eventTypeList.Add(valuePairEvent.Value);
                }
            }

            // Roll the virtual dice and look for event to trigger.
            var foundEvent = eventTypeList.ElementAtOrDefault(GameSimApp.Instance.Random.Next(100));
            if (foundEvent != null)
            {
                // Pass off execution to helper method that will construct event instance from type.
                TriggerEventByName(simEntity, foundEvent.Name);
            }

            // Cleanup copied event instances.
            eventTypeList.Clear();
        }

        /// <summary>
        ///     Accepts a type of class that can be used to create a given event, this way they can be referenced in code by their
        ///     actual class name which is what is used by the internal simulation event directors dictionary of available events.
        /// </summary>
        /// <param name="simEntity">Entity that would like to be tied to event being triggered.</param>
        /// <param name="eventType">Type of event that should be triggered, if it exists in loaded event dictionary in director.</param>
        public void TriggerEvent(IEntity simEntity, Type eventType)
        {
            // Pass off execution to helper method that will construct event instance from type.
            TriggerEventByName(simEntity, eventType.Name);
        }

        /// <summary>
        ///     Forcefully triggers an event that has been added to the active list by it's key.
        /// </summary>
        private void TriggerEventByName(IEntity simEntity, string eventName)
        {
            // Check if event name is null or empty whitespace.
            if (string.IsNullOrEmpty(eventName) ||
                string.IsNullOrWhiteSpace(eventName))
                return;

            // Check event object is null.
            if (_events[eventName] == null)
                return;

            // Invoke with a parameter string builder object we will get a string back from execute method.
            var eventInputParameter = new StringBuilder();
            var eventTUI = _events[eventName].Execute(eventInputParameter);

            // Check if the event text user interface from execute method is null or empty whitespace.
            if (string.IsNullOrEmpty(eventTUI) ||
                string.IsNullOrWhiteSpace(eventTUI))
                return;

            // Grab the event item,
            var eventItem = _events[eventName];

            // Attach random event game mode before triggering event since it will listen for it using event delegate.
            GameSimApp.Instance.AddMode(ModeCategory.RandomEvent);

            // Fire off event so primary game simulation knows we executed an event with an event.
            OnEventTriggered?.Invoke(simEntity, eventItem);

            // Add event to history of events we have executed.
            EventHistory.Add(new EventHistoryItem(simEntity, eventItem));
        }
    }
}