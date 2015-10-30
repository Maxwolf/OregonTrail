using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Represents an event that can occur to player, vehicle, or triggered by simulations such as climate and time.
    /// </summary>
    public abstract class EventItem<EventTarget, EventEnum, Response> : IEventItem
        where EventTarget : class, IEntity, new()
        where EventEnum : struct, IComparable, IFormattable, IConvertible
        where Response : EventResponse<EventTarget, EventEnum>, new()
    {
        /// <summary>
        ///     Describes what the event implementation does.
        /// </summary>
        private EventEnum _actionVerb;

        /// <summary>
        ///     Defines the action taken when the event is triggered.
        /// </summary>
        private Response _responseNoun;

        /// <summary>
        ///     Defines the target the implemented event affects.
        /// </summary>
        private EventTarget _targetThing;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.EventItem" /> class.
        /// </summary>
        protected EventItem()
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

            // Complain if generic noun is not a new class instance with empty constructor.
            if (!typeof (Response).IsClass)
            {
                throw new InvalidCastException("EventResponse generic type must be of class type!");
            }
        }

        /// <summary>
        ///     Create a new event item that can be passed to the simulation director.
        /// </summary>
        /// <param name="targetThing">Simulation compatible entity which will be affected.</param>
        /// <param name="eventEnum">Enumeration that will be passed to result along with target for event execution.</param>
        /// <param name="responseNoun">Class that can affect the target game entity based on event verb selection.</param>
        protected EventItem(EventTarget targetThing, EventEnum eventEnum, Response responseNoun)
        {
            // Set data about event.
            _targetThing = targetThing;
            _actionVerb = eventEnum;
            _responseNoun = responseNoun;

            // Set timestamp for when the event occurred.
            Timestamp = GameSimulationApp.Instance.Time.Date;
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
            get { return _targetThing; }

            set { _targetThing = value as EventTarget; }
        }

        /// <summary>
        ///     Defines a descriptive action that can be taken on the target thing.
        /// </summary>
        object IEventItem.ActionVerb
        {
            get { return _actionVerb; }

            set { _actionVerb = (EventEnum) value; }
        }

        /// <summary>
        ///     Defines a result that will occur when the event runs, this typically is a result on party, vehicle such as altering
        ///     health, removing items, inflicting diseases, etc.
        /// </summary>
        IEventResponse IEventItem.EventResponse
        {
            get { return _responseNoun; }
            set { _responseNoun = value as Response; }
        }
    }
}