using System.Reflection;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Person;
using Xunit;
using PersonEntity = OregonTrailDotNet.Entity.Person.Person;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Locks in the "Stephen Meek's 7650 is reachable" fidelity fix. A full five-person Farmer party arriving in Good
    ///     health with its wagon scores exactly 7650 - (5 x 500 health + 50 wagon) x 3 - through the real
    ///     <see cref="ScoreCalculator" /> (which mirrors the game's FinalPoints line-for-line). The two former divergences
    ///     would each miss it: a four-person party caps the base at 2050 (=> 6150), and a wagon worth 0 drops it to 7500.
    /// </summary>
    public sealed class MeekReachabilityTests : IDisposable
    {
        static MeekReachabilityTests() => Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);
        public void Dispose() => GameSimulationApp.Instance?.Destroy();

        [Fact]
        public void Perfect_Farmer_Party_Scores_Exactly_Meek_7650()
        {
            using var driver = new GameDriver();
            driver.Boot();

            var vehicle = GameSimulationApp.Instance!.Vehicle;
            vehicle.ResetVehicle(0); // clear inventory + cash so only the party and the wagon contribute points

            // A full party of Farmers, everyone starting at Good health; crew #1 is the leader (its profession is the multiplier).
            vehicle.AddPerson(new PersonEntity(Profession.Farmer, "Trailblazer 1", true));
            for (var i = 2; i <= GameSimulationApp.MAXPLAYERS; i++)
                vehicle.AddPerson(new PersonEntity(Profession.Farmer, $"Trailblazer {i}", false));

            Assert.Equal(GameSimulationApp.MAXPLAYERS, vehicle.PassengerLivingCount);
            Assert.Equal(HealthStatus.Good, vehicle.PassengerHealthStatus);
            Assert.Equal(50, Resources.Vehicle.PointsAwarded); // the wagon is worth 50 (previously scored 0)

            // base = 5 x 500 (Good) + 50 (wagon) = 2550; Farmer x3 = 7650, exactly Stephen Meek's seeded record.
            Assert.Equal(2550, ScoreCalculator.ComputeBase(vehicle));
            Assert.Equal(7650, ScoreCalculator.Compute(vehicle));
        }
    }
}
