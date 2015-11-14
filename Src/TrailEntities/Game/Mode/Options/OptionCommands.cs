using System.ComponentModel;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Defines all of the management options for the game like removing tombstones, resetting high scores, and lets you
    ///     also view the original high scores.
    /// </summary>
    public enum OptionCommands
    {
        /// <summary>
        ///     Shows the original top ten list as it is known internally as a hard-coded list.
        /// </summary>
        [Description("See the original Top Ten list")]
        SeeOriginalTopTen = 1,

        /// <summary>
        ///     Resets the current top ten list back to hard-coded defaults.
        /// </summary>
        [Description("Erase the current Top Ten list")]
        EraseCurrentTopTen = 2,

        /// <summary>
        ///     Removes any custom tombstone messages from the trail, there are two opportunities in the game where if party leader
        ///     dies they can leave a tombstone on the trail for future travelers to find.
        /// </summary>
        [Description("Erase the tombstone messages")]
        EraseTomstoneMessages = 3,

        /// <summary>
        ///     Removes the management options game mode and returns to main menu which should be below it.
        /// </summary>
        [Description("Return to the main menu")]
        ReturnToMainMenu = 4
    }
}