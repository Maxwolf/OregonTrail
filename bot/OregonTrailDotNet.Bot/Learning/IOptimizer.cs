namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A black-box optimizer that searches a fixed-length real vector to maximize a noisy, episodic fitness. It knows
    ///     nothing about the game or how a vector becomes a policy — that mapping belongs to the <see cref="ITrainingModel" />.
    ///     This is the seam that lets different training models (CEM, genetic, hill-climbing, random, neuro-evolution) plug in.
    /// </summary>
    public interface IOptimizer
    {
        /// <summary>Draws a fresh generation of candidate vectors to evaluate.</summary>
        IReadOnlyList<double[]> Sample();

        /// <summary>Folds the scored candidates back in and advances one generation.</summary>
        void Update(IReadOnlyList<(double[] Vector, double Fitness)> scored);

        int Generation { get; }

        /// <summary>Best vector seen across all generations (survives resume), or null before the first update.</summary>
        double[]? BestVector { get; }

        double BestFitness { get; }

        /// <summary>The optimizer's current "best guess" vector (e.g. a distribution mean or the current point).</summary>
        double[] MeanVector();

        byte[] Serialize();

        void Load(byte[]? blob);
    }
}
