// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainMenuCommands.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   List of all the commands for starting a new game, this is basically the main menu of the simulation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     List of all the commands for starting a new game, this is basically the main menu of the simulation.
    /// </summary>
    public enum MainMenuCommands
    {
        /// <summary>
        ///     Begins data entry for starting a new game such as selecting professions, names, starting items, etc.
        /// </summary>
        [Description("Travel the trail")]
        TravelTheTrail = 1, 

        /// <summary>
        ///     Explains how the game works and what your goals are while playing it.
        /// </summary>
        [Description("Learn about the trail")]
        LearnAboutTheTrail = 2, 

        /// <summary>
        ///     Shows high score list which is top ten highest scores in the game loaded from JSON file in application directory.
        /// </summary>
        [Description("See the Oregon Top Ten")]
        SeeTheOregonTopTen = 3, 

        /// <summary>
        ///     Shows version information, ability to clear high scores, Tombstone messages, saved games
        /// </summary>
        [Description("Choose Management Options")]
        ChooseManagementOptions = 4, 

        /// <summary>
        ///     Exits the application, clears in memory in holding.
        /// </summary>
        [Description("End")]
        CloseSimulation = 5
    }
}