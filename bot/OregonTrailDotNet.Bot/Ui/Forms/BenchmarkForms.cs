using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>
    ///     Configures a benchmark before it starts: the time limit (minutes, 0 = run until every model wins or Esc). ENTER
    ///     records the request and hands off to Program.
    /// </summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class BenchmarkConfigForm : Form<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public BenchmarkConfigForm(IWindow window) : base(window)
        {
        }

        public override bool InputFillsBuffer => true;

        public override string OnRenderForm()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Environment.NewLine}Benchmark: how long each model takes to score its first win.");
            sb.AppendLine("Runs one bot of every model until each has won (or time runs out).");
            sb.AppendLine();
            sb.AppendLine($"  Time limit:  {DurationLabel(UserData.BenchmarkMinutes)}");
            sb.AppendLine();
            sb.AppendLine("  Type a number to set the minutes (0 = run until every model wins).");
            sb.AppendLine("  ENTER - start benchmark          b - go back");
            sb.Append("  (Press Esc during the run to stop and see the results.)");
            return sb.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            var text = input?.Trim() ?? string.Empty;

            if (text.Length == 0)
            {
                // ENTER: record the request and tear the panel down so Program can run the benchmark.
                BotContext.Request = new BotRequest
                {
                    Kind = BotRequestKind.Benchmark,
                    BenchmarkMinutes = UserData.BenchmarkMinutes
                };
                BotSimulationApp.Instance?.Destroy();
                return;
            }

            if (text.Equals("b", StringComparison.OrdinalIgnoreCase))
            {
                ClearForm();
                return;
            }

            if (int.TryParse(text, out var minutes) && minutes >= 0)
                UserData.BenchmarkMinutes = minutes;
            // Anything else: ignore and leave the form up unchanged.
        }

        private static string DurationLabel(int minutes) =>
            minutes <= 0 ? "Until every model wins (or Esc)" : $"{minutes} minute{(minutes == 1 ? "" : "s")}";
    }
}
