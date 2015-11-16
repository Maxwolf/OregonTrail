using System;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Party leader has died! This will end the entire simulation since the others cannot go on without the leader.
    /// </summary>
    public sealed class DeathPlayerEvent : PersonEvent
    {
        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="type">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        public DeathPlayerEvent(EventType type) : base(type)
        {
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        public override void Execute()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender()
        {
            throw new NotImplementedException();
        }
    }
}