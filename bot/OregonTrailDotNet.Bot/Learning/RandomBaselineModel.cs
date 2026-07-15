namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     The genuine naive baseline: a model whose policy plays random legal moves (<see cref="RandomPolicy" />) and whose
    ///     search vector is a single ignored dummy value. It slots into the normal training/benchmark/watch/leaderboard plumbing
    ///     so the learners can be measured against a real weak floor — as opposed to the expert-seeded "Random Search" control,
    ///     which samples the strong hand-tuned prior. Its optimizer never meaningfully learns (there is nothing to search); it
    ///     just satisfies the <see cref="IOptimizer" /> contract.
    /// </summary>
    public sealed class RandomBaselineModel : ITrainingModel
    {
        public string Key => "naive";
        public string DisplayName => "Random Play (naive baseline)";

        public string Description =>
            "No strategy — random legal moves; the honest floor the learners beat.";

        // A single dummy gene: the policy ignores the vector entirely, so there is nothing to search.
        public int VectorLength => 1;
        public double[] InitialMean() => new double[VectorLength];
        public double[] InitialStd() => new double[VectorLength]; // 0 width — no exploration of a vector nobody reads

        public IPolicy Decode(double[] vector, string leaderName) => new RandomPolicy(leaderName);

        public IOptimizer CreateOptimizer(int populationSize) =>
            new RandomSearchOptimizer(InitialMean(), InitialStd(), populationSize);
    }
}
