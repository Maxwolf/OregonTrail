using OregonTrailDotNet.Entity.Person;
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
            Assert.Equal(string.Empty, tombstone.Epitaph);
        }

        [Fact]
        public void ToString_WithoutEpitaph_OnlyNamesTheDeceased()
        {
            var tombstone = new TombstoneEntity();

            Assert.Contains("Here lies Alice", tombstone.ToString());
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
        public void Module_Reset_ClearsAllTombstones()
        {
            var tombstone = new TombstoneEntity();
            Game.Tombstone.Add(tombstone);
            Game.Tombstone.Reset();

            Assert.False(Game.Tombstone.ContainsTombstone(tombstone.MileMarker));
        }
    }
}
