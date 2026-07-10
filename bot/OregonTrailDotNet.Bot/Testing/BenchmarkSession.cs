using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;

namespace OregonTrailDotNet.Bot.Testing
{
    /// <summary>
    ///     Benchmarks the training models against each other by measuring how long each takes to score its first win. It plays
    ///     one game of every model per round (fair, equal throughput) using the same fuzzing as automated testing, and stops
    ///     when every model has won or the caller says to stop. The models, game-runner, and clock are injectable so the loop
    ///     can be unit-tested without booting a real game or waiting on the wall clock.
    /// </summary>
    public sealed class BenchmarkSession
    {
        private readonly int _configuredMinutes;
        private readonly string _goalLabel;
        private readonly Func<RunResult, bool> _reachedGoal;
        private readonly IReadOnlyList<ITrainingModel> _models;
        private readonly Func<ITrainingModel, RunResult> _playGame;
        private readonly Func<TimeSpan> _elapsed;

        public BenchmarkSession(int configuredMinutes, string goalLabel, Func<RunResult, bool> reachedGoal,
            IReadOnlyList<ITrainingModel>? models = null,
            Func<ITrainingModel, RunResult>? playGame = null,
            Func<TimeSpan>? elapsed = null)
        {
            _configuredMinutes = configuredMinutes;
            _goalLabel = goalLabel;
            _reachedGoal = reachedGoal;
            _models = models ?? TrainingModels.All;
            _playGame = playGame ?? new FuzzPlayer(_models).Play;
            _elapsed = elapsed ?? RealClock();
        }

        /// <summary>
        ///     Plays games — one of every model per round — until every model has reached the goal or
        ///     <paramref name="keepRunning" /> returns false. <paramref name="onProgress" /> fires after every game.
        /// </summary>
        public BenchmarkReport Run(Func<bool> keepRunning, Action<BenchmarkReport>? onProgress = null)
        {
            var report = new BenchmarkReport(_models, _configuredMinutes, _goalLabel);

            while (!report.AllReached && keepRunning())
            {
                foreach (var model in _models)
                {
                    if (report.AllReached || !keepRunning())
                    {
                        report.MarkFinished();
                        return report;
                    }

                    var result = _playGame(model);
                    report.Record(model, _reachedGoal(result), result.Score, _elapsed());
                    onProgress?.Invoke(report);
                }
            }

            report.MarkFinished();
            return report;
        }

        private static Func<TimeSpan> RealClock()
        {
            var start = DateTime.UtcNow;
            return () => DateTime.UtcNow - start;
        }
    }
}
