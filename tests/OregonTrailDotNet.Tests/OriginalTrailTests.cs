using System.Collections.Generic;
using System.Linq;
using OregonTrailDotNet.Assets;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Module.Trail;
using OregonTrailDotNet.Presentation;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the OriginalTrail identity map against both of its consumers: every location the real trail can ever
    ///     visit (including fork branches) must resolve to an original landmark, and every asset a resolved stop
    ///     references must actually be embedded. Scoped to TrailRegistry.OregonTrail deliberately — the debug
    ///     WinTrail's locations are exempt, which is also why lookups fail soft instead of throwing.
    ///     Derives from <see cref="SimulationTestBase" /> because constructing a Location touches the game singleton.
    /// </summary>
    public class OriginalTrailTests : SimulationTestBase
    {
        /// <summary>Every location on the real trail, with fork branches flattened in.</summary>
        private static IEnumerable<Location> AllOregonTrailLocations()
        {
            foreach (var location in TrailRegistry.OregonTrail.Locations)
            {
                yield return location;

                if (location is not ForkInRoad fork)
                    continue;

                // A null skip choice means "stay on the main trail" and is not a location.
                foreach (var branch in fork.SkipChoices)
                    if (branch != null)
                        yield return branch;
            }
        }

        [Fact]
        public void EveryOregonTrailLocation_ResolvesToAnOriginalStop()
        {
            foreach (var location in AllOregonTrailLocations())
                Assert.True(OriginalTrail.ForLocation(location.Name) != null,
                    $"TrailRegistry location '{location.Name}' has no OriginalTrail row.");
        }

        [Fact]
        public void AllEighteenOriginalLandmarks_ArePresent()
        {
            var indices = OriginalTrail.All.Select(stop => stop.Index).Distinct().OrderBy(i => i).ToArray();
            Assert.Equal(Enumerable.Range(0, 18).ToArray(), indices);
        }

        [Fact]
        public void UnknownLocation_FailsSoft()
        {
            Assert.Null(OriginalTrail.ForLocation("Start Of Test"));
            Assert.Null(OriginalTrail.ForLocation(null));
        }

        [Fact]
        public void EveryReferencedAsset_IsEmbedded()
        {
            foreach (var stop in OriginalTrail.All)
            {
                if (stop.CardArt >= 0)
                    Assert.True(AssetStore.Has($"art/landmarks/p{stop.CardArt}.png"),
                        $"Missing card art for {stop.GameName} (p{stop.CardArt}).");

                if (stop.MusicSlug != null)
                    Assert.True(AssetStore.Has($"music/landmarks/{stop.MusicSlug}.json"),
                        $"Missing tune for {stop.GameName} ({stop.MusicSlug}).");

                if (stop.ScenerySpriteId > 0)
                    Assert.True(AssetStore.Has($"art/sprites/scenery/{stop.ScenerySpriteId:00}.png"),
                        $"Missing scenery sprite for the leg toward {stop.GameName} (#{stop.ScenerySpriteId}).");
            }

            // The stops' shared fixtures: the map every MapScene draws, the Apple II tombstone, and Taps.
            Assert.True(AssetStore.Has("art/map.png"));
            Assert.True(AssetStore.Has("art/tombstone.png"));
            Assert.True(AssetStore.Has("music/tombstone.json"));
        }

        [Fact]
        public void MapCoordinates_SitInsideTheDosMap()
        {
            foreach (var stop in OriginalTrail.All)
            {
                Assert.InRange(stop.MapX, 0, MapGame.MapWidth - 1);
                Assert.InRange(stop.MapY, 0, MapGame.MapHeight - 1);
            }
        }
    }
}
