namespace TrailEntities
{
    /// <summary>
    ///     Contains a bunch of predefined events that are used in the simulation. All of these objects are loaded into the
    ///     event director on startup to be used during random event selection while traveling the trail.
    /// </summary>
    public static class Events
    {
        /// <summary>
        ///     Complete listing of all the random events that will be loaded into the event director on startup.
        /// </summary>
        internal static IOrderedDictionary<string, EventItem> RandomEvents
        {
            get
            {
                var defaultEvents = new OrderedDictionary<string, EventItem>
                {
                    {"", null},
                    {"", null}
                };
                return defaultEvents;
            }
        }
    }
}