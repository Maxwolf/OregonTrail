using System.Text;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;

namespace OregonTrailDotNet.Bot.Testing
{
    /// <summary>
    ///     Accumulates the results of an automated-testing session — per-model game tallies and the list of problems found —
    ///     and formats them into a saveable, developer-facing report.
    /// </summary>
    public sealed class AutoTestReport
    {
        private readonly Dictionary<string, ModelStats> _byModel;
        private readonly DateTime _startedAt = DateTime.UtcNow;
        private DateTime? _finishedAt;

        public AutoTestReport(IReadOnlyList<ITrainingModel> models, int configuredMinutes, bool stopOnProblem)
        {
            ConfiguredMinutes = configuredMinutes;
            StopOnProblem = stopOnProblem;
            Models = models.Select(m => new ModelStats(m.Key, m.DisplayName)).ToList();
            _byModel = Models.ToDictionary(s => s.Key);
        }

        public int ConfiguredMinutes { get; }
        public bool StopOnProblem { get; }
        public IReadOnlyList<ModelStats> Models { get; }
        public List<AutoTestProblem> Problems { get; } = new();
        public int TotalGames { get; private set; }

        /// <summary>True when the session ended because it hit its first problem (stop-on-problem was on).</summary>
        public bool StoppedOnProblem { get; set; }

        /// <summary>Human-readable reason the session ended (set by the caller unless stopped on a problem).</summary>
        public string EndReason { get; set; } = "";

        public TimeSpan Elapsed => (_finishedAt ?? DateTime.UtcNow) - _startedAt;

        /// <summary>Records one finished game; returns the problem it surfaced, or null if the game was clean.</summary>
        public AutoTestProblem? Record(ITrainingModel model, RunResult result)
        {
            TotalGames++;
            var stats = _byModel[model.Key];
            stats.Games++;

            switch (result.Outcome)
            {
                case GameOutcomeEnum.Win:
                    stats.Wins++;
                    break;
                case GameOutcomeEnum.Death:
                    stats.Deaths++;
                    break;
                case GameOutcomeEnum.Timeout:
                    stats.Timeouts++;
                    break;
            }

            // A healthy game ends in a win, a timeout, or the party dying. Anything the bot had to abort — a crash, a
            // soft-lock, a screen with no handler, or a broken invariant — is a problem a developer should look at.
            var isProblem = result.Bug != null || result.Outcome == GameOutcomeEnum.Aborted;
            if (!isProblem)
                return null;

            stats.Problems++;
            var category = result.Bug?.Category.ToString() ?? "Aborted";
            var summary = result.Bug?.Detail
                          ?? (string.IsNullOrEmpty(result.AbortReason) ? "aborted for an unknown reason" : result.AbortReason);
            var detail = result.Bug?.Format() ?? BuildAbortDetail(result, summary);

            var problem = new AutoTestProblem
            {
                Model = model.DisplayName,
                GameIndex = TotalGames,
                Category = category,
                Summary = summary,
                Detail = detail
            };
            Problems.Add(problem);
            return problem;
        }

        public void MarkFinished() => _finishedAt ??= DateTime.UtcNow;

        private static string BuildAbortDetail(RunResult result, string summary)
        {
            var sb = new StringBuilder();
            sb.AppendLine("The bot could not finish this game (no crash was captured).");
            sb.AppendLine($"Reason   : {summary}");
            sb.AppendLine($"Outcome  : {result.Outcome}  days={result.Days} miles={result.Miles} survivors={result.Survivors}");
            return sb.ToString().TrimEnd();
        }

        /// <summary>Renders the whole session as a saveable, human-readable report.</summary>
        public string Format()
        {
            var sb = new StringBuilder();
            sb.AppendLine("==================== AUTOMATED TESTING REPORT ====================");
            sb.AppendLine($"Started  : {_startedAt:o}");
            sb.AppendLine($"Finished : {(_finishedAt ?? DateTime.UtcNow):o}");
            sb.AppendLine($"Duration : {FormatSpan(Elapsed)}   (configured: {(ConfiguredMinutes <= 0 ? "infinite" : ConfiguredMinutes + " min")})");
            sb.AppendLine($"Stop on first problem: {(StopOnProblem ? "yes" : "no")}");
            sb.AppendLine($"Ended    : {(string.IsNullOrEmpty(EndReason) ? "(unspecified)" : EndReason)}");
            sb.AppendLine();
            sb.AppendLine($"Games played: {TotalGames}     Problems found: {Problems.Count}");
            sb.AppendLine();

            sb.AppendLine("Per-model results:");
            sb.AppendLine($"  {"Model",-24}{"Games",7}{"Wins",6}{"Deaths",8}{"Timeouts",10}{"Problems",10}");
            foreach (var m in Models)
                sb.AppendLine($"  {Truncate(m.DisplayName, 23),-24}{m.Games,7}{m.Wins,6}{m.Deaths,8}{m.Timeouts,10}{m.Problems,10}");
            sb.AppendLine();

            if (Problems.Count == 0)
            {
                sb.AppendLine("No problems found. Every game ended cleanly (win, timeout, or the party dying).");
            }
            else
            {
                sb.AppendLine($"-------------------- PROBLEMS ({Problems.Count}) --------------------");
                for (var i = 0; i < Problems.Count; i++)
                {
                    var p = Problems[i];
                    sb.AppendLine();
                    sb.AppendLine($"[{i + 1}] {p.Model} — game #{p.GameIndex} — {p.Category}");
                    sb.AppendLine(p.Detail);
                }
            }

            sb.AppendLine();
            sb.AppendLine("==================================================================");
            return sb.ToString();
        }

        private static string FormatSpan(TimeSpan span) =>
            $"{(int) span.TotalMinutes}:{span.Seconds:00}";

        private static string Truncate(string text, int max) =>
            text.Length <= max ? text : text[..max];
    }
}
