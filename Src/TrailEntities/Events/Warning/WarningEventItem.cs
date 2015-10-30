using System;

namespace TrailEntities
{
    /// <summary>
    ///     Information that should be told to the player, does not mean the simulation should stop but more that alarms should
    ///     be going off to inform the player of critical situations before they get worse.
    /// </summary>
    public sealed class WarningEventItem : EventItem<Vehicle, WarningEvent>
    {
        /// <summary>
        ///     Create a new event item that can be passed to the simulation director.
        /// </summary>
        /// <param name="eventTarget">Simulation compatible entity which will be affected.</param>
        /// <param name="eventEnum">Enumeration that will be passed to result along with target for event execution.</param>
        public WarningEventItem(Vehicle eventTarget, WarningEvent eventEnum) : base(eventTarget, eventEnum)
        {
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventTarget">Entity which will be affected by this method.</param>
        /// <param name="eventEnum">Enumeration that helps this method determine what should be done.</param>
        protected override void OnEventExecute(Vehicle eventTarget, WarningEvent eventEnum)
        {
            throw new NotImplementedException();
        }
    }
}