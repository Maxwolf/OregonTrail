using System.Linq;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Module.Trail;
using OregonTrailDotNet.Window.Travel.RiverCrossing;
using Xunit;

namespace OregonTrailDotNet.Tests.Window
{
    /// <summary>
    ///     Covers the rivers: the depths they run at, and the thresholds that decide whether crossing one is a paddle, a
    ///     soaking, or a drowning. Depth is the original's own river table plus however wet the country has become, which is
    ///     the whole reason the season matters more than luck at a ford.
    /// </summary>
    public class RiverCrossingTests : SimulationTestBase
    {
        private static RiverCrossing River(string name)
        {
            return TrailRegistry.OregonTrail.Locations
                .Concat(TrailRegistry.OregonTrail.Locations.OfType<ForkInRoad>()
                    .SelectMany(fork => fork.SkipChoices.Where(choice => choice != null)))
                .OfType<RiverCrossing>()
                .First(river => river.Name == name);
        }

        [Fact]
        public void RiverBaseDepths_AreTheOriginalsRiverTable()
        {
            // Recovered from VAR.BIN (legacy/source/VAR.BIN.txt). The Kansas and the Big Blue are a foot deep in their own
            // beds and only dangerous when swollen; the Green runs twenty feet deep, which is why the Fort Bridger road is
            // worth eighty-six extra miles.
            Assert.Equal(1.0, River("Kansas River Crossing").BaseDepth);
            Assert.Equal(1.0, River("Big Blue River Crossing").BaseDepth);
            Assert.Equal(20.0, River("Green River Crossing").BaseDepth);
            Assert.Equal(6.0, River("Snake River Crossing").BaseDepth);
        }

        [Fact]
        public void RiverBottoms_DecideWhatGoesWrongOnAShallowFord()
        {
            Assert.Equal(RiverBottomEnum.Firm, River("Kansas River Crossing").Bottom);
            Assert.Equal(RiverBottomEnum.Muddy, River("Big Blue River Crossing").Bottom);
            Assert.Equal(RiverBottomEnum.Rough, River("Snake River Crossing").Bottom);
        }

        [Fact]
        public void OnlyTheKansasAndGreenHaveAFerry_SoOnlyOneRequiredCrossingCanBeBought()
        {
            Assert.Equal(RiverOptionEnum.FerryOperator, River("Kansas River Crossing").RiverCrossOption);
            Assert.Equal(RiverOptionEnum.FerryOperator, River("Green River Crossing").RiverCrossOption);
            Assert.Equal(RiverOptionEnum.IndianGuide, River("Snake River Crossing").RiverCrossOption);
            Assert.Equal(RiverOptionEnum.FloatAndFord, River("Big Blue River Crossing").RiverCrossOption);

            // The Green is avoidable by way of Fort Bridger, so of the three crossings a party must make, only the Kansas
            // can be ferried. That is what makes the first river the one you can buy your way over and the rest a gamble.
            var required = TrailRegistry.OregonTrail.Locations.OfType<RiverCrossing>().ToList();
            Assert.Single(required, river => river.RiverCrossOption == RiverOptionEnum.FerryOperator);
        }

        [Fact]
        public void FordThresholds_RunSafeThenSoakingThenDangerous()
        {
            // Two and a half feet is the line between wading and paying for it, and three between paying and drowning.
            Assert.Equal(2.5, RiverGenerator.SafeFordDepth);
            Assert.Equal(3.0, RiverGenerator.DangerousFordDepth);

            // A wagon needs water under it to float at all.
            Assert.True(RiverGenerator.FloatableDepth < RiverGenerator.SafeFordDepth);
        }
    }
}
