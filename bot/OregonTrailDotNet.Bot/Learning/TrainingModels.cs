namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     The catalog of training models the user can choose from when creating a profile. CEM is the default; the others let
    ///     the user compare how different learners play and improve on the same game.
    /// </summary>
    public static class TrainingModels
    {
        public static IReadOnlyList<ITrainingModel> All { get; } = new ITrainingModel[]
        {
            new StrategyModel("cem", "Cross-Entropy Method",
                "Refits a bell curve to the best strategies each round. Balanced and reliable — the default.",
                (mean, std, pop) => new CemOptimizer(mean, std, pop)),

            new StrategyModel("genetic", "Genetic Algorithm",
                "Breeds a population: keeps the fittest, mixes them (crossover) and mutates. Explores widely.",
                (mean, std, pop) => new GeneticOptimizer(mean, std, pop)),

            new StrategyModel("hillclimb", "Hill Climber",
                "Greedily mutates the current best and keeps it only if it scores higher. Fast; can get stuck.",
                (mean, std, pop) => new HillClimberOptimizer(mean, std, pop)),

            new StrategyModel("random", "Random Search",
                "Does not learn — tries random strategies and keeps the best seen. A baseline to compare against.",
                (mean, std, pop) => new RandomSearchOptimizer(mean, std, pop)),

            new NeuralModel()
        };

        public static ITrainingModel Default => ByKey("cem");

        /// <summary>Resolves a model by its stored key, falling back to CEM for unknown/legacy keys.</summary>
        public static ITrainingModel ByKey(string? key) =>
            All.FirstOrDefault(m => m.Key == key) ?? All.First(m => m.Key == "cem");
    }
}
