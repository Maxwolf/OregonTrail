using System.Text.Json;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Diagnostics;
using OregonTrailDotNet.Bot.Game;

namespace OregonTrailDotNet.Bot.Learning
{
    public sealed class TrainingConfig
    {
        public int PopulationSize { get; init; } = 16;
        public int GamesPerCandidate { get; init; } = 8;
        public int Generations { get; init; } = 5;

        /// <summary>Base seed for the per-generation common-random-numbers evaluation. Every candidate in a generation plays
        ///     the same set of game seeds derived from this, so fitness differences reflect the genome rather than luck; a fixed
        ///     value also makes a whole training run reproducible.</summary>
        public int EvaluationSeed { get; init; } = 20250715;
    }

    /// <summary>Progress emitted after each generation, for the live UI/console.</summary>
    public sealed class GenerationProgress
    {
        public int Generation { get; init; }
        public double MeanFitness { get; init; }
        public int BestScoreThisGen { get; init; }
        public int BestScoreEver { get; init; }
        public int GamesThisGen { get; init; }
        public int WinsThisGen { get; init; }
        public int TotalIterations { get; init; }
    }

    /// <summary>
    ///     Runs training for a profile using whichever <see cref="ITrainingModel" /> the profile was created with. Each
    ///     generation it samples candidate vectors from the model's optimizer, decodes each into a policy, evaluates it over K
    ///     games (averaging out the game's randomness), and folds the scores back in. Everything is persisted (each game to
    ///     <c>runs</c>, the optimizer state + best score/vector to the profile, qualifying finishes to the leaderboard) and it
    ///     resumes from the profile's saved learning state. Fully headless and deterministic per tick.
    /// </summary>
    public sealed class TrainingSession
    {
        private readonly BotDatabase _db;
        private readonly long _profileId;
        private readonly string _profileName;
        private readonly string _leaderName;
        private readonly TrainingConfig _config;
        private readonly ITrainingModel _model;
        private readonly IOptimizer _optimizer;
        private readonly Func<IPolicy, int?, RunResult> _playGame;

        private int _bestScoreEver;

        /// <summary>The <paramref name="playGame" /> runner is injectable (defaulting to the real <see cref="GamePlayer.PlayOnce" />)
        ///     so tests can drive the record/leaderboard/persistence pipeline deterministically, mirroring
        ///     <see cref="Testing.BenchmarkSession" />.</summary>
        public TrainingSession(BotDatabase db, ProfileRecord profile, TrainingConfig config,
            Func<IPolicy, int?, RunResult>? playGame = null)
        {
            _db = db;
            _playGame = playGame ?? ((policy, seed) => GamePlayer.PlayOnce(policy, seed: seed));
            _profileId = profile.Id;
            _profileName = profile.Name;
            // The in-game high-score list is shared with human players, so the bot marks its entries there with "(bot)". The
            // bot's own leaderboard uses the plain profile name (everything on it is already a bot).
            _leaderName = $"{profile.Name} (bot)";
            _config = config;

            _model = TrainingModels.ByKey(profile.PolicyKind);
            _optimizer = _model.CreateOptimizer(config.PopulationSize);
            _optimizer.Load(profile.LearningState);

            _bestScoreEver = profile.BestScore;
        }

        public IOptimizer Optimizer => _optimizer;
        public ITrainingModel Model => _model;

        /// <summary>Runs the configured number of generations, invoking <paramref name="onGeneration" /> after each and
        ///     stopping early if <paramref name="shouldStop" /> returns true. A negative <see cref="TrainingConfig.Generations" />
        ///     means "run open-endedly" — the loop only ends when <paramref name="shouldStop" /> says so (the Esc/Ctrl+C hook).</summary>
        public void Run(Action<GenerationProgress>? onGeneration = null, Func<bool>? shouldStop = null)
        {
            for (var g = 0; _config.Generations < 0 || g < _config.Generations; g++)
            {
                var generationNumber = _optimizer.Generation;
                var candidates = _optimizer.Sample();
                var scored = new List<(double[] Vector, double Fitness)>(candidates.Count);

                var wins = 0;
                var bestScoreThisGen = 0;

                // Common random numbers: draw one set of game seeds for this generation and evaluate EVERY candidate on the
                // same seeds. Pairing the evaluations this way cancels most of the game's luck from the comparison, so the
                // fitness differences the optimizer selects on reflect the genome rather than which candidate drew easy games.
                var seedRng = new Random(unchecked(_config.EvaluationSeed * 1000003 + generationNumber));
                var seeds = new int[_config.GamesPerCandidate];
                for (var i = 0; i < seeds.Length; i++)
                    seeds[i] = seedRng.Next();

                for (var candidate = 0; candidate < candidates.Count; candidate++)
                {
                    var vector = candidates[candidate];
                    var vectorJson = JsonSerializer.Serialize(vector);
                    double fitnessSum = 0;

                    for (var k = 0; k < _config.GamesPerCandidate; k++)
                    {
                        var policy = _model.Decode(vector, _leaderName);
                        var result = _playGame(policy, seeds[k]);
                        var fitness = Fitness(result);

                        RecordRun(result, generationNumber, candidate, vectorJson, fitness);

                        // Unambiguous bugs (a crash, a broken invariant, or a screen the bot has no handler for) stop the whole
                        // batch so a developer can fix them. A plain soft-lock is treated as a failed game (score 0, which the
                        // optimizer learns to avoid) so one unlucky stuck game doesn't abort a long training run.
                        if (result.Bug != null && result.Bug.Category != BugCategory.SoftLock)
                        {
                            PersistOptimizer();
                            throw new BotBugException(result.Bug);
                        }

                        if (result.Outcome == GameOutcome.Win) wins++;
                        bestScoreThisGen = Math.Max(bestScoreThisGen, result.Score);
                        if (result.Score > _bestScoreEver)
                            _bestScoreEver = result.Score;

                        fitnessSum += fitness;
                    }

                    scored.Add((vector, fitnessSum / _config.GamesPerCandidate));
                }

                _optimizer.Update(scored);
                PersistOptimizer();

                onGeneration?.Invoke(new GenerationProgress
                {
                    Generation = generationNumber,
                    MeanFitness = scored.Average(s => s.Fitness),
                    BestScoreThisGen = bestScoreThisGen,
                    BestScoreEver = _bestScoreEver,
                    GamesThisGen = candidates.Count * _config.GamesPerCandidate,
                    WinsThisGen = wins,
                    TotalIterations = _db.Runs.CountForProfile(_profileId)
                });

                if (shouldStop?.Invoke() == true)
                    break;
            }
        }

        private void RecordRun(RunResult result, int generation, int candidateIndex, string vectorJson, double fitness)
        {
            // A finished run makes the persistent leaderboard if it beats the current 10th place (an empty/short board has a
            // 10th place of 0, so the first finishes fill it up).
            var madeTop10 = result.IsFinished && result.Score > 0 &&
                            result.Score > _db.Leaderboard.TenthPlaceScore();

            var runId = _db.Runs.Insert(new RunRecord
            {
                ProfileId = _profileId,
                IterationIndex = _db.Runs.NextIterationIndex(_profileId),
                Generation = generation,
                CandidateIndex = candidateIndex,
                GenomeJson = vectorJson,
                Outcome = result.Outcome.ToString(),
                FinalScore = result.Score,
                Days = result.Days,
                Miles = result.Miles,
                Survivors = result.Survivors,
                CauseOfDeath = string.IsNullOrEmpty(result.CauseOfDeath) ? null : result.CauseOfDeath,
                MadeTop10 = madeTop10,
                Fitness = fitness
            });

            _db.Profiles.AddIterations(_profileId, 1);

            if (madeTop10)
                _db.Leaderboard.Insert(new LeaderboardEntry
                {
                    ProfileId = _profileId,
                    RunId = runId,
                    Name = _profileName, // bot's own board -> plain profile name (no "(bot)" suffix)
                    Score = result.Score,
                    Rating = Rating(result.Score)
                });
        }

        private void PersistOptimizer()
        {
            // The genome saved for replay/leaderboard is the optimizer's ROBUST champion — its best vector by shaped fitness
            // (averaged over the generation's common-random-numbers seeds), falling back to its current best-guess mean. This
            // replaces the old "single luckiest raw-score game" genome, which under un-seeded noise was mostly luck rather than
            // a policy worth watching. The best RAW SCORE is still tracked separately (for the "best ever" stat and leaderboard).
            var bestVector = _optimizer.BestVector ?? _optimizer.MeanVector();
            var bestGenomeJson = JsonSerializer.Serialize(bestVector);
            _db.Profiles.SaveLearningState(_profileId, _optimizer.Serialize(), _optimizer.Generation, _bestScoreEver, bestGenomeJson);
        }

        // The objective is to arrive with as many people as possible in the best health, so fitness rewards that for EVERY
        // run - finished or not - rather than the old distance-only reward for failures, which taught policies to buy oxen,
        // floor it, and ignore the party's health (medicine, clothing and spare parts never paid off under that signal). The
        // components, in priority order:
        //   - partyHealth: survivors weighted by their average health (0..2500). This is the heart of the goal and is the only
        //     signal that separates "kept 4 people alive" from "lost everyone", giving a gradient toward survival everywhere.
        //   - finishBonus: a flat bonus plus the real game score, so actually reaching Oregon dominates and finishers are still
        //     ranked among themselves by the true score the leaderboard records.
        //   - progress: a gentle distance tie-breaker so that, among equally-alive parties, getting further still ranks higher.
        // The leaderboard and best-score tracking continue to use the real game score (result.Score) untouched; only the
        // optimizer's search objective is shaped here.
        internal static double Fitness(RunResult result)
        {
            var partySize = result.PartySize > 0 ? result.PartySize : GameSimulationApp.MAXPLAYERS;
            var deaths = Math.Max(0, partySize - result.Survivors);

            // Survivors weighted SUPER-LINEARLY (survivors^2) by their average health, normalised by party size so a full,
            // healthy party scores 2500. This is the heart of the objective and is now credited on EVERY run - finished,
            // timed-out, died, OR stranded - so a party that kept people alive always out-scores one that lost them and there
            // is a smooth survival gradient everywhere. Previously a stranded-but-alive party fell into the distance-only
            // branch and got NO credit for the members it kept alive - a ~2500-point cliff that made fitness mostly luck.
            // Adding this term to both branches removes the cliff while leaving the finisher ordering byte-for-byte unchanged.
            var survivalReward = (double) result.Survivors * result.Survivors * result.PartyHealthValue / partySize;

            if (result.IsFinished)
            {
                // The party reached Oregon (or ran the clock out still on the trail). A flat bonus plus the true game score
                // make finishing dominate and rank finishers among themselves, and a HEAVY per-death penalty pushes the
                // optimizer to bring the WHOLE party through. (The penalty is capped near 800: pushed much higher a
                // low-survivor finish would score worse than not finishing at all, which would perversely discourage winning.)
                return 2000 + result.Score + survivalReward - deaths * 800.0;
            }

            // Died or stranded before Oregon. Forward progress is a gentle tie-breaker on top of the survival reward above,
            // and the death penalty here stays LIGHT: heavy enough that throwing lives away is never free, but not so heavy
            // that a doomed party is better off parking the wagon than pressing on. A penalty that dominates this branch
            // collapses training - the optimizer starves the oxen budget to avoid deaths and strands the wagon short of Oregon.
            return result.Miles * 0.25 + survivalReward - deaths * 100.0;
        }

        public static string Rating(int score) => score >= 7000 ? "Trail Guide" : score >= 3000 ? "Adventurer" : "Greenhorn";
    }
}
