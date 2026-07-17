using System.Text.Json;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Diagnostics;
using OregonTrailDotNet.Bot.Game;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>Progress emitted after each generation, for the live UI/console.</summary>
    public sealed class GenerationProgress
    {
        public int Generation { get; init; }
        public double MeanFitness { get; init; }
        public int BestScoreThisGen { get; init; }
        public int BestScoreEver { get; init; }
        public int GamesThisGen { get; init; }
        public int WinsThisGen { get; init; }
        public int TotalIterations { get; init; }
    }

    /// <summary>Progress within a single generation — one tick per game played — so the console can draw a live bar
    ///     instead of going silent for the whole 16 x 64-game batch (~10+ seconds at training speed).</summary>
    public readonly record struct GenerationTick(int Generation, int GamesPlayed, int GamesTotal, int WinsSoFar);
}
