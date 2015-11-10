using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TrailEntities.Widget;

namespace TrailEntities.Simulation
{
    /// <summary>
    ///     Factory pattern for creating director event items from type references.
    /// </summary>
    public sealed class EventFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Simulation.EventFactory" /> class.
        /// </summary>
        public EventFactory()
        {
            // Create dictionaries for storing event reference types, history of execution, and execution count.
            EventReference = new Dictionary<Tuple<EventType, string>, Type>();
            EventHistory = new List<EventHistory>();
            ExecutionCount = new Dictionary<Tuple<EventType, string>, int>();

            // Collect all of the event types with the attribute decorated on them.
            var randomEvents = AttributeHelper.GetTypesWith<EventDirectorAttribute>(false);
            foreach (var eventObject in randomEvents)
            {
                // Get the attribute itself from the event we are working on, which gives us the event type enum.
                var eventAttribute = eventObject.GetAttributes<EventDirectorAttribute>(false).First();
                var eventType = eventAttribute.EventType;

                // Initialize the execution history dictionary with every event type.
                foreach (var modeType in Enum.GetValues(typeof (EventType)))
                {
                    // Only proceed if enum value matches attribute enum value for event type.
                    if (!modeType.Equals(eventType))
                        continue;

                    // Create key for the event execution counter.
                    var tupleKey = new Tuple<EventType, string>((EventType) modeType, eventObject.Name);

                    // Execution counter by name and type.
                    if (!ExecutionCount.ContainsKey(tupleKey))
                        ExecutionCount.Add(tupleKey, 0);

                    // Reference type for creating instances.
                    if (!EventReference.ContainsKey(tupleKey))
                        EventReference.Add(tupleKey, eventObject);
                }
            }
        }

        /// <summary>
        ///     References all of the events that have been triggered by the system in chronological order they occurred.
        /// </summary>
        private Dictionary<Tuple<EventType, string>, Type> EventReference { get; }

        /// <summary>
        ///     Contains history of events that have happened.
        /// </summary>
        public List<EventHistory> EventHistory { get; }

        /// <summary>
        ///     Keeps track of all the different times that a particular type of event has been run.
        /// </summary>
        public Dictionary<Tuple<EventType, string>, int> ExecutionCount { get; }

        /// <summary>
        ///     Creates a new event based on system type which we keep track of in dictionary of event references.
        /// </summary>
        /// <param name="eventType">The type of event which we should create an instance of.</param>
        /// <returns>Instance of event type given in parameter.</returns>
        internal DirectorEvent CreateInstance(Type eventType)
        {
            // Check if event type exists in reference dictionary.
            if (!EventReference.ContainsValue(eventType))
                throw new ArgumentException(
                    $"Attempted to create instance of {eventType.Name} without any known reference to it in event factory! " +
                    $"Perhaps you are missing an attribute to define the event for reflection to correctly reference it.");

            // Grab the key value pair from event references that matches inputted type via equality reference.
            var directorEventKeyValuePair = EventReference.FirstOrDefault(x => (x.Value == eventType));

            // Check if the class is abstract base class, we don't want to add that.
            if (directorEventKeyValuePair.Value.IsAbstract)
                return null;

            // Create the game mode, it will have parameterless constructor.
            var eventInstance = Activator.CreateInstance(
                directorEventKeyValuePair.Value,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[] {directorEventKeyValuePair.Key.Item1}, // Constructor with one parameter.
                CultureInfo.InvariantCulture) as DirectorEvent;

            // If the event instance is null then complain.
            if (eventInstance == null)
                throw new ArgumentException($"Attempted to create instance of {eventType} event but failed!");

            // Create key for the event execution counter.
            var tupleKey = new Tuple<EventType, string>(directorEventKeyValuePair.Key.Item1, eventInstance.Name);

            // Increment the history for loading this type of event.
            ExecutionCount[tupleKey]++;
            return eventInstance;
        }

        /// <summary>
        ///     Gathers all of the events by specified type and picks one of them at random to return.
        /// </summary>
        /// <param name="eventType">Enum value of the type of event such as medical, person, vehicle, etc.</param>
        /// <returns>Created event product based on enum value.</returns>
        public DirectorEvent CreateRandomByType(EventType eventType)
        {
            // Find all of the reference event types that match the given enumeration value.
            var groupedEvents = EventReference.Select(pair => pair.Key.Item1.Equals(eventType));

            // Roll the dice against the event reference ceiling count to see which one we use.
            var diceRoll = GameSimulationApp.Instance.Random.Next(EventReference.Count);
            var randomEventTypeByType = groupedEvents.ElementAt(diceRoll).GetType();

            // Create the event we decided to execute from these types of event types.
            var randomEvent = CreateInstance(randomEventTypeByType);
            return randomEvent;
        }
    }
}