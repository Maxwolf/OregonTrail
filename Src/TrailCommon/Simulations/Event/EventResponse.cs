using System;

namespace TrailCommon
{
    /// <summary>
    ///     Processes information required for an event to affect simulation entities by called a special method we know every
    ///     event result will contain because of the interface they all share.
    /// </summary>
    /// <typeparam name="EventTarget">Target simulation entity which this entity result is going to directly affect.</typeparam>
    /// <typeparam name="EventEnum">Event enumeration will determine action executed on target simulation entity.</typeparam>
    public abstract class EventResponse<EventTarget, EventEnum> : IEventResponse
        where EventTarget : class, IEntity, new()
        where EventEnum : struct, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        ///     Reference to enumeration related to type of event this is which helps execution method determine what action to
        ///     take on target entity.
        /// </summary>
        private EventEnum _eventEnum;

        /// <summary>
        ///     Reference to the entity which this event result will have direct influence over.
        /// </summary>
        private EventTarget _eventTarget;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.EventResponse" /> class.
        /// </summary>
        protected EventResponse()
        {
            // Complain the generics implemented are not of an enum type.
            if (!typeof (EventEnum).IsEnum)
            {
                throw new InvalidCastException("VerbAction generic type must be of enumeration type!");
            }

            // Complain if the event target is not based on a class.
            if (!typeof (EventTarget).IsClass)
            {
                throw new InvalidCastException("EventTarget generic type must be of class type!");
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.EventResponse" /> class.
        /// </summary>
        protected EventResponse(EventTarget eventTarget, EventEnum eventEnum)
        {
            _eventTarget = eventTarget;
            _eventEnum = eventEnum;
        }

        /// <summary>
        ///     Each event result has the ability to execute method.
        /// </summary>
        public void Execute()
        {
            OnEventExecute(_eventTarget, _eventEnum);
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventTarget">Entity which will be affected by this method.</param>
        /// <param name="eventEnum">Enumeration that helps this method determine what should be done.</param>
        protected abstract void OnEventExecute(EventTarget eventTarget, EventEnum eventEnum);
    }
}