using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Confirms the bot names its party "&lt;bot&gt; 1..5" (e.g. "Trailblazer 1", "Trailblazer 2", ...) so a viewer can
    ///     tell which member died. Crew #1 is the leader, and its name is what brands the in-game high-score list.
    /// </summary>
    public sealed class PartyNamingTests : IDisposable
    {
        public void Dispose() => GameSimulationApp.Instance?.Destroy();

        [Fact]
        public void Party_Members_Are_Named_Base_1_Through_5()
        {
            var policy = new HeuristicPolicy(); // LeaderName == "Trailblazer (bot)" -> base "Trailblazer"

            List<string>? names = null;
            bool? firstIsLeader = null;
            string? leaderName = null;

            // Abort the run the instant the full five-person party exists (the confirm screen builds it from the entered
            // names), capturing the assigned names before the game plays on and anyone dies.
            GamePlayer.PlayOnce(policy, watch: null, shouldAbort: () =>
            {
                var vehicle = GameSimulationApp.Instance?.Vehicle;
                if (vehicle is null || vehicle.Passengers.Count < GameSimulationApp.MAXPLAYERS)
                    return false;

                names = vehicle.Passengers.Select(p => p.Name).ToList();
                firstIsLeader = vehicle.Passengers.First().Leader;
                leaderName = vehicle.PassengerLeader?.Name;
                return true;
            });

            Assert.Equal(
                new[] { "Trailblazer 1", "Trailblazer 2", "Trailblazer 3", "Trailblazer 4", "Trailblazer 5" }, names);
            Assert.True(firstIsLeader); // crew #1 is always the leader
            Assert.Equal("Trailblazer 1", leaderName); // the leader's name is what the high-score list will show
        }
    }
}
