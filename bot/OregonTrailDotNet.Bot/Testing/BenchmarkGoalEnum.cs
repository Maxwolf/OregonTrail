using System.Text;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;

namespace OregonTrailDotNet.Bot.Testing
{
    /// <summary>What each model races to achieve in a benchmark.</summary>
    public enum BenchmarkGoalEnum
    {
        /// <summary>Score any win (reach Oregon).</summary>
        FirstWin,

        /// <summary>Match or beat Stephen Meek's 7650 — the top score on the game's original high-score list (Trail Guide).</summary>
        MeekScore
    }
}
