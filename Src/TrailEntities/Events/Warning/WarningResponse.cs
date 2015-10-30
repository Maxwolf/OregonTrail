using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Processes warning information and attaches game mode with correct state so we can both stop the simulation and
    ///     inform the player about whatever it is we need to warn them about.
    /// </summary>
    public sealed class WarningResponse : EventResponse<Vehicle, Warning>
    {
        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventTarget">Entity which will be affected by this method.</param>
        /// <param name="eventEnum">Enumeration that helps this method determine what should be done.</param>
        protected override void OnEventExecute(Vehicle eventTarget, Warning eventEnum)
        {
            throw new NotImplementedException();
        }
    }
}