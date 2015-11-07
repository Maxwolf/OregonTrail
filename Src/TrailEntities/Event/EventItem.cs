namespace TrailEntities
{
    /// <summary>
    ///     Represents an event that can occur to player, vehicle, or triggered by simulations such as climate and time.
    /// </summary>
    public abstract class EventItem
    {
        /// <summary>
        ///     Create a new event item that can be passed to the simulation director.
        /// </summary>
        protected EventItem()
        {
            // Set timestamp for when the event occurred.
            Timestamp = GameSimApp.Instance.Time.Date;
        }

        /// <summary>
        ///     Time stamp from the simulation on when this event occurred in the time line of events that make up the players
        ///     progress on the trail and game in total.
        /// </summary>
        public Date Timestamp { get; }

        /// <summary>
        ///     Returns the friendly class name of the event.
        /// </summary>
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        ///     Each event result has the ability to execute method.
        /// </summary>
        public void Execute()
        {
            OnEventExecute();
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        protected abstract void OnEventExecute();
    }
}