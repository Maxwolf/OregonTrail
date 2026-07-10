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
            _playGame = playGame ?? new FuzzPlayer(_models).Play;
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
    }
}
