using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Window.Travel;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Every party member counts toward the score, so the policies must protect the weakest individual — not just react to
    ///     the party average, which can stay high while one member is dying.
    /// </summary>
    public sealed class PartySurvivalTests
    {
        private static readonly TravelCommands[] Available =
            { TravelCommands.ContinueOnTrail, TravelCommands.StopToRest, TravelCommands.HuntForFood };

        // Party average looks fine, but one member is failing.
        private static GameSnapshot OneMemberFailing() => new()
        {
            Health = HealthStatus.Good,
            LowestHealth = HealthStatus.Poor,
            Medicine = 5,
            Food = 500,
            DaysRemaining = 120,
            LivingCount = 4,
            PartySize = 4
        };

        [Fact]
        public void Heuristic_Rests_To_Save_A_Single_Failing_Member()
        {
            Assert.Equal(TravelCommands.StopToRest, new HeuristicPolicy().ChooseTravel(OneMemberFailing(), Available));
        }

        [Fact]
        public void Genome_Rests_To_Save_A_Single_Failing_Member()
        {
            // The default genome's rest threshold is Poor (300); the weakest member at Poor should trigger a rest.
            var policy = new GenomePolicy(StrategyGenome.Default(), "Weighted (bot)");
            Assert.Equal(TravelCommands.StopToRest, policy.ChooseTravel(OneMemberFailing(), Available));
        }

        [Fact]
        public void A_Fully_Healthy_Party_Keeps_Moving()
        {
            var healthy = new GameSnapshot
            {
                Health = HealthStatus.Good,
                LowestHealth = HealthStatus.Good,
                Medicine = 5,
                Food = 500,
                DaysRemaining = 120,
                LivingCount = 4,
                PartySize = 4
            };

            Assert.Equal(TravelCommands.ContinueOnTrail, new HeuristicPolicy().ChooseTravel(healthy, Available));
            Assert.Equal(TravelCommands.ContinueOnTrail,
                new GenomePolicy(StrategyGenome.Default(), "Weighted (bot)").ChooseTravel(healthy, Available));
        }
    }
}
