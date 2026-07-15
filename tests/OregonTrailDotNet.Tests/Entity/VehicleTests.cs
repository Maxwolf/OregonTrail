using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Entity.Vehicle;
using Xunit;
using PersonEntity = OregonTrailDotNet.Entity.Person.Person;
using VehicleEntity = OregonTrailDotNet.Entity.Vehicle.Vehicle;

namespace OregonTrailDotNet.Tests.Entity
{
    /// <summary>
    ///     Covers the wagon: money, inventory purchases, passengers, movement status, and part
    ///     breakage. Runs against a booted simulation because part breakage rolls the shared dice.
    /// </summary>
    public class VehicleTests : SimulationTestBase
    {
        [Fact]
        public void Ctor_StartsStoppedWithSteadyPaceAndFillingRations()
        {
            var vehicle = new VehicleEntity();

            Assert.Equal(VehicleStatusEnum.Stopped, vehicle.Status);
            Assert.Equal(TravelPaceEnum.Steady, vehicle.Pace);
            Assert.Equal(RationLevelEnum.Filling, vehicle.Ration);
            Assert.Equal(1, vehicle.Mileage);
            Assert.Equal(0, vehicle.Odometer);
            Assert.Empty(vehicle.Passengers);
        }

        [Fact]
        public void ResetVehicle_SetsStartingBalanceAndClearsState()
        {
            var vehicle = new VehicleEntity();
            vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, "Bob", true));
            vehicle.ResetVehicle(500);

            Assert.Equal(500f, vehicle.Balance);
            Assert.Empty(vehicle.Passengers);
            Assert.Equal(0, vehicle.Odometer);
            Assert.Equal(VehicleStatusEnum.Stopped, vehicle.Status);
        }

        [Fact]
        public void Purchase_DeductsBalanceAndAddsQuantityToInventory()
        {
            var vehicle = new VehicleEntity();
            vehicle.ResetVehicle(100);

            // 50 pounds of food at $0.10 per pound (the starting price, before any forts are departed) is a $5 transaction.
            vehicle.Purchase(new SimItem(Resources.Food, 50));

            Assert.Equal(95f, vehicle.Balance);
            Assert.Equal(50, vehicle.Inventory[EntitiesEnum.Food].Quantity);
        }

        [Fact]
        public void Purchase_WithInsufficientFunds_DoesNothing()
        {
            var vehicle = new VehicleEntity();
            vehicle.ResetVehicle(5);

            vehicle.Purchase(new SimItem(Resources.Food, 100));

            Assert.Equal(5f, vehicle.Balance);
            Assert.Equal(0, vehicle.Inventory[EntitiesEnum.Food].Quantity);
        }

        [Fact]
        public void Purchase_FractionalDollarTotal_RoundsBalanceToNearestInsteadOfFlooring()
        {
            var vehicle = new VehicleEntity();
            vehicle.ResetVehicle(100);

            // 15 pounds of food at $0.10/lb is a $1.50 charge, leaving $98.50. That balance is rounded to the nearest
            // whole dollar ($99); the old code truncated toward zero to $98, silently overcharging the player.
            vehicle.Purchase(new SimItem(Resources.Food, 15));

            Assert.Equal(99f, vehicle.Balance);
            Assert.Equal(15, vehicle.Inventory[EntitiesEnum.Food].Quantity);
        }

        [Fact]
        public void AddPerson_TracksPassengersAndLeader()
        {
            var vehicle = new VehicleEntity();
            var leader = new PersonEntity(ProfessionEnum.Banker, "Alice", true);
            vehicle.AddPerson(leader);
            vehicle.AddPerson(new PersonEntity(ProfessionEnum.Banker, "Bob", false));

            Assert.Equal(2, vehicle.Passengers.Count);
            Assert.Same(leader, vehicle.PassengerLeader);
            Assert.Equal(2, vehicle.PassengerLivingCount);
        }

        [Fact]
        public void PassengersDead_OnlyWhenEveryPassengerIsDead()
        {
            var vehicle = new VehicleEntity();

            // Nobody aboard means nobody can be dead.
            Assert.False(vehicle.PassengersDead);

            var alice = new PersonEntity(ProfessionEnum.Banker, "Alice", true);
            var bob = new PersonEntity(ProfessionEnum.Banker, "Bob", false);
            vehicle.AddPerson(alice);
            vehicle.AddPerson(bob);

            alice.Kill();
            Assert.False(vehicle.PassengersDead);
            Assert.Equal(1, vehicle.PassengerLivingCount);

            bob.Kill();
            Assert.True(vehicle.PassengersDead);
            Assert.Equal(0, vehicle.PassengerLivingCount);
        }

        [Fact]
        public void PassengerHealthStatus_NoPassengers_ReportsDead()
        {
            Assert.Equal(HealthStatusEnum.Dead, new VehicleEntity().PassengerHealthStatus);
        }

        [Fact]
        public void PassengerHealthStatus_AveragesOnlyLivingPassengers()
        {
            var vehicle = new VehicleEntity();
            var alice = new PersonEntity(ProfessionEnum.Banker, "Alice", true);
            var bob = new PersonEntity(ProfessionEnum.Banker, "Bob", false);
            vehicle.AddPerson(alice);
            vehicle.AddPerson(bob);

            bob.Kill();

            Assert.Equal(HealthStatusEnum.Good, vehicle.PassengerHealthStatus);
        }

        [Fact]
        public void ChangeRationsAndPace_UpdateSettings()
        {
            var vehicle = new VehicleEntity();
            vehicle.ChangeRations(RationLevelEnum.BareBones);
            vehicle.ChangePace(TravelPaceEnum.Grueling);

            Assert.Equal(RationLevelEnum.BareBones, vehicle.Ration);
            Assert.Equal(TravelPaceEnum.Grueling, vehicle.Pace);
        }

        [Fact]
        public void CheckStatus_WithoutOxen_DisablesVehicle()
        {
            var vehicle = new VehicleEntity();
            vehicle.CheckStatus();

            Assert.Equal(VehicleStatusEnum.Disabled, vehicle.Status);
        }

        [Fact]
        public void CheckStatus_WithOxen_AllowsMovement()
        {
            var vehicle = new VehicleEntity();
            vehicle.Inventory[EntitiesEnum.Animal].AddQuantity(2);
            vehicle.CheckStatus();

            Assert.Equal(VehicleStatusEnum.Moving, vehicle.Status);
        }

        [Fact]
        public void CheckStatus_DisabledVehicleStaysDisabled()
        {
            var vehicle = new VehicleEntity();
            vehicle.CheckStatus();
            Assert.Equal(VehicleStatusEnum.Disabled, vehicle.Status);

            // Getting oxen back does not automatically re-enable the wagon.
            vehicle.Inventory[EntitiesEnum.Animal].AddQuantity(2);
            vehicle.CheckStatus();

            Assert.Equal(VehicleStatusEnum.Disabled, vehicle.Status);
        }

        [Fact]
        public void ReduceMileage_OnlyAppliesWhileMoving()
        {
            var vehicle = new VehicleEntity();
            vehicle.ReduceMileage(5);

            Assert.Equal(1, vehicle.Mileage);
        }

        [Fact]
        public void ReduceMileage_FloorsAtZero()
        {
            var vehicle = new VehicleEntity {Status = VehicleStatusEnum.Moving};
            vehicle.ReduceMileage(5);

            Assert.Equal(0, vehicle.Mileage);
        }

        [Fact]
        public void ContainsItem_RequiresMinimumQuantityInInventory()
        {
            var vehicle = new VehicleEntity();
            var wantedWheel = new SimItem(Parts.Wheel, 1);

            Assert.False(vehicle.ContainsItem(wantedWheel));

            vehicle.Inventory[EntitiesEnum.Wheel].AddQuantity(1);
            Assert.True(vehicle.ContainsItem(wantedWheel));
        }

        [Fact]
        public void ClosestTo_PicksNearestValueInCollection()
        {
            var healthValues = new[] {0, 200, 300, 400, 500};

            Assert.Equal(300, VehicleEntity.ClosestTo(healthValues, 260));
            Assert.Equal(500, VehicleEntity.ClosestTo(healthValues, 9999));
            Assert.Equal(0, VehicleEntity.ClosestTo(healthValues, 50));
        }

        [Fact]
        public void BreakRandomPart_SetsBrokenPartAndNeverOverwritesIt()
        {
            var vehicle = new VehicleEntity();
            vehicle.BreakRandomPart();

            var broken = vehicle.BrokenPart;
            Assert.NotNull(broken);

            vehicle.BreakRandomPart();
            Assert.Same(broken, vehicle.BrokenPart);
        }

        [Fact]
        public void TryUseSparePart_WithoutSpare_LeavesVehicleBroken()
        {
            var vehicle = new VehicleEntity();
            vehicle.BreakRandomPart();

            Assert.False(vehicle.TryUseSparePart());
        }

        [Fact]
        public void TryUseSparePart_ConsumesSpareAndStopsVehicle()
        {
            var vehicle = new VehicleEntity {Status = VehicleStatusEnum.Moving};
            vehicle.BreakRandomPart();
            vehicle.Inventory[vehicle.BrokenPart.Category].AddQuantity(1);

            Assert.True(vehicle.TryUseSparePart());
            Assert.Equal(0, vehicle.Inventory[vehicle.BrokenPart.Category].Quantity);
            Assert.Equal(VehicleStatusEnum.Stopped, vehicle.Status);
        }

        [Fact]
        public void CreateRandomItems_NeverExceedsInventoryMaximums()
        {
            var vehicle = new VehicleEntity();
            var created = vehicle.CreateRandomItems();

            foreach (var item in vehicle.Inventory)
                Assert.True(item.Value.Quantity <= item.Value.MaxQuantity);

            foreach (var amount in created.Values)
                Assert.True(amount > 0);
        }

        [Fact]
        public void CreateRandomItems_NeverLootsCash()
        {
            // Cash's MaxQuantity is int.MaxValue, so an abandoned-wagon find that looted cash would roll
            // amountToMake = int.MaxValue / 4 and hand the player up to ~536 million dollars, inflating the
            // end-of-game score into the hundreds of millions. Looted cash must always stay untouched.
            var vehicle = new VehicleEntity();
            vehicle.ResetVehicle(400);

            // Run many finds so the per-item coin flip has ample opportunity to try looting cash.
            for (var i = 0; i < 500; i++)
            {
                var created = vehicle.CreateRandomItems();
                Assert.False(created.ContainsKey(EntitiesEnum.Cash));
                Assert.Equal(400, vehicle.Inventory[EntitiesEnum.Cash].Quantity);
            }
        }

        [Fact]
        public void DestroyRandomItems_NeverDrivesQuantitiesNegative()
        {
            var vehicle = new VehicleEntity();
            vehicle.Inventory[EntitiesEnum.Food].AddQuantity(100);
            vehicle.Inventory[EntitiesEnum.Clothes].AddQuantity(10);

            vehicle.DestroyRandomItems();

            foreach (var item in vehicle.Inventory)
                Assert.True(item.Value.Quantity >= 0);
        }
    }
}
