using System.Text.Json;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     Cross-Entropy Method optimizer. Keeps a diagonal Gaussian distribution (per-dimension mean + std); each generation
    ///     it samples a population and, after they are scored, refits the distribution to the top-scoring "elite" fraction
    ///     (their mean and std), with a std floor so the search never fully collapses. A strong fit for the game's noisy,
    ///     non-differentiable, episodic reward, and reused as the search engine for neuro-evolution.
    /// </summary>
    public sealed class CemOptimizer : IOptimizer
    {
        private readonly Random _rng;
        private readonly double[] _stdFloor;
        private readonly int _length;

        public CemOptimizer(double[] initialMean, double[] initialStd, int populationSize = 16,
            double eliteFraction = 0.3, int? seed = null)
        {
            _length = initialMean.Length;
            PopulationSize = Math.Max(4, populationSize);
            EliteCount = Math.Max(2, (int) Math.Round(PopulationSize * eliteFraction));
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();

            Mean = (double[]) initialMean.Clone();
            Std = (double[]) initialStd.Clone();
            _stdFloor = initialStd.Select(s => Math.Abs(s) * 0.15).ToArray();
        }

        public double[] Mean { get; private set; }
        public double[] Std { get; private set; }
        public int Generation { get; private set; }
        public int PopulationSize { get; }
        public int EliteCount { get; }
        public double[]? BestVector { get; private set; }
        public double BestFitness { get; private set; } = double.NegativeInfinity;

        public IReadOnlyList<double[]> Sample()
        {
            var population = new List<double[]>(PopulationSize);
            for (var n = 0; n < PopulationSize; n++)
            {
                var vector = new double[_length];
                for (var i = 0; i < _length; i++)
                    vector[i] = Mean[i] + Std[i] * Sampling.Gaussian(_rng);
                population.Add(vector);
            }

            return population;
        }

        public void Update(IReadOnlyList<(double[] Vector, double Fitness)> scored)
        {
            if (scored.Count == 0)
                return;

            var ranked = scored.OrderByDescending(s => s.Fitness).ToList();
            var elites = ranked.Take(EliteCount).ToList();

            var newMean = new double[_length];
            var newStd = new double[_length];

            for (var i = 0; i < _length; i++)
            {
                double sum = 0;
                foreach (var e in elites)
                    sum += e.Vector[i];
                newMean[i] = sum / elites.Count;

                double variance = 0;
                foreach (var e in elites)
                {
                    var d = e.Vector[i] - newMean[i];
                    variance += d * d;
                }

                newStd[i] = Math.Max(Math.Sqrt(variance / elites.Count), _stdFloor[i]);
            }

            Mean = newMean;
            Std = newStd;
            Generation++;

            var champion = ranked[0];
            if (champion.Fitness > BestFitness)
            {
                BestFitness = champion.Fitness;
                BestVector = (double[]) champion.Vector.Clone();
            }
        }

        public double[] MeanVector() => (double[]) Mean.Clone();

        public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(new PersistedState
        {
            Mean = Mean,
            Std = Std,
            Generation = Generation,
            BestRaw = BestVector,
            BestFitness = BestFitness
        });

        public void Load(byte[]? blob)
        {
            if (blob == null || blob.Length == 0)
                return;

            var state = JsonSerializer.Deserialize<PersistedState>(blob);
            if (state?.Mean == null || state.Std == null || state.Mean.Length != _length)
                return; // absent or stale (different model/length) — start fresh

            Mean = state.Mean;
            Std = state.Std;
            Generation = state.Generation;
            BestFitness = state.BestFitness;
            BestVector = state.BestRaw;
        }

        private sealed class PersistedState
        {
            public double[]? Mean { get; set; }
            public double[]? Std { get; set; }
            public int Generation { get; set; }
            public double[]? BestRaw { get; set; }
            public double BestFitness { get; set; }
        }
    }
}
