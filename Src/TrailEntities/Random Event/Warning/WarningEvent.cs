namespace TrailEntities
{
    /// <summary>
    ///     Defines all of the types of warnings the players would want to know about before they get worse.
    /// </summary>
    public enum WarningEvent
    {
        /// <summary>
        ///     Risk of cholera and other diseases increases.
        /// </summary>
        BadWater = 1,

        /// <summary>
        ///     Food items are running low, player needs to find, trade , hunt, or buy some more.
        /// </summary>
        LowFood = 2,

        /// <summary>
        ///     Food items are empty and gone, starvation will occur every day there is no food to eat.
        /// </summary>
        NoFood = 3,

        /// <summary>
        ///     The climate you are traveling through has very low water amount.
        /// </summary>
        LowWater = 4,

        /// <summary>
        ///     Oxen that pull the vehicle will not have as much grass available to graze one and eat.
        /// </summary>
        LowGrass = 5,

        /// <summary>
        ///     Occurs when there is no food items for the people in the vehicle to eat. Will eventually kill the party members if
        ///     ticked enough in this state.
        /// </summary>
        Starvation = 6,

        /// <summary>
        ///     There is no water in the party to drink.
        /// </summary>
        NoWater = 7
    }
}