namespace TrailCommon
{
    /// <summary>
    ///     Defines all of the commands required to start a new trail simulation with player names, leader profession
    ///     (determines starting amount of money), buying initial items, confirming all these details, and then finally
    ///     submitting it all to the simulation to start a new trail simulation.
    /// </summary>
    public enum NewGameCommands
    {
        /// <summary>
        ///     Input the leaders name.
        /// </summary>
        ChooseNames,

        /// <summary>
        ///     Choose leader profession, applies to everybody in the group (do not lose abilities if he dies).
        /// </summary>
        ChooseProfession,

        /// <summary>
        ///     Store game mode that will attach on top of new game one and allow the player a chance to buy initial starting
        ///     items.
        /// </summary>
        BuyInitialItems,

        /// <summary>
        ///     Takes the new game information object and passes it along to the currently running simulation, attaches the
        ///     traveling mode to get the game started on the trail path.
        /// </summary>
        StartGame
    }
}