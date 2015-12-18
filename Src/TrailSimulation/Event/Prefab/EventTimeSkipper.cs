using System.Text;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Prefab for an event that will be able to skip time up to a set amount of days. The purpose of which is to
    ///     accommodate events which force the player to lose time. When this happens the simulation ticks each of those days,
    ///     allowing any events such as people starving to death and wild animals occur as they normally would.
    /// </summary>
    public abstract class EventTimeSkipper : EventProduct
    {
        /// <summary>
        ///     String builder that will hold all the data from event execution.
        /// </summary>
        private StringBuilder _eventText;

        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="category">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        protected EventTimeSkipper(EventCategory category) : base(category)
        {
            // Create the string builder that will hold representation of event action to display for debugging.
            _eventText = new StringBuilder();
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="sourceEntity">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(IEntity sourceEntity)
        {
            // Clear out the text from the string builder.
            _eventText.Clear();


        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(IEntity sourceEntity)
        {
            return _eventText.ToString();
        }

        /// <summary>
        ///     Fired when the event is closed by the user or system after being executed and rendered out on text user interface.
        /// </summary>
        public override void OnEventClose()
        {
            base.OnEventClose();


        }
    }
}