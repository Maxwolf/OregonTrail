namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Defines the type of location this will serve as. All locations have the same base operations and hooks to events,
    ///     however depending on their category it might trigger the launch of another state or Windows depending on the needs
    ///     of
    ///     the given attached Windows rather than us assuming what it is going to do.
    /// </summary>
    public enum LocationCategory
    {
        /// <summary>
        ///     Normal location on the trail that has no special significance. Can hunt here but there are no stores or other
        ///     people to talk to, or trade.
        /// </summary>
        Landmark = 0,

        /// <summary>
        ///     Slightly more accommodations than a landmark, typically has people to talk to, and a store. No hunting is allowed
        ///     in forts though!
        /// </summary>
        Settlement = 1,

        /// <summary>
        ///     Location is a river that the player will need to cross, there is a special state to deal with this.
        /// </summary>
        RiverCrossing = 2,

        /// <summary>
        ///     Location is a fork in the road and the player needs to make a choice, there is a special state to deal with this.
        /// </summary>
        ForkInRoad = 3
    }
}