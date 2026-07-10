using System.Collections.Generic;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Window.MainMenu;
using OregonTrailDotNet.Window.MainMenu.Profession;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Drives the simulation the way a real play session does: through the menu system via typed
    ///     input, and down the trail day by day with random events firing along the way.
    /// </summary>
    public class IntegrationTests : SimulationTestBase
    {
        [Fact]
        public void MainMenu_StartsWithCommandMenuAndNoForm()
        {
            Assert.IsType<MainMenu>(Game.WindowManager.FocusedWindow);
            Assert.Null(Game.WindowManager.FocusedWindow.CurrentForm);
        }

        [Fact]
        public void MainMenu_TravelCommand_OpensProfessionSelector()
        {
            SendCommand("1");

            Assert.IsType<ProfessionSelector>(Game.WindowManager.FocusedWindow.CurrentForm);
        }

        [Fact]
        public void MainMenu_TopTenCommand_ShowsHighScores()
        {
            SendCommand("3");

            Assert.IsType<CurrentTopTen>(Game.WindowManager.FocusedWindow.CurrentForm);
        }

        [Fact]
        public void FullJourney_PartyTravelsFromIndependenceToKansasRiver()
        {
            // Outfit the party the same way the new game flow would.
            Game.SetStartInfo(new NewGameInfo
            {
                PlayerNames = new List<string> {"Alice", "Bob", "Carol", "Dave"},
                PlayerProfession = Profession.Banker,
                StartingMonies = 1600,
                StartingMonth = Month.April
            });

            var vehicle = Game.Vehicle;
            vehicle.Purchase(new SimItem(Parts.Oxen, 8));
            vehicle.Purchase(new SimItem(Resources.Food, 1000));
            vehicle.Purchase(new SimItem(Resources.Clothing, 8));
            vehicle.Purchase(new SimItem(Resources.Bullets, 40));
            vehicle.Purchase(new SimItem(Parts.Wheel, 1));
            vehicle.Purchase(new SimItem(Parts.Axle, 1));
            vehicle.Purchase(new SimItem(Parts.Tongue, 1));

            // Arrive at Independence, which primes the distance to the next location.
            Game.Trail.ArriveAtNextLocation();
            Assert.Equal("Independence", Game.Trail.CurrentLocation.Name);
            Assert.True(Game.Trail.DistanceToNextLocation > 0);

            // Leave town the same way the continue on trail command does; while a location is
            // still flagged as arrived the trail module refuses to move the vehicle onward.
            Game.Trail.CurrentLocation.Status = LocationStatus.Departed;

            // Head out. Random events may stop or damage the wagon along the way, so keep pushing
            // until the party arrives at the second location on the trail.
            for (var day = 0; (day < 150) && (Game.Trail.LocationIndex < 1); day++)
            {
                vehicle.Status = VehicleStatus.Moving;
                Game.TakeTurn(false);
            }

            Assert.Equal(1, Game.Trail.LocationIndex);
            Assert.Equal("Kansas River Crossing", Game.Trail.CurrentLocation.Name);
            Assert.True(vehicle.Odometer > 0);
            Assert.True(Game.TotalTurns > 0);
            Assert.True(Game.Time.CurrentMonth != Month.April || Game.Time.Date.Day > 1);
        }

        [Fact]
        public void PartyWithNoFood_DiesAndRecordsACauseOfDeath()
        {
            // Outfit a party with oxen and clothing but deliberately no food and no ammunition, so they cannot eat and
            // cannot hunt - they are doomed to die on the trail.
            Game.SetStartInfo(new NewGameInfo
            {
                PlayerNames = new List<string> {"Alice", "Bob"},
                PlayerProfession = Profession.Banker,
                StartingMonies = 1600,
                StartingMonth = Month.April
            });

            var vehicle = Game.Vehicle;
            vehicle.Purchase(new SimItem(Parts.Oxen, 8));
            vehicle.Purchase(new SimItem(Resources.Clothing, 8));

            Game.Trail.ArriveAtNextLocation();
            Game.Trail.CurrentLocation.Status = LocationStatus.Departed;

            // Push down the trail until the whole party has perished.
            for (var day = 0; (day < 250) && !vehicle.PassengersDead; day++)
            {
                vehicle.Status = VehicleStatus.Moving;
                Game.TakeTurn(false);
            }

            // The party should be dead, and at least one member should have a recorded cause of death (they take starvation
            // damage every single day, so the killing blow records a cause) rather than being left as Unknown - proving the
            // death screen has something to report.
            Assert.True(vehicle.PassengersDead);
            Assert.Contains(vehicle.Passengers, person => person.Cause != CauseOfDeath.Unknown);
        }
    }
}
