namespace OregonTrail.Common
{
    /// <summary>
    ///     Defines all of the possible points in a given list of locations that the party of players will iterate through,
    ///     every game should have the start point be the beginning and the end point be at the end.
    /// </summary>
    public enum LocationCategory
    {
        /// <summary>
        ///     Beginning of the game, offers up chance to buy supplies and name party players.
        /// </summary>
        Start,

        /// <summary>
        ///     Ending point where the game over screen is shown and points are tabulated based on party and vehicle health,
        ///     bonuses for having extra items in good condition also.
        /// </summary>
        End,

        /// <summary>
        ///     Typically does not have a store but offers a safe place to rest where you won't have to worry about thieves.
        ///     Increases health of all party players when sleeping there.
        /// </summary>
        Fort,

        /// <summary>
        ///     Beautiful scenery worth stopping to look at, provides extra points for increasing health of players if resting is
        ///     done there.
        /// </summary>
        Landmark,

        /// <summary>
        ///     Section of the game that requires traversing manually, there is a total distance it equals. Since it is made of
        ///     linked list there is possibility to see what location comes before and after the current trip.
        /// </summary>
        Road
    }
}