using WolfCurses.Graphics;

namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     The river crossing's drawing primitives, shared by every host that plays the <c>CROSS.LIB</c> picture:
    ///     the three fan sweeps, the fill and clipped line they are made of, and the <c>float</c>-sheet sprite ids.
    ///     See <see cref="RiverCrossingGame" /> for where each number comes from.
    /// </summary>
    public static class CrossingArt
    {
        // float.png ids. The DOS port draws all four states the Apple II's RIVER.IMA carries, so the mapping is
        // one-for-one: I=2 ford, I=3 float, I=1 ferry, I=4 wreck.

        /// <summary>The fording wagon on the <c>float</c> sheet.</summary>
        public const int FordSprite = 1;

        /// <summary>The caulked, floating wagon on the <c>float</c> sheet.</summary>
        public const int FloatSprite = 2;

        /// <summary>The ferry on the <c>float</c> sheet.</summary>
        public const int FerrySprite = 3;

        /// <summary>The capsized wreck on the <c>float</c> sheet.</summary>
        public const int WreckSprite = 8;

        /// <summary>
        ///     `:50010` — <c>FOR L = 0 TO 52: HPLOT 114+2*L,136 TO 218,77+L</c>. The fan pivots off the bottom-right
        ///     corner: its start slides along the foot of the picture while its end slides down the right-hand side,
        ///     so the region swept is the triangle <c>(114,136) (218,136) (218,77)</c> — which is exactly where the
        ///     Apple II's river picture puts the bank. Run to completion it paints that bank; run partway in water it
        ///     is the party pulling away from it.
        /// </summary>
        public static void NearBank(PixelBuffer canvas, double progress, Rgba32 colour)
        {
            foreach (var l in Fan(RiverCrossingGame.CrossingSteps, progress))
                Line(canvas,
                    RiverCrossingGame.ToX(114 + 2 * l), RiverCrossingGame.ToY(136.0),
                    RiverCrossingGame.ToX(218.0), RiverCrossingGame.ToY(77 + l), colour);
        }

        /// <summary>
        ///     `:50030` — <c>FOR L = 0 TO 60: HPLOT 58,24+L TO 58+2*L,24</c>, in <c>HCOLOR=4</c>, which is black, and
        ///     black is what that scene draws land in. The same primitive as the near bank, fired into the opposite
        ///     corner: the far shore coming into view, and the only thing that marks a clean crossing.
        /// </summary>
        public static void FarBank(PixelBuffer canvas, double progress, Rgba32 colour)
        {
            foreach (var l in Fan(RiverCrossingGame.LandingSteps, progress))
                Line(canvas,
                    RiverCrossingGame.ToX(58.0), RiverCrossingGame.ToY(24 + l),
                    RiverCrossingGame.ToX(58 + 2 * l), RiverCrossingGame.ToY(24.0), colour);
        }

        /// <summary>
        ///     `:50060` — <c>FOR L = 0 TO 21: HPLOT 80+2*L,71 TO 180,100-L</c>, a wedge of water fanned across the
        ///     wagon's middle. It covers everything below y=71, so what is left showing is the top of the canvas:
        ///     swamped, not removed.
        /// </summary>
        public static void SwampWedge(PixelBuffer canvas, double progress, Rgba32 colour)
        {
            foreach (var l in Fan(RiverCrossingGame.SwampSteps, progress))
                Line(canvas,
                    RiverCrossingGame.ToX(80 + 2 * l), RiverCrossingGame.ToY(71.0),
                    RiverCrossingGame.ToX(180.0), RiverCrossingGame.ToY(100 - l), colour);
        }

        /// <summary>The sub-step positions of a sweep that is <paramref name="progress" /> of the way through.</summary>
        public static IEnumerable<double> Fan(int steps, double progress)
        {
            var last = steps * progress * RiverCrossingGame.FanSubdivision;
            for (var s = 0; s < last; s++)
                yield return s / (double) RiverCrossingGame.FanSubdivision;
        }

        /// <summary>A solid rectangle, which is all <c>&amp; BOX</c> is.</summary>
        public static void Fill(PixelBuffer canvas, int x, int y, int width, int height, Rgba32 colour)
        {
            for (var dy = 0; dy < height; dy++)
            for (var dx = 0; dx < width; dx++)
                canvas.SetPixel(x + dx, y + dy, colour);
        }

        /// <summary>The original draws these sweeps with <c>HPLOT ... TO ...</c>; this is that, clipped.</summary>
        public static void Line(PixelBuffer canvas, int x1, int y1, int x2, int y2, Rgba32 colour)
        {
            var dx = Math.Abs(x2 - x1);
            var dy = -Math.Abs(y2 - y1);
            var stepX = x1 < x2 ? 1 : -1;
            var stepY = y1 < y2 ? 1 : -1;
            var error = dx + dy;

            while (true)
            {
                if (x1 >= 0 && x1 < canvas.Width && y1 >= 0 && y1 < canvas.Height)
                    canvas.SetPixel(x1, y1, colour);

                if (x1 == x2 && y1 == y2)
                    return;

                var doubled = 2 * error;
                if (doubled >= dy)
                {
                    error += dy;
                    x1 += stepX;
                }

                if (doubled > dx)
                    continue;

                error += dx;
                y1 += stepY;
            }
        }
    }
}
