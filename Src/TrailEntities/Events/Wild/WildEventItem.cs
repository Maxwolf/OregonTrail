using System;

namespace TrailEntities
{
    public sealed class WildEventItem : EventItem<Vehicle, WildEvent>
    {
        /// <summary>
        ///     Create a new event item that can be passed to the simulation director.
        /// </summary>
        /// <param name="eventTarget">Simulation compatible entity which will be affected.</param>
        /// <param name="eventEnum">Enumeration that will be passed to result along with target for event execution.</param>
        public WildEventItem(Vehicle eventTarget, WildEvent eventEnum) : base(eventTarget, eventEnum)
        {
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventTarget">Entity which will be affected by this method.</param>
        /// <param name="eventEnum">Enumeration that helps this method determine what should be done.</param>
        protected override void OnEventExecute(Vehicle eventTarget, WildEvent eventEnum)
        {
            throw new NotImplementedException();
        }
    }
}