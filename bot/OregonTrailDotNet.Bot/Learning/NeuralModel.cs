namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     The neuro-evolution model: a small neural network (<see cref="NeuralPolicy" />) whose weights are searched by CEM.
    ///     The whole setup + tactical slice is warm-started from the same expert prior as the strategy models, and the network
    ///     weights start at zero — so at generation 0 the policy plays the exact expert strategy (a zero-residual network), then
    ///     evolves state-adaptive corrections on top of it. This is the warm-start that lets it reach its first win as quickly as
    ///     the linear bots instead of rediscovering a whole tactical policy from random weights.
    /// </summary>
    public sealed class NeuralModel : ITrainingModel
    {
        public string Key => "neuro";
        public string DisplayName => "Neuro-Evolution";

        public string Description => "A small neural net reacts to the live game state. Adaptive and unique.";

        public int VectorLength => NeuralPolicy.VectorLength;

        public double[] InitialMean()
        {
            var mean = new double[VectorLength];
            // Warm-start the FULL genome (setup AND the rest/hunt/river/fork tactical thresholds), so the network starts as a
            // zero residual on top of the expert policy rather than from a do-nothing zero baseline. The MLP weights stay 0.
            Array.Copy(StrategyGenome.DefaultMean(), mean, NeuralPolicy.SetupLength);
            return mean;
        }

        public double[] InitialStd()
        {
            var std = new double[VectorLength];
            Array.Copy(StrategyGenome.DefaultStd(), std, NeuralPolicy.SetupLength);
            // Network-weight exploration width. Kept modest so early candidates stay close to the expert baseline (small
            // tactical nudges) instead of large random swings that undo the warm-start and cost the bot its early wins.
            for (var i = NeuralPolicy.SetupLength; i < VectorLength; i++)
                std[i] = 0.25;
            return std;
        }

        public IPolicy Decode(double[] vector, string leaderName) => new NeuralPolicy(vector, leaderName);

        public IOptimizer CreateOptimizer(int populationSize) =>
            new CemOptimizer(InitialMean(), InitialStd(), populationSize);
    }
}
