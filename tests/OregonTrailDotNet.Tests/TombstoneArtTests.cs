using OregonTrailDotNet.Presentation;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the inscription layout rules the game depends on: the original's 29-char epitaphs always fit the
    ///     stone's 21x5 text window, but the game allows 38 — so over-long words hard-break at the column edge and
    ///     the blank spacer row drops when the epitaph needs three rows, keeping the whole inscription on the stone.
    /// </summary>
    public class TombstoneArtTests
    {
        [Fact]
        public void Wrap_HardBreaks_AWordLongerThanARow()
        {
            var wrapped = TombstoneArt.Wrap(new string('x', 38), TombstoneArt.Columns);

            Assert.Equal(2, wrapped.Count);
            Assert.Equal(new string('x', 21), wrapped[0]);
            Assert.Equal(new string('x', 17), wrapped[1]);
        }

        [Fact]
        public void Inscription_KeepsTheSpacer_ForAnOriginalLengthEpitaph()
        {
            // 29 characters always wrap to two rows, so the classic four-line layout stands.
            var lines = TombstoneArt.Inscription("Maxwolf", "Rest in peace weary traveler", out var epitaphRow);

            Assert.Equal(string.Empty, lines[2]);
            Assert.Equal(3, epitaphRow);
            Assert.True(lines.Count <= TombstoneArt.Rows);
        }

        [Fact]
        public void Inscription_DropsTheSpacer_WhenTheEpitaphNeedsThreeRows()
        {
            // Three 11-12 character words cannot share rows at 21 columns: three epitaph rows, spacer dropped,
            // and the whole inscription still fits the five-row window.
            var lines = TombstoneArt.Inscription("Maxwolf", "abcdefghijk abcdefghij abcdefghijkl",
                out var epitaphRow, 38);

            Assert.Equal(2, epitaphRow);
            Assert.Equal(5, lines.Count);
            Assert.DoesNotContain(string.Empty, lines);
        }

        [Fact]
        public void Inscription_HonorsTheCallersLimit()
        {
            // The default clips at the original's 29; the game passes its own cap and keeps all 38 characters.
            var epitaph = new string('a', 20) + " " + new string('b', 17);

            var clipped = TombstoneArt.Inscription("Maxwolf", epitaph, out _);
            var full = TombstoneArt.Inscription("Maxwolf", epitaph, out _, 38);

            Assert.Equal(new string('a', 20), clipped[3]);
            Assert.Contains(new string('b', 17), full[full.Count - 1]);
        }
    }
}
