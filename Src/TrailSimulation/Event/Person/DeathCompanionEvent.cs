using System;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Called when one of your party members dies that is not the leader of the group, the game will still be able to
    ///     continue without this person.
    /// </summary>
    public sealed class DeathCompanionEvent : PersonEvent
    {
        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="category">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        public DeathCompanionEvent(EventCategory category) : base(category)
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