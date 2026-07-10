namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     The neuro-evolution model: a small neural network (<see cref="NeuralPolicy" />) whose weights are searched by CEM.
    ///     Its setup slice starts from the same sensible defaults as the strategy models, while the network weights start near
    ///     zero and are explored outward — so early on it behaves plainly and then discovers state-adaptive tactics.
    /// </summary>
    public sealed class NeuralModel : ITrainingModel
    {
        public string Key => "neuro";
        public string DisplayName => "Neuro-Evolution";

        public string Description =>
            "Evolves a small neural network that reads the live game state (food, health, days left...) and decides " +
            "tactics on the fly. The most different, state-adaptive play style.";

        public int VectorLength => NeuralPolicy.VectorLength;

        public double[] InitialMean()
        {
            var mean = new double[VectorLength];
            Array.Copy(StrategyGenome.DefaultMean(), mean, NeuralPolicy.SetupLength);
            // MLP weights start at 0 (rest of the array is already zero).
            return mean;
        }

        public double[] InitialStd()
        {
            var std = new double[VectorLength];
            Array.Copy(StrategyGenome.DefaultStd(), std, NeuralPolicy.SetupLength);
            for (var i = NeuralPolicy.SetupLength; i < VectorLength; i++)
                std[i] = 0.5; // exploration width for the network weights
            return std;
        }

        public IPolicy Decode(double[] vector, string leaderName) => new NeuralPolicy(vector, leaderName);

        public IOptimizer CreateOptimizer(int populationSize) =>
            new CemOptimizer(InitialMean(), InitialStd(), populationSize);
    }
}
