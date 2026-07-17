// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using OregonTrailDotNet.Event;
using WolfCurses.Utility;

namespace OregonTrailDotNet.Module.Director
{
    /// <summary>
    ///     Factory pattern for creating director event items from type references.
    /// </summary>
    public sealed class EventFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrailDotNet.Module.Director.EventFactory" /> class.
        /// </summary>
        public EventFactory()
        {
            // Create dictionaries for storing event reference types, history of execution, and execution count.
            EventReference = new Dictionary<EventKey, Type>();

            // Collect all of the event types with the attribute decorated on them.
            var randomEvents = AttributeExtensions.GetTypesWith<DirectorEventAttribute>(true);
            foreach (var eventObject in randomEvents)
            {
                // Check if the class is abstract base class, we don't want to add that.
                if (eventObject.GetTypeInfo().IsAbstract)
                    continue;

                // Check the attribute itself from the event we are working on, which gives us the event type enum.
                var eventAttribute = eventObject.GetTypeInfo().GetAttributes<DirectorEventAttribute>(true).First();
                var eventType = eventAttribute.EventCategory;

                // Initialize the execution history dictionary with every event type.
                foreach (var modeType in Enum.GetValues(typeof(EventCategoryEnum)))
                {
                    // Only proceed if enum value matches attribute enum value for event type.
                    if (!modeType.Equals(eventType))
                        continue;

                    // Create key for the event execution counter.
                    var eventKey = new EventKey((EventCategoryEnum) modeType, eventObject.Name,
                        eventAttribute.EventExecutionType);

                    // Reference type for creating instances.
                    if (!EventReference.ContainsKey(eventKey))
                        EventReference.Add(eventKey, eventObject);
                }
            }
        }

        /// <summary>
        ///     References all of the events that have been triggered by the system in chronological order they occurred.
        /// </summary>
        private Dictionary<EventKey, Type> EventReference { get; }

        /// <summary>Creates a new event based on system type which we keep track of in dictionary of event references.</summary>
        /// <param name="eventType">The type of event which we should create an instance of.</param>
        /// <returns>Instance of event type given in parameter.</returns>
        internal EventProduct CreateInstance(Type eventType)
        {
            // Check if event type exists in reference dictionary.
            if (!EventReference.ContainsValue(eventType))
                throw new ArgumentException(
                    $"Attempted to create instance of {eventType.Name} without any known reference to it in event factory! " +
                    "Perhaps you are missing the [DirectorEvent()] attribute.");

            // Grab the key value pair from event references that matches inputted type via equality reference.
            var directorEventKeyValuePair = EventReference.FirstOrDefault(x => x.Value == eventType);

            // Check if the class is abstract base class, we don't want to add that.
            if (directorEventKeyValuePair.Value.GetTypeInfo().IsAbstract)
                return null;

            // Create the event product, but don't call any constructor. WolfCurses FactoryExtensions
            // cannot do this on modern .NET since FormatterServices was removed from the runtime.
            var eventInstance =
                RuntimeHelpers.GetUninitializedObject(directorEventKeyValuePair.Value) as
                    EventProduct;

            // If the event instance is null then complain.
            if (eventInstance == null)
                throw new ArgumentException($"Attempted to create instance of {eventType} event but failed!");

            // Fire event that acts like our own constructor for the object but only calling it when we say here.
            eventInstance.OnEventCreate();

            // Increment the history for loading this type of event.
            return eventInstance;
        }

        /// <summary>
        ///     The [DirectorEvent] attribute an event type was tagged with.
        /// </summary>
        /// <param name="eventType">Event type to read.</param>
        /// <returns>The attribute describing how the director should treat this event.</returns>
        private static DirectorEventAttribute AttributeOf(Type eventType)
        {
            return eventType.GetTypeInfo().GetAttributes<DirectorEventAttribute>(true).First();
        }

        /// <summary>
        ///     Every event in a category whose real daily odds are known, paired with those odds. These are rolled one by one
        ///     each day instead of competing for the category's single roll, because their frequency is a fact about the game
        ///     rather than a matter of taste - wild fruit really is found on about one summer day in twenty-five.
        /// </summary>
        /// <param name="eventCategory">Category being rolled.</param>
        /// <returns>Event types and their chance out of a hundred per day.</returns>
        public IEnumerable<Tuple<Type, double>> EventsWithOwnOdds(EventCategoryEnum eventCategory)
        {
            var found = new List<Tuple<Type, double>>();
            foreach (var type in EventReference)
            {
                if (!type.Key.Category.Equals(eventCategory) ||
                    (type.Key.ExecutionType != EventExecutionEnum.RandomOrManual))
                    continue;

                var attribute = AttributeOf(type.Value);
                if (attribute.HasOwnOdds)
                    found.Add(new Tuple<Type, double>(type.Value, attribute.DailyChance));
            }

            return found;
        }

        /// <summary>Gathers all of the events by specified type and picks one of them at random to return.</summary>
        /// <param name="eventCategory">Enum value of the type of event such as medical, person, vehicle, etc.</param>
        /// <returns>Created event product based on enum value.</returns>
        public EventProduct CreateRandomByType(EventCategoryEnum eventCategory)
        {
            // Query all of the reference event types that match the given enumeration value. Events that carry their own
            // daily odds are excluded: they are rolled separately every day and would otherwise get two bites at happening.
            var groupedEventList = new List<Type>();
            foreach (var type in EventReference)
                if (type.Key.Category.Equals(eventCategory) &&
                    (type.Key.ExecutionType == EventExecutionEnum.RandomOrManual) &&
                    !AttributeOf(type.Value).HasOwnOdds)
                    groupedEventList.Add(type.Value);

            // Check to make sure there is at least one type of event of this type.
            if (groupedEventList.Count <= 0)
                return null;

            // Weight each event by the probability declared on its [DirectorEvent] attribute and lay them out on a cumulative
            // number line. A single roll then selects the event whose slice the roll lands in, so events with a higher declared
            // probability occupy a wider slice and fire more often. Successive numbers determine the odds (e.g. 0-6=eventA,
            // 6-11=eventB, and so on) exactly like the original game's probability table.
            var weights = new List<int>(groupedEventList.Count);
            var totalWeight = 0;
            foreach (var type in groupedEventList)
            {
                var weight = type.GetTypeInfo().GetAttributes<DirectorEventAttribute>(true).First().EventProbability;
                weights.Add(weight);
                totalWeight += weight;
            }

            // Fallback to uniform selection if every event in this category declared a zero weight.
            Type selectedType;
            if (totalWeight <= 0)
            {
                selectedType = groupedEventList[GameSimulationApp.Instance.Random.Next(groupedEventList.Count)];
            }
            else
            {
                var diceRoll = GameSimulationApp.Instance.Random.Next(1, totalWeight + 1);
                var cumulative = 0;
                selectedType = groupedEventList[groupedEventList.Count - 1];
                for (var i = 0; i < groupedEventList.Count; i++)
                {
                    cumulative += weights[i];
                    if (diceRoll > cumulative)
                        continue;

                    selectedType = groupedEventList[i];
                    break;
                }
            }

            // Create the event we decided to execute from these types of event types.
            var randomEvent = CreateInstance(selectedType);

            // Clear the temporary list we made to get by category and return create event instance.
            groupedEventList.Clear();
            return randomEvent;
        }
    }
}