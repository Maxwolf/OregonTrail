namespace TrailEntities.EndGame
{
    /// <summary>
    ///     Defines the commands that can be used when the end game mode is attached and currently the active game mode in the
    ///     simulation.
    /// </summary>
    public enum EndGameCommands
    {
        /// <summary>
        ///     At the end of the game the player is given the chance to return to the main menu with everything reset so they can
        ///     play again.
        /// </summary>
        ReturnToMainMenu = 1,

        /// <summary>
        ///     At the end of the game the player is given the chance to close the game if they want.
        /// </summary>
        ExitSimulation = 2
    }
}