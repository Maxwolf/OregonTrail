namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A training model over the interpretable <see cref="StrategyGenome" /> (profession, provisioning, thresholds, river
    ///     preferences). The four evolutionary variants (CEM, genetic, hill-climber, random) are all instances of this class
    ///     that differ only in which <see cref="IOptimizer" /> they build — they share the same genes and the same
    ///     <see cref="GenomePolicy" /> decoding.
    /// </summary>
    public sealed class StrategyModel : ITrainingModel
    {
        private readonly Func<double[], double[], int, IOptimizer> _optimizerFactory;

        public StrategyModel(string key, string displayName, string description,
            Func<double[], double[], int, IOptimizer> optimizerFactory)
        {
            Key = key;
            DisplayName = displayName;
            Description = description;
            _optimizerFactory = optimizerFactory;
        }

        public string Key { get; }
        public string DisplayName { get; }
        public string Description { get; }

        public int VectorLength => StrategyGenome.Length;
        public double[] InitialMean() => StrategyGenome.DefaultMean();
        public double[] InitialStd() => StrategyGenome.DefaultStd();

        public IPolicy Decode(double[] vector, string leaderName) =>
            // Fit the vector to the current gene layout so replaying a genome saved under an older layout never indexes out of
            // range (fresh training always matches exactly).
            new GenomePolicy(new StrategyGenome { Raw = StrategyGenome.Sized(vector) }, leaderName);

        public IOptimizer CreateOptimizer(int populationSize) =>
            _optimizerFactory(InitialMean(), InitialStd(), populationSize);
    }
}
