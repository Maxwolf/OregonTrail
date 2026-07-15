using System.Text;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;

namespace OregonTrailDotNet.Bot.Testing
{
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

        /// <summary>Highest score this model has scored so far — climbs as the benchmark runs, even before the goal is met.</summary>
        public int BestScore { get; set; }
    }
}
