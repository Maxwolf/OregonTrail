namespace OregonTrailDotNet.Bot.Data
{
    /// <summary>Per-generation training progress used to render the learning curve. Carries both the shaped fitness the
    ///     optimizer actually maximizes (mean/best) and the raw game score, plus the win-rate.</summary>
    public sealed class GenerationStat
    {
        public int Generation { get; init; }
        public double MeanFitness { get; init; }
        public double BestFitness { get; init; }
        public double MeanScore { get; init; }
        public int BestScore { get; init; }
        public double WinRate { get; init; }
        public int Games { get; init; }
    }
}
