using OregonTrailDotNet;
using OregonTrailDotNet.Bot.Learning;
using OregonTrailDotNet.Bot.Testing;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>Runs the real automated-testing pipeline (fuzzed policies driving live games) for one round across every model.</summary>
    public sealed class AutoTestIntegrationTests : IDisposable
    {
        public void Dispose() => GameSimulationApp.Instance?.Destroy();

        [Fact]
        public void A_Short_Real_Session_Plays_One_Game_Of_Every_Model()
        {
            var games = 0;
            var modelCount = TrainingModels.All.Count;
            var session = new AutoTestSession(0, stopOnProblem: false); // real fuzz player + real games

            // Stop after one full round (one game per model); onProgress fires once per completed game.
            var report = session.Run(keepRunning: () => games < modelCount, onProgress: _ => games++);

            Assert.Equal(modelCount, report.TotalGames);
            Assert.All(report.Models, m => Assert.Equal(1, m.Games)); // exactly one of every model actually ran
            Assert.Contains("AUTOMATED TESTING REPORT", report.Format());
        }
    }
}
