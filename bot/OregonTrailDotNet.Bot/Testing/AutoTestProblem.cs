using System.Text;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;

namespace OregonTrailDotNet.Bot.Testing
{
    /// <summary>One problem the automated-testing session surfaced from a single game.</summary>
    public sealed class AutoTestProblem
    {
        public string Model { get; init; } = "";

        /// <summary>1-based index of the game (across all models) that produced this problem.</summary>
        public int GameIndex { get; init; }

        public string Category { get; init; } = "";
        public string Summary { get; init; } = "";

        /// <summary>Full developer-facing detail (a formatted <see cref="Diagnostics.BugReport" /> or abort context).</summary>
        public string Detail { get; init; } = "";
    }
}
