using System.Text.Json;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     Cross-Entropy Method optimizer over the strategy vector. It keeps a diagonal Gaussian distribution
    ///     (per-dimension mean + std); each generation it samples a population of genomes, and after they are scored it refits
    ///     the distribution to the top-scoring "elite" fraction (their mean and std), with a std floor so the search never
    ///     collapses and stops exploring. This is a good fit for the game's noisy, non-differentiable, episodic reward.
    /// </summary>
    public sealed class CemOptimizer
    {
        private readonly Random _rng;
        private readonly double[] _stdFloor;

        public CemOptimizer(int populationSize = 16, double eliteFraction = 0.3, int? seed = null)
        {
            PopulationSize = Math.Max(4, populationSize);
            EliteCount = Math.Max(2, (int) Math.Round(PopulationSize * eliteFraction));
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();

            Mean = StrategyGenome.DefaultMean();
            Std = StrategyGenome.DefaultStd();
            _stdFloor = StrategyGenome.DefaultStd().Select(s => s * 0.15).ToArray();
        }

        public double[] Mean { get; private set; }
        public double[] Std { get; private set; }
        public int Generation { get; private set; }
        public int PopulationSize { get; }
        public int EliteCount { get; }

        /// <summary>Best genome seen across all generations and its fitness (survives resume).</summary>
        public StrategyGenome? BestGenome { get; private set; }
        public double BestFitness { get; private set; } = double.NegativeInfinity;

        /// <summary>Draws a fresh population from the current distribution.</summary>
        public List<StrategyGenome> SampleGeneration()
        {
            var population = new List<StrategyGenome>(PopulationSize);
            for (var n = 0; n < PopulationSize; n++)
            {
                var raw = new double[StrategyGenome.Length];
                for (var i = 0; i < raw.Length; i++)
                    raw[i] = Mean[i] + Std[i] * Gaussian();
                population.Add(new StrategyGenome { Raw = raw });
            }

            return population;
        }

        /// <summary>Refits the distribution to the elite fraction of a scored population and advances the generation.</summary>
        public void Update(IReadOnlyList<(StrategyGenome Genome, double Fitness)> scored)
        {
            if (scored.Count == 0)
                return;

            var elites = scored.OrderByDescending(s => s.Fitness).Take(EliteCount).ToList();

            var newMean = new double[StrategyGenome.Length];
            var newStd = new double[StrategyGenome.Length];

            for (var i = 0; i < StrategyGenome.Length; i++)
            {
                double sum = 0;
                foreach (var e in elites)
                    sum += e.Genome.Raw[i];
                newMean[i] = sum / elites.Count;

                double variance = 0;
                foreach (var e in elites)
                {
                    var d = e.Genome.Raw[i] - newMean[i];
                    variance += d * d;
                }

                newStd[i] = Math.Max(Math.Sqrt(variance / elites.Count), _stdFloor[i]);
            }

            Mean = newMean;
            Std = newStd;
            Generation++;

            var champion = scored.OrderByDescending(s => s.Fitness).First();
            if (champion.Fitness > BestFitness)
            {
                BestFitness = champion.Fitness;
                BestGenome = champion.Genome;
            }
        }

        /// <summary>The distribution mean decoded as a genome — the optimizer's current "best guess" strategy.</summary>
        public StrategyGenome MeanGenome() => new() { Raw = (double[]) Mean.Clone() };

        public byte[] Serialize()
        {
            var state = new PersistedState
            {
                Mean = Mean,
                Std = Std,
                Generation = Generation,
                BestRaw = BestGenome?.Raw,
                BestFitness = BestFitness
            };
            return JsonSerializer.SerializeToUtf8Bytes(state);
        }

        public void Load(byte[]? blob)
        {
            if (blob == null || blob.Length == 0)
                return;

            var state = JsonSerializer.Deserialize<PersistedState>(blob);
            if (state?.Mean == null || state.Std == null)
                return;

            Mean = state.Mean;
            Std = state.Std;
            Generation = state.Generation;
            BestFitness = state.BestFitness;
            BestGenome = state.BestRaw != null ? new StrategyGenome { Raw = state.BestRaw } : null;
        }

        private double Gaussian()
        {
            var u1 = 1.0 - _rng.NextDouble();
            var u2 = 1.0 - _rng.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
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
