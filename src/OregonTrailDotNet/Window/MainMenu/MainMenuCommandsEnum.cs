// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using WolfCurses.Utility;

namespace OregonTrailDotNet.Window.MainMenu
{
    /// <summary>
    ///     List of all the commands for starting a new game, this is basically the main menu of the simulation.
    /// </summary>
    public enum MainMenuCommandsEnum
    {
        /// <summary>
        ///     Begins data entry for starting a new game such as selecting professions, names, starting items, etc.
        /// </summary>
        [Description("Travel the trail")] TravelTheTrail = 1,

        /// <summary>
        ///     Explains how the game works and what your goals are while playing it.
        /// </summary>
        [Description("Learn about the trail")] LearnAboutTheTrail = 2,

        /// <summary>
        ///     Shows high score list which is top ten highest scores in the game loaded from JSON file in application directory.
        /// </summary>
        [Description("See the Oregon Top Ten")] SeeTheOregonTopTen = 3,

        /// <summary>
        ///     Shows version information, ability to clear high scores, Tombstone messages, saved games
        /// </summary>
        [Description("Choose Management Options")] ChooseManagementOptions = 4,

        /// <summary>
        ///     Exits the application, clears in memory in holding.
        /// </summary>
        [Description("End")] CloseSimulation = 5,

        /// <summary>
        ///     Mutes and unmutes the music, the original's "Turn sound off". Only offered when presentation is on —
        ///     headless hosts have no sound and their menu text must not change. Value 6 because the printed number
        ///     IS the enum value and 1-5 are frozen (the original's slot 4 belongs to Management Options here).
        /// </summary>
        [Description("Turn sound on/off")] ToggleSound = 6
    }
}