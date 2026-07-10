using System.Reflection;
using OregonTrailDotNet;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using OregonTrailDotNet.Entity.Person;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>Covers the "watch a game" narration and confirms the paced/rendered code path still drives a full game.</summary>
    public sealed class WatchModeTests : IDisposable
    {
        static WatchModeTests() => Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);
        public void Dispose() => GameSimulationApp.Instance?.Destroy();

        [Fact]
        public void Narration_Explains_Decisions()
        {
            var lowFood = new GameSnapshot { Food = 40, Health = HealthStatus.Fair };

            Assert.Equal("Setting off on the Oregon Trail", WatchNarration.Describe("MainMenu", "", "1", lowFood));

            var hunt = WatchNarration.Describe("Travel", "", "8", lowFood); // TravelCommands.HuntForFood == 8
            Assert.Contains("hunting", hunt, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("40", hunt);

            Assert.Contains("carpenter", WatchNarration.Describe("", "ProfessionSelector", "2", lowFood));
            Assert.Contains("oxen", WatchNarration.Describe("", "Store", "1", lowFood));
            Assert.Contains("May", WatchNarration.Describe("", "SelectStartingMonthState", "3", lowFood));
            Assert.Contains("toll", WatchNarration.Describe("", "TollRoadQuestion", "Y", lowFood));
            Assert.Contains("Rolling", WatchNarration.PromptStatus("ContinueOnTrail"));
        }

        [Fact]
        public void Speed_Presets_Order_From_Fastest_To_Slowest()
        {
            var fast = WatchOptions.ForSpeed(WatchSpeed.Fast);
            var medium = WatchOptions.ForSpeed(WatchSpeed.Medium);
            var slow = WatchOptions.ForSpeed(WatchSpeed.Slow);

            // Each slower step waits longer, both per tick and at each decision, so playback is visibly slower.
            Assert.True(fast.TickDelayMs < medium.TickDelayMs && medium.TickDelayMs < slow.TickDelayMs);
            Assert.True(fast.DecisionPauseMs < medium.DecisionPauseMs && medium.DecisionPauseMs < slow.DecisionPauseMs);

            // Medium is the established default.
            Assert.Equal(WatchOptions.Default.TickDelayMs, medium.TickDelayMs);
            Assert.Equal(WatchOptions.Default.DecisionPauseMs, medium.DecisionPauseMs);
        }

        [Fact]
        public void Watch_Path_Drives_A_Full_Game()
        {
            // Zero delays keep the test fast; rendering is a no-op under a redirected console. This just proves the extra
            // narrate/repaint work in watch mode doesn't disturb the driving logic.
            var options = new WatchOptions { TickDelayMs = 0, DecisionPauseMs = 0, Narrate = true };
            var result = GamePlayer.PlayOnce(new HeuristicPolicy(), options);

            Assert.True(result.Bug is null || result.Bug.Category == Diagnostics.BugCategory.SoftLock);
            Assert.NotEqual(0, result.Days); // it actually played
        }
    }
}
