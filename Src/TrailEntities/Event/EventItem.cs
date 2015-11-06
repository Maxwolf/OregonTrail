using System;

namespace TrailEntities
{
    /// <summary>
    ///     Represents an event that can occur to player, vehicle, or triggered by simulations such as climate and time.
    /// </summary>
    public abstract class EventItem<EventTarget, EventEnum> : IEventItem
        where EventTarget : class, IEntity, new()
        where EventEnum : struct, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        ///     Describes what the event implementation does.
        /// </summary>
        private EventEnum _eventEnum;

        /// <summary>
        ///     Defines the target the implemented event affects.
        /// </summary>
        private EventTarget _eventTarget;

        /// <summary>
        ///     Create a new event item that can be passed to the simulation director.
        /// </summary>
        /// <param name="eventTarget">Simulation compatible entity which will be affected.</param>
        /// <param name="eventEnum">Enumeration that will be passed to result along with target for event execution.</param>
        protected EventItem(EventTarget eventTarget, EventEnum eventEnum)
        {
            // Complain the generics implemented are not of an enum type.
            if (!typeof (EventEnum).IsEnum)
            {
                throw new InvalidCastException("EventEnum generic type must be of enumeration type!");
            }

            // Complain if the event target is not based on a class.
            if (!typeof (EventTarget).IsClass)
            {
                throw new InvalidCastException("EventTarget generic type must be of class type!");
            }

            // Set data about event.
            _eventTarget = eventTarget;
            _eventEnum = eventEnum;

            // Set timestamp for when the event occurred.
            Timestamp = GameSimApp.Instance.Time.Date;
        }

        /// <summary>
        ///     Each event result has the ability to execute method.
        /// </summary>
        public void Execute()
        {
            OnEventExecute(_eventTarget, _eventEnum);
        }

        /// <summary>
        ///     Time stamp from the simulation on when this event occurred in the time line of events that make up the players
        ///     progress on the trail and game in total.
        /// </summary>
        public Date Timestamp { get; set; }

        /// <summary>
        ///     Gets and returns target entity generic type, which should be a class using IEntity interface.
        /// </summary>
        object IEventItem.TargetThing
        {
            get { return _eventTarget; }

            set { _eventTarget = value as EventTarget; }
        }

        /// <summary>
        ///     Defines a descriptive action that can be taken on the target thing.
        /// </summary>
        object IEventItem.ActionVerb
        {
            get { return _eventEnum; }

            set { _eventEnum = (EventEnum) value; }
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