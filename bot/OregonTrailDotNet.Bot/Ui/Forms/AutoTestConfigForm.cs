using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>
    ///     Configures an automated-testing session before it starts: how long to run (minutes, 0 = until Esc) and whether to
    ///     stop at the first problem or keep going and log them all. ENTER records the request and hands off to Program.
    /// </summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class AutoTestConfigForm : Form<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public AutoTestConfigForm(IWindow window) : base(window)
        {
        }

        public override bool InputFillsBuffer => true;

        public override string OnRenderForm()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Environment.NewLine}Automated testing runs one bot of every model against the game and");
            sb.AppendLine("records crashes, soft-locks, and stuck screens into a report you can save.");
            sb.AppendLine();
            sb.AppendLine($"  Duration:    {DurationLabel(UserData.AutoTestMinutes)}");
            sb.AppendLine(UserData.AutoTestStopOnProblem
                ? "  On problem:  Stop the session"
                : "  On problem:  Keep going and log it");
            sb.AppendLine();
            sb.AppendLine("  Type a number to set the minutes (0 = run until Esc).");
            sb.AppendLine("  s - toggle what happens when a problem is found");
            sb.AppendLine("  ENTER - start testing          b - go back");
            sb.Append("  (Press Esc during the run to stop and see the report.)");
            return sb.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            var text = input?.Trim() ?? string.Empty;

            if (text.Length == 0)
            {
                // ENTER: record the request and tear the panel down so Program can run the session.
                BotContext.Request = new BotRequest
                {
                    Kind = BotRequestKindEnum.AutoTest,
                    AutoTestMinutes = UserData.AutoTestMinutes,
                    AutoTestStopOnProblem = UserData.AutoTestStopOnProblem
                };
                BotSimulationApp.Instance?.Destroy();
                return;
            }

            if (text.Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                UserData.AutoTestStopOnProblem = !UserData.AutoTestStopOnProblem;
                return;
            }

            if (text.Equals("b", StringComparison.OrdinalIgnoreCase))
            {
                ClearForm();
                return;
            }

            if (int.TryParse(text, out var minutes) && minutes >= 0)
                UserData.AutoTestMinutes = minutes;
            // Anything else: ignore and leave the form up unchanged.
        }

        private static string DurationLabel(int minutes) =>
            minutes <= 0 ? "Infinite (until Esc)" : $"{minutes} minute{(minutes == 1 ? "" : "s")}";
    }
}
