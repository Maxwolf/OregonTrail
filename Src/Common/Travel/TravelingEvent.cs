namespace OregonTrail
{
    /// <summary>
    ///     Defines all the possible events that can occur to a party, depending on the roll chance and what type of event is
    ///     specified a class can be constructed to change the behavior.
    /// </summary>
    public enum TravelingEvent
    {
        /// <summary>
        ///     Affects party players and not items. Could be a simple fever or something crazy like a death.
        /// </summary>
        Medical,

        /// <summary>
        ///     Affects items and not party players. Could be a thief stealing something or a wagon wheel breaking.
        /// </summary>
        Physical,

        /// <summary>
        ///     Abandoned vessel or building that is ready for picking by the player, can contain items like food and parts.
        /// </summary>
        Derelict,

        /// <summary>
        ///     Grave site of another player that has died, can contain epitaph so the player can remember them.
        /// </summary>
        Tombstone,

        /// <summary>
        ///     A greedy person that has it out for you attacks while you in one of your turns. They can take food or part items,
        ///     and in extreme cases even kill party members.
        /// </summary>
        Theif
    }
}