using System.Text.Json;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A greedy (1+λ) hill climber: keep a single current point, and each generation evaluate the current point plus λ
    ///     Gaussian-mutated neighbours, then move to whichever scored best. Fast and simple, but with no population it tends to
    ///     get stuck in local optima — an instructive contrast to CEM/GA.
    /// </summary>
    public sealed class HillClimberOptimizer : IOptimizer
    {
        private readonly Random _rng;
        private readonly double[] _step;
        private readonly int _length;
        private readonly int _population;

        private double[] _current;

        public HillClimberOptimizer(double[] initialMean, double[] initialStd, int populationSize = 16, int? seed = null)
        {
            _length = initialMean.Length;
            _population = Math.Max(2, populationSize);
            _step = (double[]) initialStd.Clone();
            _current = (double[]) initialMean.Clone();
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public int Generation { get; private set; }
        public double[]? BestVector { get; private set; }
        public double BestFitness { get; private set; } = double.NegativeInfinity;

        public IReadOnlyList<double[]> Sample()
        {
            // The current point (so a lucky re-evaluation can't push us backwards) plus mutated neighbours.
            var batch = new List<double[]>(_population) { (double[]) _current.Clone() };
            while (batch.Count < _population)
            {
                var neighbour = new double[_length];
                for (var i = 0; i < _length; i++)
                    neighbour[i] = _current[i] + _step[i] * Sampling.Gaussian(_rng);
                batch.Add(neighbour);
            }

            return batch;
        }

        public void Update(IReadOnlyList<(double[] Vector, double Fitness)> scored)
        {
            if (scored.Count == 0)
                return;

            var best = scored.OrderByDescending(s => s.Fitness).First();
            _current = (double[]) best.Vector.Clone();
            Generation++;

            if (best.Fitness > BestFitness)
            {
                BestFitness = best.Fitness;
                BestVector = (double[]) best.Vector.Clone();
            }
        }

        public double[] MeanVector() => (double[]) _current.Clone();

        public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(new PersistedState
        {
            Current = _current,
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

            if (state.Current != null && state.Current.Length == _length)
                _current = state.Current;

            Generation = state.Generation;
            BestFitness = state.BestFitness;
            if (state.BestRaw != null && state.BestRaw.Length == _length)
                BestVector = state.BestRaw;

            // A champion scored under an older fitness shaping is not comparable on the new scale — drop it (the climber's
            // current point remains a meaningful starting point) so the next batch's champion can take over.
            if (state.FitnessVersion != TrainingSession.FitnessVersion)
            {
                BestFitness = double.MinValue; // MinValue, not NegativeInfinity: a bug-halt can Serialize before the next Update, and JSON cannot express Infinity
                BestVector = null;
            }
        }

        private sealed class PersistedState
        {
            public double[]? Current { get; set; }
            public int Generation { get; set; }
            public double[]? BestRaw { get; set; }
            public double BestFitness { get; set; }

            // Absent in blobs saved before versioning existed — deserializes to 0, which never matches a real version.
            public int FitnessVersion { get; set; }
        }
    }
}
