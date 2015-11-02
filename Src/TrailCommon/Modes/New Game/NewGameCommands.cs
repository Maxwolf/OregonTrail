namespace TrailCommon
{
    /// <summary>
    /// List of all the commands for starting a new game, this is basically the main menu of the simulation.
    /// </summary>
    public enum NewGameCommands
    {
        /// <summary>
        /// Begins data entry for starting a new game such as selecting professions, names, starting items, etc.
        /// </summary>
        TravelTheTrail = 1,

        /// <summary>
        /// Explains how the game works and what your goals are while playing it.
        /// </summary>
        LearnAboutTheTrail = 2,

        /// <summary>
        /// Shows high score list which is top ten highest scores in the game loaded from JSON file in application directory.
        /// </summary>
        SeeTheOregonTopTen = 3,


        ChooseManagementOptions = 4,


        End = 5
    }
}