namespace TrailCommon
{
    /// <summary>
    ///     Defines all of the possible item types in the game that need to be kept track of, these are not implementations but
    ///     rather flags that need to be associated with the concrete handlers so we know what kind of item we are dealing with
    ///     and can assign points and meaning to it accordingly.
    /// </summary>
    public enum ItemCategory
    {
        /// <summary>
        ///     Food that party members consume everyday determined by rations.
        /// </summary>
        Food,

        /// <summary>
        ///     Clothing that can be worn by party members to provide warmth in colder weather.
        /// </summary>
        Clothing,

        /// <summary>
        ///     Bullets which are used for hunting animals, bodies will be converted in food.
        /// </summary>
        Bullets,

        /// <summary>
        ///     Parts go on the vehicle and keep it running.
        /// </summary>
        Part
    }
}