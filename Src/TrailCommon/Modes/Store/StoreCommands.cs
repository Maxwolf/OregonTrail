namespace TrailCommon
{
    /// <summary>
    ///     Holds all of the available commands a store can make, it will pull the store items from the current point of
    ///     interest. The way the inheritance chain is setup settlement points of interest are the lowest tier that support
    ///     having a store, if it is empty then there is considered to be no store present.
    /// </summary>
    public enum StoreCommands
    {
        /// <summary>
        ///     Purchase an oxen to pull the vehicle, in the simulation oxen are not considered animals but instead parts of the
        ///     vehicle since without them no more progress can be made.
        /// </summary>
        BuyOxen,

        /// <summary>
        ///     Purchase food, it comes in pounds and it fairly abstract in the contents. There is no delineation on things like
        ///     sugar, bacon, salt, since there is no individual way to control these items while on the trail only consume to keep
        ///     health good.
        /// </summary>
        BuyFood,

        /// <summary>
        ///     Purchases clothing to add resistance to the climate, if it is very cold outside then there should be enough
        ///     clothing items for every party member. When the calculation is made if there is less clothing items than there is
        ///     party members one of them that was left out will begin to freeze and their health will lower.
        /// </summary>
        BuyClothing,

        /// <summary>
        ///     Bullets come in boxes of twenty (20) each, players use bullets to kill wild animals in the hunting game mode. They
        ///     can only take back 250 pounds of meat to the wagon (all they can carry).
        /// </summary>
        BuyAmmunition,

        /// <summary>
        ///     Wheels allow the vehicle to move forward, without them the entire trip comes to a halt and they must be fixed.
        /// </summary>
        BuySpareWheel,

        /// <summary>
        ///     Transfers oxen power to the rear wheels so they all turn at the same time rather than independently, without an
        ///     axle the vehicle cannot move.
        /// </summary>
        BuySpareAxles,

        /// <summary>
        ///     Hitches the oxen to the vehicle, without this part the vehicle is broken and cannot move and must be repaired.
        /// </summary>
        BuySpareTongues,

        /// <summary>
        ///     Shows some general help and advice about what the various items mean and what they are for during the course of the
        ///     simulation.
        /// </summary>
        StoreAdvice,

        /// <summary>
        ///     Detaches the store mode from the simulation and returns to the one previous.
        /// </summary>
        LeaveStore
    }
}