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
                "Balanced and reliable. Refits toward the best strategies each round.",
                (mean, std, pop) => new CemOptimizer(mean, std, pop)),

            new StrategyModel("genetic", "Genetic Algorithm",
                "Breeds a population with crossover and mutation. Explores widely.",
                (mean, std, pop) => new GeneticOptimizer(mean, std, pop)),

            new StrategyModel("hillclimb", "Hill Climber",
                "Greedily improves one strategy. Fast, but can get stuck.",
                (mean, std, pop) => new HillClimberOptimizer(mean, std, pop)),

            new StrategyModel("random", "Random Search (expert-seeded)",
                "No learning — best of random tries around the expert prior (strong).",
                (mean, std, pop) => new RandomSearchOptimizer(mean, std, pop)),

            new NeuralModel(),

            // The genuine weak floor: plays random legal moves, no strategy at all. Distinct from "Random Search" above, which
            // samples the strong expert prior — the gap between the learners and THIS is the honest measure of learning.
            new RandomBaselineModel()
        };

        public static ITrainingModel Default => ByKey("cem");

        /// <summary>Resolves a model by its stored key, falling back to CEM for unknown/legacy keys.</summary>
        public static ITrainingModel ByKey(string? key) =>
            All.FirstOrDefault(m => m.Key == key) ?? All.First(m => m.Key == "cem");
    }
}
