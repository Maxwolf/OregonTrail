using System.Text;
using TrailEntities.Simulation.Director;

namespace TrailEntities.Event
{
    /// <summary>
    ///     Called when one of your party members dies that is not the leader of the group, the game will still be able to
    ///     continue without this person.
    /// </summary>
    public sealed class DeathCompanion : EventItem
    {
        /// <summary>
        ///     Creates a new event item using class activator and reflection to pass in name to constructor manually.
        /// </summary>
        /// <param name="name">Name of the event as it should be known to the simulation, used as key in list of all events.</param>
        public DeathCompanion(string name) : base(name)
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
            eventActionDescription.Append("Party member has died");
            return eventActionDescription.ToString();
        }
    }
}