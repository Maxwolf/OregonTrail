using System.Text;

namespace TrailEntities.Event
{
    /// <summary>
    ///     Party leader has died! This will end the entire simulation since the others cannot go on without the leader.
    /// </summary>
    public sealed class DeathPlayerEvent : DirectorEventItem
    {
        /// <summary>
        ///     Creates a new event item using class activator and reflection to pass in name to constructor manually.
        /// </summary>
        /// <param name="name">Name of the event as it should be known to the simulation, used as key in list of all events.</param>
        public DeathPlayerEvent(string name) : base(name)
        {
        }

        /// <summary>
        ///     References what type of event this event is going to register as, allows for easy sorting and filtering by event
        ///     director.
        /// </summary>
        public override EventCategory Category
        {
            get { return EventCategory.Person; }
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        public override string Execute(StringBuilder eventActionDescription)
        {
            eventActionDescription.Append("You have died");
            return eventActionDescription.ToString();
        }
    }
}