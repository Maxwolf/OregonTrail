using System.Text;
using OregonTrailDotNet.Bot.Game;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>
    ///     Shown before a watch session so the viewer can set the playback speed and choose whether to keep replaying games
    ///     until they press Esc. Number keys adjust the settings in place; ENTER starts watching with the current choices.
    /// </summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class WatchConfigForm : Form<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public WatchConfigForm(IWindow window) : base(window)
        {
        }

        public override bool InputFillsBuffer => true;

        public override string OnRenderForm()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Environment.NewLine}Watch '{BotContext.ActiveProfileName}' play — set up the experience:{Environment.NewLine}");

            sb.AppendLine($"  Speed:  {SpeedLabel(UserData.WatchSpeed)}");
            sb.AppendLine("            1. Fast     2. Medium     3. Slow");
            sb.AppendLine();
            sb.AppendLine(UserData.WatchLoop
                ? "  Loop:   On — replay games until you press Esc"
                : "  Loop:   Off — play a single game");
            sb.AppendLine("            4. Toggle looping");

            sb.Append($"{Environment.NewLine}Press ENTER to start watching, or 0 to go back.");
            return sb.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            switch (input?.Trim())
            {
                case "1":
                    UserData.WatchSpeed = WatchSpeedEnum.Fast;
                    break;
                case "2":
                    UserData.WatchSpeed = WatchSpeedEnum.Medium;
                    break;
                case "3":
                    UserData.WatchSpeed = WatchSpeedEnum.Slow;
                    break;
                case "4":
                    UserData.WatchLoop = !UserData.WatchLoop;
                    break;
                case "0":
                    ClearForm();
                    break;
                case "":
                case null:
                    // Start watching: record the request and tear the control panel down so Program can run the session.
                    BotContext.Request = new BotRequest
                    {
                        Kind = BotRequestKindEnum.Watch,
                        ProfileId = BotContext.ActiveProfileId,
                        WatchSpeed = UserData.WatchSpeed,
                        LoopUntilEscape = UserData.WatchLoop
                    };
                    BotSimulationApp.Instance?.Destroy();
                    break;
                // Any other key leaves the form up unchanged.
            }
        }

        private static string SpeedLabel(WatchSpeedEnum speed) => speed switch
        {
            WatchSpeedEnum.Fast => "Fast",
            WatchSpeedEnum.Slow => "Slow",
            _ => "Medium"
        };
    }
}
