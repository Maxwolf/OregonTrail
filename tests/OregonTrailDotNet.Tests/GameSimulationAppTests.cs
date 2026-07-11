using System;
using System.Collections.Generic;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Window.MainMenu;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Covers the singleton lifecycle and the top level turn/party plumbing of the simulation.
    /// </summary>
    public class GameSimulationAppTests : SimulationTestBase
    {
        [Fact]
        public void Create_WhenInstanceAlreadyExists_Throws()
        {
            Assert.Throws<InvalidOperationException>(GameSimulationApp.Create);
        }

        [Fact]
        public void FirstTick_BootsAllModules()
        {
            Assert.NotNull(Game.Time);
            Assert.NotNull(Game.Trail);
            Assert.NotNull(Game.EventDirector);
            Assert.NotNull(Game.Vehicle);
            Assert.NotNull(Game.Scoring);
            Assert.NotNull(Game.Tombstone);
        }

        [Fact]
        public void FirstTick_FocusesMainMenuWindow()
        {
            Assert.IsType<MainMenu>(Game.WindowManager.FocusedWindow);
        }

        [Fact]
        public void TakeTurn_IncrementsTotalTurns()
        {
            Game.TakeTurn(false);
            Game.TakeTurn(false);

            Assert.Equal(2, Game.TotalTurns);
        }

        [Fact]
        public void TakeTurn_SkippingDay_DoesNotCountAsTurn()
        {
            Game.TakeTurn(true);

            Assert.Equal(0, Game.TotalTurns);
        }

        [Fact]
        public void SetStartInfo_BuildsPartyWithLeaderFirst()
        {
            Game.SetStartInfo(new NewGameInfo
            {
                PlayerNames = new List<string> {"Alice", "Bob", "Carol", "Dave", "Eve"},
                PlayerProfession = Profession.Farmer,
                StartingMonies = 1000,
                StartingMonth = Month.April
            });

            Assert.Equal(GameSimulationApp.MAXPLAYERS, Game.Vehicle.Passengers.Count);
            Assert.Equal("Alice", Game.Vehicle.PassengerLeader.Name);
            Assert.True(Game.Vehicle.Passengers[0].Leader);
            Assert.False(Game.Vehicle.Passengers[1].Leader);
            Assert.All(Game.Vehicle.Passengers, person => Assert.Equal(Profession.Farmer, person.Profession));
            Assert.Equal(1000f, Game.Vehicle.Balance);
            Assert.Equal(Month.April, Game.Time.CurrentMonth);
        }

        [Fact]
        public void OnPreRender_ReportsTurnCounter()
        {
            Assert.Contains("Turns: 0000", Game.OnPreRender());

            Game.TakeTurn(false);
            Assert.Contains("Turns: 0001", Game.OnPreRender());
        }

        [Fact]
        public void Destroy_TearsDownSingleton()
        {
            Game.Destroy();

            Assert.Null(GameSimulationApp.Instance);
        }
    }
}
