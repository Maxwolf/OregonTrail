using TrailSimulation.Entity;
using TrailSimulation.Event;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Represents an event that can be triggered by the event director when vehicle is traveling along the trail.
    /// </summary>
    public abstract class EventProduct
    {
        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="category">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        protected EventProduct(EventCategory category)
        {
            Category = category;
        }

        /// <summary>
        ///     Defines what type of event this will be, used for grouping and filtering and triggering events by type rather than
        ///     type of.
        /// </summary>
        public EventCategory Category { get; }

        /// <summary>
        ///     Grabs the current name of the event as it should be known by the simulation. Generally this is the friendly class
        ///     name.
        /// </summary>
        public string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="sourceEntity">
        ///     Entity which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public abstract void Execute(IEntity sourceEntity);

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        public string Render()
        {
            return OnRender();
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected abstract string OnRender();
    }
}