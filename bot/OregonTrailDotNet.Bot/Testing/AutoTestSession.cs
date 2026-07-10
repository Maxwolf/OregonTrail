using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;

namespace OregonTrailDotNet.Bot.Testing
{
    /// <summary>
    ///     Soak/fuzz tester: repeatedly plays one game of every training model against the real game, watching for crashes,
    ///     soft-locks, screens the bot can't answer, and broken invariants. It is purely a diagnostic and never touches the
    ///     profiles/leaderboard database. The models and the game-playing function are injectable so the loop can be unit
    ///     tested without booting a real game.
    /// </summary>
    public sealed class AutoTestSession
    {
        // How many varied candidate vectors to draw from a model at a time before drawing another batch.
        private const int SampleBatch = 16;

        private readonly int _configuredMinutes;
        private readonly bool _stopOnProblem;
        private readonly IReadOnlyList<ITrainingModel> _models;
        private readonly Func<ITrainingModel, RunResult> _playGame;

        public AutoTestSession(int configuredMinutes, bool stopOnProblem,
            IReadOnlyList<ITrainingModel>? models = null,
            Func<ITrainingModel, RunResult>? playGame = null)
        {
            _configuredMinutes = configuredMinutes;
            _stopOnProblem = stopOnProblem;
            _models = models ?? TrainingModels.All;
            _playGame = playGame ?? BuildFuzzPlayer(_models);
        }

        /// <summary>
        ///     Plays games — one of every model per round — until <paramref name="keepRunning" /> returns false, or (when
        ///     stop-on-problem is on) the first problem is found. <paramref name="onProgress" /> fires after every game with the
        ///     live report so a caller can render a dashboard.
        /// </summary>
        public AutoTestReport Run(Func<bool> keepRunning, Action<AutoTestReport>? onProgress = null)
        {
            var report = new AutoTestReport(_models, _configuredMinutes, _stopOnProblem);

            while (keepRunning())
            {
                foreach (var model in _models)
                {
                    if (!keepRunning())
                    {
                        report.MarkFinished();
                        return report;
                    }

                    var result = _playGame(model);
                    var problem = report.Record(model, result);
                    onProgress?.Invoke(report);

                    if (problem != null && _stopOnProblem)
                    {
                        report.StoppedOnProblem = true;
                        report.EndReason = $"Stopped after the first problem ({problem.Category} in {problem.Model}).";
                        report.MarkFinished();
                        return report;
                    }
                }
            }

            report.MarkFinished();
            return report;
        }

        // Draws varied policies for each model from a fresh optimizer, so the fuzzing explores many different game states
        // (rather than replaying one fixed strategy). No learning happens — candidates just keep being sampled.
        private static Func<ITrainingModel, RunResult> BuildFuzzPlayer(IReadOnlyList<ITrainingModel> models)
        {
            var optimizers = models.ToDictionary(m => m.Key, m => m.CreateOptimizer(SampleBatch));
            var queues = models.ToDictionary(m => m.Key, _ => new Queue<double[]>());

            return model =>
            {
                var queue = queues[model.Key];
                if (queue.Count == 0)
                    foreach (var vector in optimizers[model.Key].Sample())
                        queue.Enqueue(vector);

                var candidate = queue.Count > 0 ? queue.Dequeue() : model.InitialMean();
                var policy = model.Decode(candidate, $"{model.DisplayName} (test)");
                return GamePlayer.PlayOnce(policy);
            };
        }
    }
}
