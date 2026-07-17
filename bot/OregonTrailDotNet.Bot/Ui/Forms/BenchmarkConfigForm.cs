using System.Text;
using OregonTrailDotNet.Bot.Testing;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>
    ///     Configures a benchmark before it starts: the goal each model races to (a first win, Stephen Meek's 7650, or the
    ///     game's 13,860 ceiling) and the time limit (minutes, 0 = run until every model reaches it or Esc). ENTER records
    ///     the request and hands off to Program.
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
            sb.AppendLine($"{Environment.NewLine}Benchmark: how long each model takes to reach a goal.");
            sb.AppendLine("Runs one bot of every model until each reaches it (or time runs out).");
            sb.AppendLine();
            sb.AppendLine($"  Goal:        {GoalLabel(UserData.BenchmarkGoal)}");
            sb.AppendLine($"  Time limit:  {DurationLabel(UserData.BenchmarkMinutes)}");
            sb.AppendLine();
            sb.AppendLine("  g - switch goal (first win / Meek test: 7650 / max score: 13,860)");
            sb.AppendLine("  Type a number to set the minutes (0 = run until every model reaches it).");
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
                    Kind = BotRequestKindEnum.Benchmark,
                    BenchmarkMinutes = UserData.BenchmarkMinutes,
                    BenchmarkGoal = UserData.BenchmarkGoal
                };
                BotSimulationApp.Instance?.Destroy();
                return;
            }

            if (text.Equals("g", StringComparison.OrdinalIgnoreCase))
            {
                // Cycle the three goals in escalating order: first win -> Meek's 7650 -> the 13,860 ceiling -> back.
                UserData.BenchmarkGoal = UserData.BenchmarkGoal switch
                {
                    BenchmarkGoalEnum.FirstWin => BenchmarkGoalEnum.MeekScore,
                    BenchmarkGoalEnum.MeekScore => BenchmarkGoalEnum.MaxScore,
                    _ => BenchmarkGoalEnum.FirstWin
                };
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

        private static string GoalLabel(BenchmarkGoalEnum goal) => goal switch
        {
            BenchmarkGoalEnum.MeekScore => "Meek test — reach Stephen Meek's 7650 (Trail Guide)",
            BenchmarkGoalEnum.MaxScore =>
                $"Max score test — reach the game's ceiling of {OregonTrailDotNet.Module.Scoring.ScoringModule.MaxPossibleScore:N0}",
            _ => "First win — reach Oregon"
        };

        private static string DurationLabel(int minutes) =>
            minutes <= 0 ? "Until every model reaches it (or Esc)" : $"{minutes} minute{(minutes == 1 ? "" : "s")}";
    }
}
