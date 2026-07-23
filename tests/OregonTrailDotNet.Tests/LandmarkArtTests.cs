using System.Linq;
using OregonTrailDotNet.Presentation;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the landmark caption against the class of bug that got through once: the DOS cards are cropped to
    ///     varying heights, and a box placed for one height lands wrong on another. The placement itself is
    ///     asserted — the box's white rows at the clamped proportional position, the caption text inked inside it
    ///     — because dimension checks alone are true by construction for any placement at all.
    /// </summary>
    public class LandmarkArtTests
    {
        /// <summary>The caption box geometry WithCaption commits to: two 8-row text cells plus padding.</summary>
        private const int BoxHeight = 2 * 8 + 6;

        [Fact]
        public void TheCaptionBox_LandsOnEveryCard_AtTheClampedProportionalRow()
        {
            var longestName = OriginalTrail.All.Select(stop => LandmarkArt.CaptionName(stop.OriginalName))
                .OrderByDescending(name => name.Length)
                .First();

            for (var index = 0; index <= 17; index++)
            {
                var card = LandmarkArt.Card(index);
                Assert.True(card.Height >= BoxHeight, $"card p{index} is shorter than the caption box itself");

                var framed = LandmarkArt.WithCaption(card, longestName, "September 28, 1848");
                Assert.Equal(card.Width, framed.Width);
                Assert.Equal(card.Height, framed.Height);

                // The box must sit at 5/6 of the card's own height (the BASIC's rows 160-180 of 192), clamped
                // so its bottom row stays on the picture; the centre column crosses the box wherever the text
                // width lands, so both edge rows must read white there.
                var expectedTop = System.Math.Clamp(card.Height * 160 / 192, 0, card.Height - BoxHeight);
                var centre = card.Width / 2;
                Assert.Equal(Palette.White, framed.GetPixel(centre, expectedTop));
                Assert.Equal(Palette.White, framed.GetPixel(centre, expectedTop + BoxHeight - 1));

                // And the caption is actually inked inside the box: both text rows carry black glyph pixels.
                Assert.Contains(Enumerable.Range(0, card.Width),
                    x => framed.GetPixel(x, expectedTop + 3 + 4).Equals(Palette.Black));
                Assert.Contains(Enumerable.Range(0, card.Width),
                    x => framed.GetPixel(x, expectedTop + 3 + 8 + 1 + 4).Equals(Palette.Black));
            }
        }

        [Fact]
        public void TheCaptionName_StripsOnlyALeadingLowercaseThe()
        {
            // Z$ (OREGON TRAIL:260) strips a leading "the " — and only that: leading, lowercase, once.
            Assert.Equal("Kansas River crossing", LandmarkArt.CaptionName("the Kansas River crossing"));
            Assert.Equal("The Dalles", LandmarkArt.CaptionName("The Dalles"));
            Assert.Equal("Independence", LandmarkArt.CaptionName("Independence"));

            // An interior article survives — the rule is a prefix strip, not a search-and-replace.
            Assert.Equal("Crossing the Kansas", LandmarkArt.CaptionName("Crossing the Kansas"));

            foreach (var stop in OriginalTrail.All)
            {
                var caption = LandmarkArt.CaptionName(stop.OriginalName);
                if (stop.OriginalName.StartsWith("the ", System.StringComparison.Ordinal))
                    Assert.Equal(stop.OriginalName[4..], caption);
                else
                    Assert.Equal(stop.OriginalName, caption);
            }
        }
    }
}
