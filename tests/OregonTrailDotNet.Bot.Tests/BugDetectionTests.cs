using System.Reflection;
using OregonTrailDotNet;
using OregonTrailDotNet.Bot.Diagnostics;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Window.Travel;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>Verifies the bot stops and produces a developer report when a game/driving error occurs.</summary>
    public sealed class BugDetectionTests : IDisposable
    {
        static BugDetectionTests() => Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);
        public void Dispose() => GameSimulationApp.Instance?.Destroy();

        [Fact]
        public void An_Exception_While_Driving_Is_Caught_As_A_Crash_Report()
        {
            var result = GamePlayer.PlayOnce(new ThrowingPolicy());

            Assert.Equal(GameOutcomeEnum.Aborted, result.Outcome);
            Assert.NotNull(result.Bug);
            Assert.Equal(BugCategoryEnum.Crash, result.Bug!.Category);
            Assert.Contains("boom in ChooseTravel", result.Bug.ExceptionDetail);

            var formatted = result.Bug.Format();
            Assert.Contains("BOT STOPPED", formatted);
            Assert.Contains("Vehicle state:", formatted);
            Assert.Contains("Last inputs", formatted);
        }

        // Delegates real setup to the heuristic but blows up on the first travel decision, mid-game.
        private sealed class ThrowingPolicy : IPolicy
        {
            private readonly HeuristicPolicy _inner = new();
            public string Name => "throwing";
            public int Profession => _inner.Profession;
            public int StartMonth => _inner.StartMonth;
            public string LeaderName => "Crasher (bot)";
            public int TargetQuantity(EntitiesEnum item, GameSnapshot state) => _inner.TargetQuantity(item, state);
            public TravelCommandsEnum ChooseTravel(GameSnapshot state, IReadOnlyCollection<TravelCommandsEnum> available)
                => throw new InvalidOperationException("boom in ChooseTravel");
            public int Pace(GameSnapshot state) => _inner.Pace(state);
            public int Ration(GameSnapshot state) => _inner.Ration(state);
            public int RestDays(GameSnapshot state) => _inner.RestDays(state);
            public bool YesNo(string formName, GameSnapshot state) => _inner.YesNo(formName, state);
            public RiverChoiceKindEnum River(GameSnapshot state, IReadOnlyCollection<RiverChoiceKindEnum> options) => _inner.River(state, options);
            public int Fork(GameSnapshot state, int branchCount) => _inner.Fork(state, branchCount);
        }
    }
}
