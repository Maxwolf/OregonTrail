using System.Reflection;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;
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
            vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, "Trailblazer 1", true));
            for (var i = 2; i <= GameSimulationApp.MAXPLAYERS; i++)
                vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, $"Trailblazer {i}", false));

            Assert.Equal(GameSimulationApp.MAXPLAYERS, vehicle.PassengerLivingCount);
            Assert.Equal(HealthStatusEnum.Good, vehicle.PassengerHealthStatus);
            Assert.Equal(50, Resources.Vehicle.PointsAwarded); // the wagon is worth 50 (previously scored 0)

            // base = 5 x 500 (Good) + 50 (wagon) = 2550; Farmer x3 = 7650, exactly Stephen Meek's seeded record.
            Assert.Equal(2550, ScoreCalculator.ComputeBase(vehicle));
            Assert.Equal(7650, ScoreCalculator.Compute(vehicle));
        }

        /// <summary>
        ///     Locks in the 1985-faithful score ceiling. With every scored item at its original cap — 20 oxen, 3 of each
        ///     spare part, 255 sets of clothing, 65,535 bullets, 2,000 lb food, $360 leftover cash — a full Good-health
        ///     Farmer party scores exactly (2500+50+80+18+510+1310+80+72) x 3 = 13,860, matching the maximum computed
        ///     from the decompiled Apple II disk. Quantities are added far past each cap on purpose: the inventory clamps
        ///     are part of what this test pins.
        /// </summary>
        [Fact]
        public void Perfect_Grind_Farmer_Party_Scores_The_1985_Ceiling_13860()
        {
            using var driver = new GameDriver();
            driver.Boot();

            var vehicle = GameSimulationApp.Instance!.Vehicle;
            vehicle.ResetVehicle(0);

            vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, "Trailblazer 1", true));
            for (var i = 2; i <= GameSimulationApp.MAXPLAYERS; i++)
                vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, $"Trailblazer {i}", false));

            var inv = vehicle.Inventory;
            inv[EntitiesEnum.Animal].AddQuantity(999);
            inv[EntitiesEnum.Wheel].AddQuantity(99);
            inv[EntitiesEnum.Axle].AddQuantity(99);
            inv[EntitiesEnum.Tongue].AddQuantity(99);
            inv[EntitiesEnum.Clothes].AddQuantity(9999);
            inv[EntitiesEnum.Ammo].AddQuantity(999999);
            inv[EntitiesEnum.Food].AddQuantity(99999);
            inv[EntitiesEnum.Cash].AddQuantity(360);

            // The 1985 caps hold: 20 oxen, 3 per spare part, 255 clothes, 65,535 bullets, 2,000 lb food.
            Assert.Equal(20, inv[EntitiesEnum.Animal].Quantity);
            Assert.Equal(255, inv[EntitiesEnum.Clothes].Quantity);
            Assert.Equal(65535, inv[EntitiesEnum.Ammo].Quantity);
            Assert.Equal(2000, inv[EntitiesEnum.Food].Quantity);

            // base = 2500 people + 50 wagon + 80 oxen + 18 parts + 510 clothes + 1310 bullets + 80 food + 72 cash = 4620.
            Assert.Equal(4620, ScoreCalculator.ComputeBase(vehicle));
            Assert.Equal(13860, ScoreCalculator.Compute(vehicle));
        }
    }
}
