namespace TrailSimulation.Game
{
    /// <summary>
    ///     Determines if the game is currently running, won, or lost. Depending on these states the game will make decisions
    ///     about what data will be shown to the user and what commands are available to them.
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        ///     Game simulation is running normally and should be executing simulation as intended until a win or lose state is
        ///     achieved.
        /// </summary>
        Running = 0,


        /// <summary>
        ///     Game won by player, time to show them the end game points and let them choose to restart if they want.
        /// </summary>
        Win = 1,


        /// <summary>
        ///     Game has failed and now we will show the player the game over screen and let them restart.
        /// </summary>
        Fail = 2
    }
}