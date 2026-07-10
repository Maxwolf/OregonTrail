using WolfCurses.Utility;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>
    ///     Top-level actions of the bot control panel. The integer values are what the player types to select the command
    ///     (WolfCurses maps the enum's "D" formatted value to the menu key), so they must stay contiguous starting at 1.
    /// </summary>
    public enum BotMainMenuCommands
    {
        [Description("Create a new bot profile")] CreateProfile = 1,

        [Description("Select an existing profile")] SelectProfile = 2,

        [Description("Train the active profile")] StartTraining = 3,

        [Description("Watch the active profile play a game")] WatchGame = 4,

        [Description("View the bot leaderboard")] ViewLeaderboard = 5,

        [Description("View training stats for the active profile")] ViewStats = 6,

        [Description("Automated testing (find game bugs)")] AutomatedTesting = 7,

        [Description("Delete a bot or erase all data")] ManageData = 8,

        [Description("Quit")] Quit = 9
    }
}
