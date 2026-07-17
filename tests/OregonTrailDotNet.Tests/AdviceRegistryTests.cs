using System.Collections.Generic;
using System.Linq;
using OregonTrailDotNet.Window.Travel.TalkToPeople;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the "Talk to people" gossip against the 1985 Apple II original (decoded from the disks' OREGON1/2.SEQ,
    ///     preserved in legacy/source/GOSSIP.txt): 51 quotes in total, with the original speaker names — several of which
    ///     had drifted in this port (an invented "Sam Hendricks", "Jimmy" for Jim, dropped surnames and epithets).
    /// </summary>
    public class AdviceRegistryTests
    {
        private static IEnumerable<Advice> All => AdviceRegistry.Tutorial
            .Concat(AdviceRegistry.River)
            .Concat(AdviceRegistry.Landmark)
            .Concat(AdviceRegistry.Settlement)
            .Concat(AdviceRegistry.Mountain)
            .Concat(AdviceRegistry.Ending);

        [Fact]
        public void Registry_HasAllFiftyOneOriginalQuotes()
        {
            // 17 landmarks x 3 quotes on the 1985 disks.
            Assert.Equal(51, All.Count());
        }

        [Theory]
        [InlineData("A trader named Jim")]
        [InlineData("A traveler, Miles Hendricks,")]
        [InlineData("A stranger")]
        [InlineData("A lady, Marnie Stewart,")]
        [InlineData("Big Louie, a trail driver,")]
        [InlineData("A Sioux brave")]
        [InlineData("Jacob Hofsteader")]
        [InlineData("Aunt Rebecca Sims")]
        public void Registry_UsesTheOriginalSpeakerNames(string name)
        {
            Assert.Contains(All, advice => advice.Name == name);
        }

        [Fact]
        public void Registry_HasNoInventedSpeakerNames()
        {
            // Names that existed only in this port, never on the 1985 disks.
            Assert.DoesNotContain(All, advice => advice.Name == "Sam Hendricks");
            Assert.DoesNotContain(All, advice => advice.Name == "A trader named Jimmy");
            Assert.DoesNotContain(All, advice => advice.Name == "A Sioux hunter");
            Assert.DoesNotContain(All, advice => advice.Name == "Jacob Hofstead");
        }
    }
}
