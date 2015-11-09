using System.Text;

namespace TrailEntities.Event
{
    /// <summary>
    ///     Represents an event that can be triggered by the event director when vehicle is traveling along the trail.
    /// </summary>
    [DirectorEvent]
    public abstract class DirectorEventItem
    {
        /// <summary>
        ///     Creates a new event item using class activator and reflection to pass in name to constructor manually.
        /// </summary>
        /// <param name="name">Name of the event as it should be known to the simulation, used as key in list of all events.</param>
        protected DirectorEventItem(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     References what type of event this event is going to register as, allows for easy sorting and filtering by event
        ///     director.
        /// </summary>
        public abstract EventType EventType { get; }

        /// <summary>
        ///     Grabs the current name of the event as it should be known by the simulation. Generally this is the friendly class
        ///     name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        public abstract string Execute(StringBuilder eventActionDescription);
    }
}