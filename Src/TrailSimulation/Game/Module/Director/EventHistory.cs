using TrailSimulation.Entity;
using TrailSimulation.Event;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Represents an event that has occurred in the simulations past.
    /// </summary>
    public sealed class EventHistory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Game.EventHistory" /> class.
        /// </summary>
        public EventHistory(IEntity entity, EventProduct directorEvent)
        {
            // When the event happened.
            Timestamp = GameSimulationApp.Instance.Time.Date;

            // Who triggered the event.
            SourceEntity = entity;

            // What the event name and type were.
            EventName = directorEvent.Name;
            EventCategory = directorEvent.Category;
        }

        /// <summary>
        ///     Determines what entity in the simulation triggered the event or rolled the dice for it to happen.
        /// </summary>
        public IEntity SourceEntity { get; }

        /// <summary>
        ///     Defines what type of event this was.
        /// </summary>
        public EventCategory EventCategory { get; }

        /// <summary>
        ///     Holds the name of the event that was fired.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        ///     Defines when the event actually took place.
        /// </summary>
        public Date Timestamp { get; }
    }
}