using TrailSimulation.Event;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Acts like a unique identifier for each event that is to be registered in the system and defines several special
    ///     things about that we want in a key such as category, name, and if it should be random or not.
    /// </summary>
    public sealed class EventKey
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Game.EventKey" /> class.
        /// </summary>
        public EventKey(EventCategory category, string name, bool allowRandomSelectionByCategory)
        {
            AllowRandomSelectionByCategory = allowRandomSelectionByCategory;
            Category = category;
            Name = name;
        }

        /// <summary>
        ///     Category the event should fall under so when we select by type we can include this event in that selection.
        /// </summary>
        public EventCategory Category { get; }

        /// <summary>
        ///     Name of the event, typically this is the type name but it really could be anything.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Determines if this event will be selected for being chosen at random when events are fired by category and not
        ///     directly by their type.
        /// </summary>
        public bool AllowRandomSelectionByCategory { get; }
    }
}