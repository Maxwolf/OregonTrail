using TrailEntities.Entity;

namespace TrailEntities.Simulation
{
    /// <summary>
    ///     Represents an event that has occurred in the simulations past.
    /// </summary>
    public sealed class EventHistory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Simulation.EventHistory" /> class.
        /// </summary>
        public EventHistory(IEntity entity, DirectorEvent directorEvent)
        {
            // When the event happened.
            Timestamp = GameSimulationApp.Instance.Time.Date;

            // Who triggered the event.
            SourceEntity = entity;

            // What the event name and category were.
            EventName = directorEvent.Name;
            EventType = directorEvent.Type;
        }

        /// <summary>
        ///     Determines what entity in the simulation triggered the event or rolled the dice for it to happen.
        /// </summary>
        public IEntity SourceEntity { get; }

        /// <summary>
        ///     Defines what category of event this was.
        /// </summary>
        public EventType EventType { get; }

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