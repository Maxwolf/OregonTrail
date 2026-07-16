using System.Text.Json;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A genetic algorithm: it keeps a population of vectors and breeds the next generation from the current one — carry
    ///     the fittest few unchanged (elitism), pick parents by tournament selection, mix them with uniform crossover, and
    ///     apply per-gene Gaussian mutation. Explores more broadly and escapes local optima differently than CEM.
    /// </summary>
    public sealed class GeneticOptimizer : IOptimizer
    {
        private const int TournamentSize = 3;
        private const double MutationRate = 0.25;

        private readonly Random _rng;
        private readonly double[] _initialMean;
        private readonly double[] _initialStd;
        private readonly double[] _mutationScale;
        private readonly int _length;
        private readonly int _population;
        private readonly int _eliteCount;

        private List<double[]>? _current;

        public GeneticOptimizer(double[] initialMean, double[] initialStd, int populationSize = 16, int? seed = null)
        {
            _initialMean = (double[]) initialMean.Clone();
            _initialStd = (double[]) initialStd.Clone();
            _mutationScale = (double[]) initialStd.Clone();
            _length = initialMean.Length;
            _population = Math.Max(4, populationSize);
            _eliteCount = Math.Max(1, _population / 6);
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public int Generation { get; private set; }
        public double[]? BestVector { get; private set; }
        public double BestFitness { get; private set; } = double.NegativeInfinity;

        public IReadOnlyList<double[]> Sample()
        {
            _current ??= InitialPopulation();
            return _current;
        }

        public void Update(IReadOnlyList<(double[] Vector, double Fitness)> scored)
        {
            if (scored.Count == 0)
                return;

            var ranked = scored.OrderByDescending(s => s.Fitness).ToList();

            var next = new List<double[]>(_population);
            for (var i = 0; i < _eliteCount && i < ranked.Count; i++)
                next.Add((double[]) ranked[i].Vector.Clone());

            while (next.Count < _population)
            {
                var child = Crossover(Tournament(scored), Tournament(scored));
                Mutate(child);
                next.Add(child);
            }

            _current = next;
            Generation++;

            if (ranked[0].Fitness > BestFitness)
            {
                BestFitness = ranked[0].Fitness;
                BestVector = (double[]) ranked[0].Vector.Clone();
            }
        }

        public double[] MeanVector() => BestVector != null ? (double[]) BestVector.Clone() : (double[]) _initialMean.Clone();

        private List<double[]> InitialPopulation()
        {
            var pop = new List<double[]>(_population);
            for (var n = 0; n < _population; n++)
            {
                var vector = new double[_length];
                for (var i = 0; i < _length; i++)
                    vector[i] = _initialMean[i] + _initialStd[i] * Sampling.Gaussian(_rng);
                pop.Add(vector);
            }

            return pop;
        }

        private double[] Tournament(IReadOnlyList<(double[] Vector, double Fitness)> scored)
        {
            var best = scored[_rng.Next(scored.Count)];
            for (var i = 1; i < TournamentSize; i++)
            {
                var challenger = scored[_rng.Next(scored.Count)];
                if (challenger.Fitness > best.Fitness)
                    best = challenger;
            }

            return best.Vector;
        }

        private double[] Crossover(double[] a, double[] b)
        {
            var child = new double[_length];
            for (var i = 0; i < _length; i++)
                child[i] = _rng.NextDouble() < 0.5 ? a[i] : b[i];
            return child;
        }

        private void Mutate(double[] vector)
        {
            for (var i = 0; i < _length; i++)
                if (_rng.NextDouble() < MutationRate)
                    vector[i] += _mutationScale[i] * Sampling.Gaussian(_rng);
        }

        public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(new PersistedState
        {
            Population = _current,
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

            if (state.Population != null && state.Population.All(v => v.Length == _length))
                _current = state.Population;

            Generation = state.Generation;
            BestFitness = state.BestFitness;
            if (state.BestRaw != null && state.BestRaw.Length == _length)
                BestVector = state.BestRaw;

            // A champion scored under an older fitness shaping is not comparable on the new scale — drop it (the evolved
            // population itself remains a meaningful starting point) so the next generation's champion can take over.
            if (state.FitnessVersion != TrainingSession.FitnessVersion)
            {
                BestFitness = double.MinValue; // MinValue, not NegativeInfinity: a bug-halt can Serialize before the next Update, and JSON cannot express Infinity
                BestVector = null;
            }
        }

        private sealed class PersistedState
        {
            public List<double[]>? Population { get; set; }
            public int Generation { get; set; }
            public double[]? BestRaw { get; set; }
            public double BestFitness { get; set; }

            // Absent in blobs saved before versioning existed — deserializes to 0, which never matches a real version.
            public int FitnessVersion { get; set; }
        }
    }
}
