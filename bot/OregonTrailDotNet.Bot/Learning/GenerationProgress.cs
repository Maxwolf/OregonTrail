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
}
