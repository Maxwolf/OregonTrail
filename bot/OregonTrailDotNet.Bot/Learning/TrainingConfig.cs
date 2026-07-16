using System.Text.Json;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Diagnostics;
using OregonTrailDotNet.Bot.Game;

namespace OregonTrailDotNet.Bot.Learning
{
    public sealed class TrainingConfig
    {
        public int PopulationSize { get; init; } = 16;

        /// <summary>Games averaged per candidate. Wins are rare (a few percent at best) and worth thousands of fitness, so an
        ///     8-game mean was dominated by whether a candidate happened to draw a lucky win — 85% of 8-game evaluations
        ///     contained no win at all and elites were selected on that noise. 64 games cuts the standard error ~3x, enough to
        ///     rank candidates whose true win rates differ by a few percentage points.</summary>
        public int GamesPerCandidate { get; init; } = 64;

        public int Generations { get; init; } = 5;

        /// <summary>Base seed for the per-generation common-random-numbers evaluation. Every candidate in a generation plays
        ///     the same set of game seeds derived from this, so fitness differences reflect the genome rather than luck; a fixed
        ///     value also makes a whole training run reproducible.</summary>
        public int EvaluationSeed { get; init; } = 20250715;
    }
}
