using System.Reflection;
using OregonTrailDotNet;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Confirms the bot numbers its party so a viewer can tell who died: crew #1 is the leader (keeping its scored
    ///     "(bot)" name, which brands the in-game high-score list) and the three other members are named 2, 3, 4.
    /// </summary>
    public sealed class PartyNamingTests : IDisposable
    {
        static PartyNamingTests() => Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);
        public void Dispose() => GameSimulationApp.Instance?.Destroy();

        [Fact]
        public void Leader_Keeps_Its_Name_And_Members_Are_Numbered_2_3_4()
        {
            var policy = new HeuristicPolicy(); // LeaderName == "Trailblazer (bot)"

            List<string>? names = null;
            bool? firstIsLeader = null;
            string? leaderName = null;

            // Abort the run the instant the full four-person party exists (the confirm screen builds it from the entered
            // names), capturing the assigned names before the game plays on and anyone dies.
            GamePlayer.PlayOnce(policy, watch: null, shouldAbort: () =>
            {
                var vehicle = GameSimulationApp.Instance?.Vehicle;
                if (vehicle is null || vehicle.Passengers.Count < 4)
                    return false;

                names = vehicle.Passengers.Select(p => p.Name).ToList();
                firstIsLeader = vehicle.Passengers.First().Leader;
                leaderName = vehicle.PassengerLeader?.Name;
                return true;
            });

            Assert.Equal(new[] { "Trailblazer (bot)", "2", "3", "4" }, names);
            Assert.True(firstIsLeader); // crew #1 is always the leader
            Assert.Equal("Trailblazer (bot)", leaderName); // the (bot) leader name is what the high-score list will show
        }
    }
}
