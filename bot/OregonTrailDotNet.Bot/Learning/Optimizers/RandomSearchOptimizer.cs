using System.Text.Json;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A control that does not learn: every generation it draws a fresh batch of wide random vectors around a fixed centre
    ///     and only remembers the best one it has ever seen. Useful as a baseline — the gap between this and the learning
    ///     models is a direct measure of how much the learning actually helps.
    /// </summary>
    public sealed class RandomSearchOptimizer : IOptimizer
    {
        private const double Spread = 1.5; // wider than the learners' starting std, and it never shrinks

        private readonly Random _rng;
        private readonly double[] _mean;
        private readonly double[] _std;
        private readonly int _length;
        private readonly int _population;

        public RandomSearchOptimizer(double[] initialMean, double[] initialStd, int populationSize = 16, int? seed = null)
        {
            _mean = (double[]) initialMean.Clone();
            _std = (double[]) initialStd.Clone();
            _length = initialMean.Length;
            _population = Math.Max(2, populationSize);
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public int Generation { get; private set; }
        public double[]? BestVector { get; private set; }
        public double BestFitness { get; private set; } = double.NegativeInfinity;

        public IReadOnlyList<double[]> Sample()
        {
            var batch = new List<double[]>(_population);
            for (var n = 0; n < _population; n++)
            {
                var vector = new double[_length];
                for (var i = 0; i < _length; i++)
                    vector[i] = _mean[i] + Spread * _std[i] * Sampling.Gaussian(_rng);
                batch.Add(vector);
            }

            return batch;
        }

        public void Update(IReadOnlyList<(double[] Vector, double Fitness)> scored)
        {
            if (scored.Count == 0)
                return;

            var best = scored.OrderByDescending(s => s.Fitness).First();
            Generation++;

            if (best.Fitness > BestFitness)
            {
                BestFitness = best.Fitness;
                BestVector = (double[]) best.Vector.Clone();
            }
        }

        public double[] MeanVector() => BestVector != null ? (double[]) BestVector.Clone() : (double[]) _mean.Clone();

        public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(new PersistedState
        {
            Generation = Generation,
            BestRaw = BestVector,
            BestFitness = BestFitness,
            FitnessVersion = TrainingSession.FitnessVersion
        });

        public void Load(byte[]? blob)
        {
            if (blob == null || blob.Length == 0)
                return;

            var state = JsonSerializer.Deserialize<PersistedState>(blob);
            if (state == null)
                return;

            Generation = state.Generation;
            BestFitness = state.BestFitness;
            if (state.BestRaw != null && state.BestRaw.Length == _length)
                BestVector = state.BestRaw;

            // A champion scored under an older fitness shaping is not comparable on the new scale — drop it so the next
            // generation's champion can take over (the sampler itself is stateless, so nothing else is lost).
            if (state.FitnessVersion != TrainingSession.FitnessVersion)
            {
                BestFitness = double.MinValue; // MinValue, not NegativeInfinity: a bug-halt can Serialize before the next Update, and JSON cannot express Infinity
                BestVector = null;
            }
        }

        private sealed class PersistedState
        {
            public int Generation { get; set; }
            public double[]? BestRaw { get; set; }
            public double BestFitness { get; set; }

            // Absent in blobs saved before versioning existed — deserializes to 0, which never matches a real version.
            public int FitnessVersion { get; set; }
        }
    }
}
