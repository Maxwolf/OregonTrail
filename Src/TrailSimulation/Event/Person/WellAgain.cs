using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Makes the person whom the event was fired on no loner afflicted by any illness.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    public sealed class WellAgain : EventProduct
    {
        /// <summary>
        ///     Person whom is all better now and no longer having illness infecting them.
        /// </summary>
        private Person _sourceEntity;

        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="category">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        public WellAgain(EventCategory category) : base(category)
        {
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
            // Cast the source entity as a player.
            _sourceEntity = sourceEntity as Person;

            // Remove the infection if any exists.
            if (_sourceEntity != null)
                _sourceEntity.Infection = Disease.None;
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender()
        {
            return $"{_sourceEntity.Name} is well again.";
        }
    }
}