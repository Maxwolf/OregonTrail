using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Module.Tombstone;
using Xunit;
using PersonEntity = OregonTrailDotNet.Entity.Person.Person;
using TombstoneEntity = OregonTrailDotNet.Module.Tombstone.Tombstone;

namespace OregonTrailDotNet.Tests.Module
{
    /// <summary>
    ///     Covers grave markers left on the trail for future players to find. Creating a tombstone
    ///     reads the party leader and odometer from the running simulation.
    /// </summary>
    public class TombstoneTests : SimulationTestBase
    {
        public TombstoneTests()
        {
            // Tombstones capture the current party leader by name.
            Game.Vehicle.AddPerson(new PersonEntity(Profession.Banker, "Alice", true));
            Game.Vehicle.AddPerson(new PersonEntity(Profession.Banker, "Bob", false));
        }

        [Fact]
        public void Ctor_CapturesLeaderNameAndMileMarker()
        {
            var tombstone = new TombstoneEntity();

            Assert.Equal("Alice", tombstone.PlayerName);
            Assert.Equal(Game.Vehicle.Odometer, tombstone.MileMarker);

            // A fresh grave starts with a random silly epitaph from the pool rather than a blank one.
            Assert.False(string.IsNullOrEmpty(tombstone.Epitaph));
            Assert.Contains(tombstone.Epitaph, EpitaphCatalog.All);
        }

        [Fact]
        public void ToString_WithoutEpitaph_OnlyNamesTheDeceased()
        {
            // A grave with an explicitly empty epitaph (e.g. one loaded blank from the database) only names the deceased.
            var tombstone = new TombstoneEntity(0, 100, "Alice", "", "Independence", "Kansas River Crossing", 20);

            Assert.Contains("Here lies Alice", tombstone.ToString());
            Assert.Equal("Here lies Alice", tombstone.ToString().Trim());
        }

        [Fact]
        public void ToString_WithEpitaph_IncludesFinalWords()
        {
            var tombstone = new TombstoneEntity {Epitaph = "Died of dysentery."};

            Assert.Contains("Here lies Alice", tombstone.ToString());
            Assert.Contains("Died of dysentery.", tombstone.ToString());
        }

        [Fact]
        public void Clone_CreatesIndependentCopy()
        {
            var tombstone = new TombstoneEntity {Epitaph = "Original"};
            var clone = (TombstoneEntity) tombstone.Clone();
            clone.Epitaph = "Changed";

            Assert.Equal("Original", tombstone.Epitaph);
            Assert.Equal(tombstone.PlayerName, clone.PlayerName);
            Assert.Equal(tombstone.MileMarker, clone.MileMarker);
        }

        [Fact]
        public void Module_AddAndFindTombstoneByMileMarker()
        {
            var tombstone = new TombstoneEntity();
            Game.Tombstone.Add(tombstone);

            Assert.True(Game.Tombstone.ContainsTombstone(tombstone.MileMarker));

            Game.Tombstone.FindTombstone(tombstone.MileMarker, out var found);
            Assert.NotNull(found);
            Assert.Equal("Alice", found.PlayerName);
        }

        [Fact]
        public void Module_UnknownMileMarker_HasNoTombstone()
        {
            Assert.False(Game.Tombstone.ContainsTombstone(12345));
        }

        [Fact]
        public void Module_SecondDeathInSameHalf_OverwritesTheEarlierGrave()
        {
            // Two deaths in the first half (trail_half 0): the trail only keeps one grave per half, so the later one wins.
            Game.Tombstone.Add(new TombstoneEntity(0, 100, "Early", "", "Independence", "Kansas River Crossing", 20));
            Game.Tombstone.Add(new TombstoneEntity(0, 300, "Later", "", "Fort Kearney", "Chimney Rock", 10));

            Assert.False(Game.Tombstone.ContainsTombstone(100)); // the first grave was overwritten
            Assert.True(Game.Tombstone.ContainsTombstone(300));  // only the most recent death in the half remains

            Game.Tombstone.FindTombstone(300, out var grave);
            Assert.Equal("Later", grave.PlayerName);
        }

        [Fact]
        public void Module_DeathsInDifferentHalves_KeepBothGraves()
        {
            // One grave per half means a first-half and a second-half death coexist.
            Game.Tombstone.Add(new TombstoneEntity(0, 300, "First Half", "", "Fort Kearney", "Chimney Rock", 10));
            Game.Tombstone.Add(new TombstoneEntity(1, 1500, "Second Half", "", "Fort Hall", "Fort Boise", 5));

            Assert.True(Game.Tombstone.ContainsTombstone(300));
            Assert.True(Game.Tombstone.ContainsTombstone(1500));
        }

        [Fact]
        public void Module_Reset_ClearsAllTombstones()
        {
            var tombstone = new TombstoneEntity();
            Game.Tombstone.Add(tombstone);
            Game.Tombstone.Reset();

            Assert.False(Game.Tombstone.ContainsTombstone(tombstone.MileMarker));
        }

        [Fact]
        public void FindTombstoneBetween_DetectsAGraveCrossedThisTurn_AndRemembersIt()
        {
            // A grave sitting at mile 250 that the party sweeps past when the odometer jumps from 200 to 300.
            Game.Tombstone.Add(new TombstoneEntity(0, 250, "Wanderer", "welp", "Fort Kearney", "Chimney Rock", 10));

            Assert.True(Game.Tombstone.FindTombstoneBetween(200, 300, out var grave));
            Assert.Equal("Wanderer", grave.PlayerName);

            // The crossed grave is remembered so the graveyard view can show it even though we did not land on mile 250.
            Assert.NotNull(Game.Tombstone.Encountered);
            Assert.Equal(250, Game.Tombstone.Encountered.MileMarker);
        }

        [Fact]
        public void FindTombstoneBetween_IgnoresGravesOutsideTheStretchJustTraveled()
        {
            Game.Tombstone.Add(new TombstoneEntity(0, 250, "Wanderer", "welp", "Fort Kearney", "Chimney Rock", 10));

            // The party has not reached mile 250 yet (still short of it) so there is nothing to come across.
            Assert.False(Game.Tombstone.FindTombstoneBetween(100, 200, out var beforeIt));
            Assert.Null(beforeIt);

            // And a grave exactly at the start of the stretch was already passed on the previous turn — not crossed again.
            Assert.False(Game.Tombstone.FindTombstoneBetween(250, 350, out var alreadyPassed));
            Assert.Null(alreadyPassed);
        }
    }
}
