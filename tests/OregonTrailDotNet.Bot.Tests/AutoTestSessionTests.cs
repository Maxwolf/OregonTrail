using OregonTrailDotNet.Bot.Diagnostics;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using OregonTrailDotNet.Bot.Testing;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Covers the automated-testing session loop with an injected game-runner, so the fan-out over every model, the
    ///     stop-on-problem default, the keep-going mode, and the generated report are all verified without booting a game.
    /// </summary>
    public sealed class AutoTestSessionTests
    {
        private static RunResult Clean(GameOutcomeEnum outcome = GameOutcomeEnum.Death) => new()
        {
            Outcome = outcome,
            Score = outcome == GameOutcomeEnum.Win ? 1000 : 0,
            Days = 100,
            Miles = 1200,
            Survivors = outcome == GameOutcomeEnum.Win ? 4 : 0
        };

        private static RunResult Buggy(BugCategoryEnum category, string detail) => new()
        {
            Outcome = GameOutcomeEnum.Aborted,
            Bug = new BugReport { Category = category, Detail = detail, WindowType = "Travel", FormType = "SomeForm" }
        };

        [Fact]
        public void Runs_One_Of_Every_Model_And_Reports_No_Problems_When_Games_Are_Clean()
        {
            var played = 0;
            var session = new AutoTestSession(5, stopOnProblem: false, playGame: _ =>
            {
                played++;
                return Clean();
            });

            var report = session.Run(keepRunning: () => played < 12);

            Assert.Equal(12, report.TotalGames);
            Assert.Empty(report.Problems);
            Assert.Equal(TrainingModels.All.Count, report.Models.Count); // one of every model
            Assert.All(report.Models, m => Assert.True(m.Games > 0));

            var text = report.Format();
            Assert.Contains("No problems found", text);
            Assert.Contains("Cross-Entropy Method", text);
            Assert.Contains("Neuro-Evolution", text);
        }

        [Fact]
        public void Stops_At_The_First_Problem_When_Configured()
        {
            // Default behaviour: stop the moment a problem is found.
            var session = new AutoTestSession(0, stopOnProblem: true, playGame: model =>
                model.Key == "hillclimb" ? Buggy(BugCategoryEnum.SoftLock, "stuck at the river") : Clean());

            var report = session.Run(keepRunning: () => true);

            Assert.True(report.StoppedOnProblem);
            Assert.Single(report.Problems);
            Assert.Equal("Hill Climber", report.Problems[0].Model);
            Assert.Equal("SoftLock", report.Problems[0].Category);
            // Models listed after the offender never played, so the session really did stop early.
            Assert.Equal(0, report.Models.First(m => m.Key == "random").Games);
            Assert.Contains("PROBLEMS (1)", report.Format());
            Assert.Contains("stuck at the river", report.Format());
        }

        [Fact]
        public void Keeps_Going_And_Logs_Every_Problem_When_Not_Stopping()
        {
            var played = 0;
            var session = new AutoTestSession(5, stopOnProblem: false, playGame: _ =>
            {
                played++;
                return Buggy(BugCategoryEnum.Crash, "boom");
            });

            var report = session.Run(keepRunning: () => played < 8);

            Assert.False(report.StoppedOnProblem);
            Assert.Equal(8, report.TotalGames);
            Assert.Equal(report.TotalGames, report.Problems.Count); // every game was a problem, all logged
            Assert.All(report.Models, m => Assert.Equal(m.Games, m.Problems));
        }
    }
}
