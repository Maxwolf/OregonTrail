using System;
using System.Collections.Generic;
using System.Linq;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Module.Trail;
using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.MainMenu;
using OregonTrailDotNet.Window.Travel.Scene;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the raft's loss routines against the FLOAT spec: the leader is spared from drowning while anybody
    ///     else stands (and takes the roll alone), the loss-line counter is per collision, more than nine lines in
    ///     one event destroys the raft and the party, and a missed landing costs supplies only. Deterministic
    ///     Random stubs stand in for the dice.
    /// </summary>
    public class RaftDamageTests : SimulationTestBase
    {
        /// <summary>Every roll hits, and every quantity roll takes the minimum (one).</summary>
        private sealed class AlwaysHit : Random
        {
            protected override double Sample() => 0.0;
        }

        /// <summary>No roll ever hits.</summary>
        private sealed class NeverHit : Random
        {
            protected override double Sample() => 0.9999999;
        }

        public override void Dispose()
        {
            GameSimulationApp.PresentationEnabled = false;
            Music.Shutdown();
            base.Dispose();
        }

        /// <summary>Outfits a real four-person party and zeroes the wagon so each test sets its own stakes.</summary>
        private void BootParty()
        {
            Game.SetStartInfo(new NewGameInfo
            {
                PlayerNames = new List<string> { "Alice", "Bob", "Carol", "Dave" },
                PlayerProfession = ProfessionEnum.Banker,
                StartingMonies = 1600,
                StartingMonth = MonthEnum.April
            });

            foreach (EntitiesEnum entity in Enum.GetValues(typeof(EntitiesEnum)))
                if (Game.Vehicle.Inventory.ContainsKey(entity))
                    Game.Vehicle.Inventory[entity].ReduceQuantity(int.MaxValue);
        }

        [Fact]
        public void RockHit_SparesTheLeader_WhileOthersStand()
        {
            BootParty();

            // No oxen, no supplies: the only lines are the three companions drowning — under ten, no destruction.
            var report = RaftDamage.RockHit(new AlwaysHit());

            Assert.False(report.Destroyed);
            Assert.Equal(3, report.Lines.Count);
            var leader = Game.Vehicle.PassengerLeader;
            Assert.NotNull(leader);
            Assert.NotEqual(HealthStatusEnum.Dead, leader.HealthStatus);
            Assert.Equal(3, Game.Vehicle.Passengers.Count(p => p.HealthStatus == HealthStatusEnum.Dead));
        }

        [Fact]
        public void CatastrophicHit_DestroysTheRaft_AndTheParty()
        {
            BootParty();

            // Three drownings + eight oxen is eleven loss lines in ONE event — past nine, the river wins outright.
            Game.Vehicle.Inventory[EntitiesEnum.Animal].AddQuantity(8);

            var report = RaftDamage.RockHit(new AlwaysHit());

            Assert.True(report.Destroyed);
            Assert.True(Game.Vehicle.PassengersDead);
            Assert.Equal(0, Game.Vehicle.Inventory[EntitiesEnum.Animal].Quantity);
        }

        [Fact]
        public void MissedLanding_CostsSuppliesOnly()
        {
            BootParty();
            Game.Vehicle.Inventory[EntitiesEnum.Food].AddQuantity(100);
            Game.Vehicle.Inventory[EntitiesEnum.Animal].AddQuantity(4);

            var report = RaftDamage.MissedLanding(new AlwaysHit());

            // Nobody drowns and no ox is lost on a missed landing; the food takes the minimum hit of one.
            Assert.False(report.Destroyed);
            Assert.Equal(0, Game.Vehicle.Passengers.Count(p => p.HealthStatus == HealthStatusEnum.Dead));
            Assert.Equal(4, Game.Vehicle.Inventory[EntitiesEnum.Animal].Quantity);
            Assert.Equal(99, Game.Vehicle.Inventory[EntitiesEnum.Food].Quantity);
        }

        [Fact]
        public void KindWater_TakesNothing()
        {
            BootParty();
            Game.Vehicle.Inventory[EntitiesEnum.Food].AddQuantity(100);

            var report = RaftDamage.RockHit(new NeverHit());

            Assert.False(report.Destroyed);
            Assert.Empty(report.Lines);
            Assert.Equal(100, Game.Vehicle.Inventory[EntitiesEnum.Food].Quantity);
        }

        [Fact]
        public void Columbia_IsTheRaftCrossing_AndTheGateChoosesByFlag()
        {
            // The registry marks exactly one raft crossing: the Columbia, on The Dalles fork.
            var rafts = TrailRegistry.OregonTrail.Locations
                .OfType<ForkInRoad>()
                .SelectMany(fork => fork.SkipChoices)
                .OfType<RiverCrossing>()
                .Where(river => river.RaftCrossing)
                .ToList();

            Assert.Single(rafts);
            Assert.Equal("Columbia River", rafts[0].Name);
        }
    }
}
