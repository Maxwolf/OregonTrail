using WolfCurses.Graphics;

namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     The Columbia raft's synthesized artwork: the river backdrop the DOS port never shipped, and the bank
    ///     markers that arrive merged into one cut sprite. Everything here is derived from the game's own geometry
    ///     or the sheet's own colours rather than drawn by eye, so any host draws the same river.
    /// </summary>
    public static class RaftArt
    {
        // float.png sprite ids, established by cutting the sheet and looking at it. The small raft is the steerable
        // one: the rocks are 14-15px, so it is about twice a rock across, where the big raft is over five times and
        // would leave nowhere to dodge on a 320px river.

        /// <summary>The steerable raft on the <c>float</c> sheet.</summary>
        public const int SmallRaft = 5;

        /// <summary>A midstream rock on the <c>float</c> sheet.</summary>
        public const int Rock = 4;

        /// <summary>
        ///     Both bank markers arrive as <b>one</b> cut sprite, and have to be split apart before use.
        ///     <para>
        ///         The sheet is keyed on its water blue, so the sand these two are drawn on is opaque content as far as
        ///         the cutter is concerned, and the arrow post and the trail squiggle flood-fill as a single 57x35
        ///         block. Blitting it whole put a peach rectangle on the river — and used the same picture for both
        ///         markers, when the original clearly draws two: <c>:1148</c> is <c>&amp; IMAGE,5</c> for the three
        ///         direction signs and <c>:1149</c> is <c>&amp; IMAGE,6</c> for the landing.
        ///     </para>
        /// </summary>
        public const int MarkerBlock = 6;

        /// <summary>
        ///     Pulls one bank marker out of the block the cutter merged, dropping the sand it was drawn on.
        ///     <para>
        ///         The split is found rather than hard-coded: the sand is a single flat colour, so the columns holding
        ///         anything else fall into two runs with a clear gap between them — the arrow post on the left, the
        ///         trail squiggle on the right. Keying the sand out as well as cropping to it matters, because these
        ///         ride the bank at an angle while a sprite is an upright rectangle; carrying the sand along would put
        ///         square corners of beach out over the water as the marker drifts past the waterline.
        ///     </para>
        /// </summary>
        /// <param name="block">The merged sprite, <see cref="MarkerBlock" />.</param>
        /// <param name="right">True for the right-hand marker (the landing), false for the left (the direction sign).</param>
        public static PixelBuffer Marker(PixelBuffer block, bool right)
        {
            var sand = Palette.Sand;
            bool IsSand(int x, int y)
            {
                var pixel = block.GetPixel(x, y);
                return pixel.A == 0 || (pixel.R == sand.R && pixel.G == sand.G && pixel.B == sand.B);
            }

            // Columns that hold something other than sand, then the two runs they group into.
            var used = new bool[block.Width];
            for (var x = 0; x < block.Width; x++)
            for (var y = 0; y < block.Height; y++)
                if (!IsSand(x, y))
                {
                    used[x] = true;
                    break;
                }

            var runs = new List<(int Start, int End)>();
            for (var x = 0; x < block.Width; x++)
            {
                if (!used[x])
                    continue;

                if (runs.Count > 0 && runs[^1].End == x - 1)
                    runs[^1] = (runs[^1].Start, x);
                else
                    runs.Add((x, x));
            }

            var (start, end) = runs[right ? ^1 : 0];

            // Trim vertically too, so each marker sits on its own baseline rather than the taller one's.
            int top = block.Height, bottom = -1;
            for (var y = 0; y < block.Height; y++)
            for (var x = start; x <= end; x++)
                if (!IsSand(x, y))
                {
                    top = Math.Min(top, y);
                    bottom = Math.Max(bottom, y);
                    break;
                }

            var marker = new PixelBuffer(end - start + 1, bottom - top + 1);
            for (var y = top; y <= bottom; y++)
            for (var x = start; x <= end; x++)
                marker.SetPixel(x - start, y - top,
                    IsSand(x, y) ? Palette.Clear : block.GetPixel(x, y));

            return marker;
        }

        /// <summary>
        ///     Paints the river the raft crosses. The DOS port ships no river backdrop — its raft sprites are keyed
        ///     against the water colour because the water was drawn, not blitted — so this draws one, out of the game's
        ///     own geometry rather than by eye.
        ///     <para>
        ///         The trick is to measure a pixel <i>across</i> the river rather than along it.
        ///         <see cref="RaftGame.LaneOf" /> does exactly that, and the original's own constants confirm the
        ///         orientation: a rock drifting <c>(+8, -4)</c> and bank scenery drifting <c>(+6, -3)</c> both leave the
        ///         lane coordinate <b>unchanged</b>, which is only true if that is the direction the river flows. So the
        ///         banks land exactly on lanes 0 and 17 — the two lanes <c>FLOAT</c> scores as a crash.
        ///     </para>
        /// </summary>
        public static PixelBuffer BuildRiver()
        {
            var river = new PixelBuffer(Art.DosWidth, Art.DosHeight);

            // Both colours are read off float.png itself rather than picked: the sheet is keyed on its water, and the
            // shore block it ships is that tan. Those two are the whole palette — the banks of the Columbia here are
            // bare arid sand, with no grass and no shallows, and the water meets them on one hard diagonal edge.
            var water = Palette.Water;
            var sand = Palette.Sand;

            for (var y = 0; y < river.Height; y++)
            for (var x = 0; x < river.Width; x++)
            {
                // Back into FLOAT's screen, then across the river into lane coordinates.
                var fx = x * (double) RaftGame.ScreenWidth / Art.DosWidth;
                var fy = y * (double) RaftGame.ScreenHeight / Art.DosHeight;
                var lane = RaftGame.LaneOf(fx, fy);

                river.SetPixel(x, y, lane is >= -0.5 and <= 17.5 ? water : sand);
            }

            return river;
        }
    }
}
