using System.Text.Json;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Diagnostics;
using OregonTrailDotNet.Bot.Game;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     Runs training for a profile using whichever <see cref="ITrainingModel" /> the profile was created with. Each
    ///     generation it samples candidate vectors from the model's optimizer, decodes each into a policy, evaluates it over K
    ///     games (averaging out the game's randomness), and folds the scores back in. Everything is persisted (each game to
    ///     <c>runs</c>, the optimizer state + best score/vector to the profile, qualifying finishes to the leaderboard) and it
    ///     resumes from the profile's saved learning state. Fully headless and deterministic per tick.
    /// </summary>
    public sealed class TrainingSession
    {
        /// <summary>Bumped whenever <see cref="Fitness" /> changes scale or semantics. Optimizers stamp it into their
        ///     persisted learning state so a champion's BestFitness measured under an old objective is never compared against
        ///     new-scale fitness on resume — a stale cross-scale champion could otherwise never be displaced and the profile
        ///     would keep replaying it as its "best" genome forever.</summary>
        internal const int FitnessVersion = 5;

        /// <summary>Weight of the real game score inside the win branch of <see cref="Fitness" />. At 1x the optimizer
        ///     settled on the most RELIABLE win it could find — Banker (x1 multiplier, $1600 restocking warchest), ~2750
        ///     points tops — because higher-multiplier professions win somewhat less often and lost on expected value. The
        ///     amplified weight lowers the win-rate bar a score-chasing strategy must clear: at 3x, a Carpenter (x2) beats a
        ///     58%-win-rate Banker from ~35% win rate, and a Meek-grade Farmer (x3, 7650) from ~21% — pushing training
        ///     toward the best score AND winning, not merely finishing. Pushed much higher, one lucky high-score win would
        ///     dominate a candidate's 64-game mean and selection would degrade into score-lottery noise.</summary>
        internal const double WinScoreWeight = 3.0;

        /// <summary>Weight of a candidate's single best WINNING score, added on top of its mean fitness. The mean is an
        ///     expected-value objective, and measured play shows the reliable Banker (43% wins, ~1300-point wins) beats the
        ///     score-multiplier professions on EV no matter how the per-game score is weighted — Carpenter's 1.7x score
        ///     edge never overcomes its 2.3x win-rate deficit, so training kept abandoning high-score play. Crediting the
        ///     best win a candidate produced (every candidate plays the same seeds, so the comparison is fair) rewards the
        ///     high-ceiling strategies for their peaks and keeps them in the gene pool. Wins only: a Timeout's partial
        ///     score must never feed this, or the park-and-stall exploit returns through the back door.</summary>
        internal const double MaxScoreBonusWeight = 1.0;

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
        ///     means "run open-endedly" — the loop only ends when <paramref name="shouldStop" /> says so (the Esc/Ctrl+C hook).
        ///     <paramref name="shouldStop" /> is polled between GAMES, not just between generations: a stop mid-batch abandons
        ///     the in-progress generation and discards its partial results (the optimizer can only learn from a fully scored
        ///     generation, and a half-evaluated one would otherwise pollute the run history and get replayed on resume).
        ///     <paramref name="onGame" /> fires after every single game inside a generation, for a live progress bar.</summary>
        public void Run(Action<GenerationProgress>? onGeneration = null, Func<bool>? shouldStop = null,
            Action<GenerationTick>? onGame = null)
        {
            for (var g = 0; _config.Generations < 0 || g < _config.Generations; g++)
            {
                var generationNumber = _optimizer.Generation;
                var candidates = _optimizer.Sample();
                var scored = new List<(double[] Vector, double Fitness)>(candidates.Count);

                // First iteration index this generation will write, so an abandoned batch can be surgically removed.
                var generationStartIteration = _db.Runs.NextIterationIndex(_profileId);
                var gamesRecorded = 0;
                var abandoned = false;

                var wins = 0;
                var bestScoreThisGen = 0;

                // Common random numbers: draw one set of game seeds for this generation and evaluate EVERY candidate on the
                // same seeds. Pairing the evaluations this way cancels most of the game's luck from the comparison, so the
                // fitness differences the optimizer selects on reflect the genome rather than which candidate drew easy games.
                var seedRng = new Random(unchecked(_config.EvaluationSeed * 1000003 + generationNumber));
                var seeds = new int[_config.GamesPerCandidate];
                for (var i = 0; i < seeds.Length; i++)
                    seeds[i] = seedRng.Next();

                for (var candidate = 0; candidate < candidates.Count && !abandoned; candidate++)
                {
                    var vector = candidates[candidate];
                    var vectorJson = JsonSerializer.Serialize(vector);
                    double fitnessSum = 0;
                    var bestWinScore = 0;

                    for (var k = 0; k < _config.GamesPerCandidate; k++)
                    {
                        // Esc lands here, between games, so stopping never waits out the rest of a ~1000-game batch.
                        if (shouldStop?.Invoke() == true)
                        {
                            abandoned = true;
                            break;
                        }

                        var policy = _model.Decode(vector, _leaderName);
                        var result = _playGame(policy, seeds[k]);
                        var fitness = Fitness(result);

                        RecordRun(result, generationNumber, candidate, vectorJson, fitness);
                        gamesRecorded++;

                        // Unambiguous bugs (a crash, a broken invariant, or a screen the bot has no handler for) stop the whole
                        // batch so a developer can fix them. A plain soft-lock is treated as a failed game (recorded with the
                        // ordinary non-finish fitness for its end state) so one unlucky stuck game doesn't abort a long run.
                        if (result.Bug != null && result.Bug.Category != BugCategoryEnum.SoftLock)
                        {
                            PersistOptimizer();
                            throw new BotBugException(result.Bug);
                        }

                        if (result.Outcome == GameOutcomeEnum.Win)
                        {
                            wins++;
                            bestWinScore = Math.Max(bestWinScore, result.Score);
                        }

                        bestScoreThisGen = Math.Max(bestScoreThisGen, result.Score);
                        if (result.Score > _bestScoreEver)
                            _bestScoreEver = result.Score;

                        fitnessSum += fitness;

                        onGame?.Invoke(new GenerationTick(generationNumber,
                            candidate * _config.GamesPerCandidate + k + 1,
                            candidates.Count * _config.GamesPerCandidate, wins));
                    }

                    if (abandoned)
                        break;

                    scored.Add((vector,
                        fitnessSum / _config.GamesPerCandidate + MaxScoreBonusWeight * bestWinScore));
                }

                if (abandoned)
                {
                    // Discard the partial generation: remove its recorded games and rewind the profile's game counter so
                    // the run history only ever holds fully evaluated generations (the same generation number replays with
                    // the same common-random-number seeds on resume). The optimizer never saw the partial scores, so
                    // persisting just re-saves its unchanged state plus any best-score a partial game legitimately set.
                    _db.Runs.DeleteFromIteration(_profileId, generationStartIteration);
                    _db.Profiles.AddIterations(_profileId, -gamesRecorded);
                    PersistOptimizer();
                    return;
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

        // The objective is to REACH OREGON with the highest score the game can award - a full healthy party, the x3
        // profession, supplies still in the wagon. Fitness shapes that in strict priority order:
        //   - winning: only an actual Win earns the finish bonus, sized so that EVERY win outranks EVERY non-win. (Timeout
        //     used to share this branch, which taught the optimizer that a fat party idling out the 246-day clock beats
        //     trying to finish - stalling was the accessible optimum, so nothing ever learned to push for Oregon.)
        //   - score, amplified (see WinScoreWeight): among wins, the game's own tally is the objective, so a Meek-grade
        //     finish beats grinding out the cheapest reliable one.
        //   - partyHealth: survivors weighted by their average health (0..2500), credited on every run and — off the finish
        //     line — scaled by trail progress, so "keep them alive AND get them down the trail" is the gradient rather than
        //     "park at the trailhead and keep everyone fat".
        //   - progress: a gentle distance tie-breaker so that, among equally-alive parties, getting further still ranks higher.
        // The leaderboard and best-score tracking continue to use the real game score (result.Score) untouched; only the
        // optimizer's search objective is shaped here.
        internal static double Fitness(RunResult result)
        {
            var partySize = result.PartySize > 0 ? result.PartySize : GameSimulationApp.MAXPLAYERS;
            var deaths = Math.Max(0, partySize - result.Survivors);

            // Survivors weighted SUPER-LINEARLY (survivors^2) by their average health, normalised by party size so a full,
            // healthy party scores 2500. This is credited on EVERY run - won, timed-out, died, OR stranded - so a party that
            // kept people alive always out-scores one that lost them and there is a smooth survival gradient everywhere.
            var survivalReward = (double) result.Survivors * result.Survivors * result.PartyHealthValue / partySize;

            if (result.Outcome == GameOutcomeEnum.Win)
            {
                // Reaching Oregon strictly dominates: the flat bonus exceeds the best possible non-win fitness (500 progress
                // + 2500 survival = 3000) even after the worst-case death penalty (4 x 150 = 600) — the score term only adds
                // (it is never negative) — so the optimizer can never prefer stalling or parking over a genuine finish.
                // Within wins, the AMPLIFIED game score is the objective: it packages everything a top finish means
                // (survivors x health x profession multiplier, plus supplies and cash carried through), so weighting it up
                // steers training toward Meek-grade scores instead of the cheapest reliable finish. The per-death penalty
                // stays LIGHT because a heavy one made a costly win rank below not finishing, which discouraged winning.
                return 4000 + WinScoreWeight * result.Score + survivalReward - deaths * 150.0;
            }

            // Timed out, died, or stranded before Oregon - the party did not finish, so no finish bonus. The survival reward
            // is scaled by trail progress: without this, a party that parks at the trailhead and strands with five fat
            // members out-scores every genuine attempt (observed in real training - fitness climbed while miles FELL to ~320
            // and wins hit zero), because full survival credit (up to 2500) dwarfs anything progress or rare wins can offer.
            // Keeping 15% everywhere preserves a survival gradient even for early deaths; the death penalty stays LIGHT so
            // a doomed party is still better off pressing on than parking the wagon.
            var progress = Math.Min(1.0, result.Miles / 2000.0);
            return result.Miles * 0.25 + survivalReward * (0.15 + 0.85 * progress) - deaths * 100.0;
        }

        public static string Rating(int score) => score >= 7000 ? "Trail Guide" : score >= 3000 ? "Adventurer" : "Greenhorn";
    }
}
