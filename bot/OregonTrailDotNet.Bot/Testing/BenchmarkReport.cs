using System.Text;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;

namespace OregonTrailDotNet.Bot.Testing
{
    /// <summary>One model's progress toward (and result at) its first win during a benchmark.</summary>
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

        public bool Won { get; set; }

        /// <summary>Games this model had played when it first won (only meaningful when <see cref="Won" /> is true).</summary>
        public int GamesToFirstWin { get; set; }

        /// <summary>Wall-clock time from the start of the session to this model's first win.</summary>
        public TimeSpan TimeToFirstWin { get; set; }

        public int FirstWinScore { get; set; }
    }

    /// <summary>
    ///     Accumulates a benchmark that measures how long each training model takes to score its first win, and formats the
    ///     comparison (fastest first) into a saveable report.
    /// </summary>
    public sealed class BenchmarkReport
    {
        private readonly Dictionary<string, BenchmarkResult> _byModel;
        private readonly DateTime _startedAt = DateTime.UtcNow;
        private DateTime? _finishedAt;

        public BenchmarkReport(IReadOnlyList<ITrainingModel> models, int configuredMinutes)
        {
            ConfiguredMinutes = configuredMinutes;
            Results = models.Select(m => new BenchmarkResult(m.Key, m.DisplayName)).ToList();
            _byModel = Results.ToDictionary(r => r.Key);
        }

        public int ConfiguredMinutes { get; }
        public IReadOnlyList<BenchmarkResult> Results { get; }
        public int TotalGames { get; private set; }
        public string EndReason { get; set; } = "";

        public TimeSpan Elapsed => (_finishedAt ?? DateTime.UtcNow) - _startedAt;

        /// <summary>True once every model has scored at least one win.</summary>
        public bool AllWon => Results.All(r => r.Won);

        /// <summary>Records one finished game; returns the model's result if this game was its first win, else null.</summary>
        public BenchmarkResult? Record(ITrainingModel model, RunResult result, TimeSpan elapsed)
        {
            TotalGames++;
            var stats = _byModel[model.Key];
            stats.Games++;

            if (result.Outcome != GameOutcome.Win || stats.Won)
                return null;

            stats.Won = true;
            stats.GamesToFirstWin = stats.Games;
            stats.TimeToFirstWin = elapsed;
            stats.FirstWinScore = result.Score;
            return stats;
        }

        public void MarkFinished() => _finishedAt ??= DateTime.UtcNow;

        /// <summary>Renders the benchmark as a saveable, human-readable report — winners fastest-first, then any that didn't win.</summary>
        public string Format()
        {
            var sb = new StringBuilder();
            sb.AppendLine("==================== MODEL BENCHMARK: TIME TO FIRST WIN ====================");
            sb.AppendLine($"Started  : {_startedAt:o}");
            sb.AppendLine($"Finished : {(_finishedAt ?? DateTime.UtcNow):o}");
            sb.AppendLine($"Duration : {FormatSpan(Elapsed)}   (time limit: {(ConfiguredMinutes <= 0 ? "none" : ConfiguredMinutes + " min")})");
            sb.AppendLine($"Ended    : {(string.IsNullOrEmpty(EndReason) ? "(unspecified)" : EndReason)}");
            sb.AppendLine();
            sb.AppendLine($"Games played: {TotalGames}     Models that won: {Results.Count(r => r.Won)}/{Results.Count}");
            sb.AppendLine();

            // Winners ranked by how quickly they got there; models that never won are listed last.
            var ranked = Results
                .OrderByDescending(r => r.Won)
                .ThenBy(r => r.TimeToFirstWin)
                .ToList();

            sb.AppendLine($"  {"Rank",-5}{"Model",-24}{"Time to win",13}{"Games",8}{"Score",8}");
            var rank = 1;
            foreach (var r in ranked)
            {
                if (r.Won)
                    sb.AppendLine($"  {rank++ + ".",-5}{Truncate(r.DisplayName, 23),-24}{FormatSpan(r.TimeToFirstWin),13}{r.GamesToFirstWin,8}{r.FirstWinScore,8}");
                else
                    sb.AppendLine($"  {"-",-5}{Truncate(r.DisplayName, 23),-24}{"no win",13}{r.Games,8}{"-",8}");
            }

            sb.AppendLine();
            if (Results.All(r => r.Won))
                sb.AppendLine("Every model scored a win. The ranking above is the head-to-head result.");
            else
                sb.AppendLine("Models marked \"no win\" did not score a win before the session ended (shown with games played).");

            sb.AppendLine("===========================================================================");
            return sb.ToString();
        }

        private static string FormatSpan(TimeSpan span) => $"{(int) span.TotalMinutes}:{span.Seconds:00}";

        private static string Truncate(string text, int max) => text.Length <= max ? text : text[..max];
    }
}
