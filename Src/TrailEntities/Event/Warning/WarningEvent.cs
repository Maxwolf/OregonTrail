using TrailEntities.Game;

namespace TrailEntities.Event
{
    [EventDirector(EventType.Warning)]
    public abstract class WarningEvent : DirectorEvent
    {
        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="type">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        protected WarningEvent(EventType type) : base(type)
        {
        }
    }
}