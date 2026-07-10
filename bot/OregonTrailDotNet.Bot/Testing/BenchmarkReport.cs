using System.Text;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;

namespace OregonTrailDotNet.Bot.Testing
{
    /// <summary>What each model races to achieve in a benchmark.</summary>
    public enum BenchmarkGoal
    {
        /// <summary>Score any win (reach Oregon).</summary>
        FirstWin,

        /// <summary>Match or beat Stephen Meek's 7650 — the top score on the game's original high-score list (Trail Guide).</summary>
        MeekScore
    }

    /// <summary>One model's progress toward (and result at) the benchmark goal.</summary>
    public sealed class BenchmarkResult
    {
        public BenchmarkResult(string key, string displayName)
        {
            Key = key;
            DisplayName = displayName;
        }

        public string Key { get; }
        public string DisplayName { get; }

        /// <summary>Total games this model has played so far.</summary>
        public int Games { get; set; }

        public bool Reached { get; set; }

        /// <summary>Games this model had played when it first reached the goal (only meaningful when <see cref="Reached" />).</summary>
        public int GamesToGoal { get; set; }

        /// <summary>Wall-clock time from the start of the session to this model reaching the goal.</summary>
        public TimeSpan TimeToGoal { get; set; }

        /// <summary>The score of the game that reached the goal.</summary>
        public int ScoreAtGoal { get; set; }
    }

    /// <summary>
    ///     Accumulates a benchmark that measures how long each training model takes to reach a goal (a first win, or Stephen
    ///     Meek's 7650), and formats the comparison (fastest first) into a saveable report.
    /// </summary>
    public sealed class BenchmarkReport
    {
        private readonly Dictionary<string, BenchmarkResult> _byModel;
        private readonly DateTime _startedAt = DateTime.UtcNow;
        private DateTime? _finishedAt;

        public BenchmarkReport(IReadOnlyList<ITrainingModel> models, int configuredMinutes, string goalLabel)
        {
            ConfiguredMinutes = configuredMinutes;
            GoalLabel = goalLabel;
            Results = models.Select(m => new BenchmarkResult(m.Key, m.DisplayName)).ToList();
            _byModel = Results.ToDictionary(r => r.Key);
        }

        public int ConfiguredMinutes { get; }

        /// <summary>Human-readable description of the goal, e.g. "a first win" or "Stephen Meek's 7650 (Trail Guide)".</summary>
        public string GoalLabel { get; }

        public IReadOnlyList<BenchmarkResult> Results { get; }
        public int TotalGames { get; private set; }
        public string EndReason { get; set; } = "";

        /// <summary>Highest single-game score seen so far across every model — a fun figure to watch climb during a run.</summary>
        public int BestScore { get; private set; }

        /// <summary>Which model scored <see cref="BestScore" /> (empty until any game scores above zero).</summary>
        public string BestScoreModel { get; private set; } = "";

        public TimeSpan Elapsed => (_finishedAt ?? DateTime.UtcNow) - _startedAt;

        /// <summary>True once every model has reached the goal.</summary>
        public bool AllReached => Results.All(r => r.Reached);

        /// <summary>Records one finished game; returns the model's result if this game first reached the goal, else null.</summary>
        public BenchmarkResult? Record(ITrainingModel model, bool reachedGoal, int score, TimeSpan elapsed)
        {
            TotalGames++;
            var stats = _byModel[model.Key];
            stats.Games++;

            if (score > BestScore)
            {
                BestScore = score;
                BestScoreModel = model.DisplayName;
            }

            if (!reachedGoal || stats.Reached)
                return null;

            stats.Reached = true;
            stats.GamesToGoal = stats.Games;
            stats.TimeToGoal = elapsed;
            stats.ScoreAtGoal = score;
            return stats;
        }

        public void MarkFinished() => _finishedAt ??= DateTime.UtcNow;

        /// <summary>Renders the benchmark as a saveable report — models that reached the goal fastest-first, then any that didn't.</summary>
        public string Format()
        {
            var sb = new StringBuilder();
            sb.AppendLine("==================== MODEL BENCHMARK ====================");
            sb.AppendLine($"Goal     : reach {GoalLabel}");
            sb.AppendLine($"Started  : {_startedAt:o}");
            sb.AppendLine($"Finished : {(_finishedAt ?? DateTime.UtcNow):o}");
            sb.AppendLine($"Duration : {FormatSpan(Elapsed)}   (time limit: {(ConfiguredMinutes <= 0 ? "none" : ConfiguredMinutes + " min")})");
            sb.AppendLine($"Ended    : {(string.IsNullOrEmpty(EndReason) ? "(unspecified)" : EndReason)}");
            sb.AppendLine();
            sb.AppendLine($"Games played: {TotalGames}     Models that reached it: {Results.Count(r => r.Reached)}/{Results.Count}");
            sb.AppendLine($"Highest score seen: {BestScore}{(string.IsNullOrEmpty(BestScoreModel) ? "" : $" ({BestScoreModel})")}");
            sb.AppendLine();

            // Winners ranked by how quickly they got there; models that never reached the goal are listed last.
            var ranked = Results
                .OrderByDescending(r => r.Reached)
                .ThenBy(r => r.TimeToGoal)
                .ToList();

            sb.AppendLine($"  {"Rank",-5}{"Model",-24}{"Time",13}{"Games",8}{"Score",8}");
            var rank = 1;
            foreach (var r in ranked)
            {
                if (r.Reached)
                    sb.AppendLine($"  {rank++ + ".",-5}{Truncate(r.DisplayName, 23),-24}{FormatSpan(r.TimeToGoal),13}{r.GamesToGoal,8}{r.ScoreAtGoal,8}");
                else
                    sb.AppendLine($"  {"-",-5}{Truncate(r.DisplayName, 23),-24}{"not reached",13}{r.Games,8}{"-",8}");
            }

            sb.AppendLine();
            sb.AppendLine(AllReached
                ? "Every model reached the goal. The ranking above is the head-to-head result."
                : $"Models marked \"not reached\" did not reach {GoalLabel} before the session ended (shown with games played).");

            sb.AppendLine("========================================================");
            return sb.ToString();
        }

        /// <summary>Formats a duration for display: milliseconds under a second (games can win that fast), else m:ss.</summary>
        public static string FormatDuration(TimeSpan span) =>
            span.TotalSeconds < 1
                ? $"{(int) span.TotalMilliseconds}ms"
                : $"{(int) span.TotalMinutes}:{span.Seconds:00}";

        private static string FormatSpan(TimeSpan span) => FormatDuration(span);

        private static string Truncate(string text, int max) => text.Length <= max ? text : text[..max];
    }
}
