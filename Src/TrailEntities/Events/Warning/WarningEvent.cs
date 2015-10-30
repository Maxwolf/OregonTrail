namespace TrailEntities
{
    /// <summary>
    ///     Information that should be told to the player, does not mean the simulation should stop but more that alarms should
    ///     be going off to inform the player of critical situations before they get worse.
    /// </summary>
    public sealed class WarningEvent : EventItem<Vehicle, Warning>
    {
        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventTarget">Entity which will be affected by this method.</param>
        /// <param name="eventEnum">Enumeration that helps this method determine what should be done.</param>
        protected override void OnEventExecute(Vehicle eventTarget, Warning eventEnum)
        {
            throw new System.NotImplementedException();
        }
    }
}