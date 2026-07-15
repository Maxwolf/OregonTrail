using System.Text.Json;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Diagnostics;
using OregonTrailDotNet.Bot.Game;

namespace OregonTrailDotNet.Bot.Learning
{
    public sealed class TrainingConfig
    {
        public int PopulationSize { get; init; } = 16;
        public int GamesPerCandidate { get; init; } = 8;
        public int Generations { get; init; } = 5;

        /// <summary>Base seed for the per-generation common-random-numbers evaluation. Every candidate in a generation plays
        ///     the same set of game seeds derived from this, so fitness differences reflect the genome rather than luck; a fixed
        ///     value also makes a whole training run reproducible.</summary>
        public int EvaluationSeed { get; init; } = 20250715;
    }
}
