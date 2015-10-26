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
        ///     Takes the new game information object and passes it along to the currently running simulation, attaches the
        ///     traveling mode to get the game started on the trail path.
        /// </summary>
        StartGame = 1
    }
}