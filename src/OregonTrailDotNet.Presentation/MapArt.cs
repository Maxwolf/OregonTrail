using WolfCurses.Graphics;

namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     The trail map's drawing, shared by every host that shows it: route plotting over the DOS
    ///     <c>map.png</c> backdrop and the vertical stretch that shapes the 640-wide picture to a console.
    ///     The picture is exactly what the original does — <c>M0.PCK</c> is loaded as a backdrop and
    ///     <c>MAP.LIB</c> <c>HPLOT</c>s the line on top; see <see cref="MapGame" /> for the three lines of
    ///     BASIC this is.
    /// </summary>
    public static class MapArt
    {
        /// <summary>
        ///     Copies the map and plots the route over it, at the map's own 640-wide scale.
        ///     <para>
        ///         Returns the frame <b>unsqueezed</b>, deliberately: the landmark coordinates are in this space, and
        ///         the caller decides whether it wants the whole thing halved for an overview or a slice of it at full
        ///         width. Squeezing here instead is what broke it — the caller then halved an already halved frame and
        ///         ran straight off the end of it.
        ///     </para>
        /// </summary>
        /// <param name="map">The map backdrop; never mutated.</param>
        /// <param name="game">The journey whose route is drawn.</param>
        public static PixelBuffer Plotted(PixelBuffer map, MapGame game)
        {
            var frame = new PixelBuffer(map.Width, map.Height);
            frame.DrawImage(map, 0, 0);
            PlotRoute(frame, game);
            return frame;
        }

        /// <summary>
        ///     Plots the route in place: the finished polyline through every landmark reached, then the leg in
        ///     progress growing out of the landmark just left.
        /// </summary>
        /// <param name="picture">The frame to draw on, in the map's own 640-wide pixel space.</param>
        /// <param name="game">The journey whose route is drawn.</param>
        public static void PlotRoute(PixelBuffer picture, MapGame game)
        {
            // :50000 -- the finished route, a polyline through every landmark actually reached.
            for (var i = 0; i + 1 < game.Visited.Count; i++)
            {
                var a = MapGame.Landmarks[game.Visited[i]];
                var b = MapGame.Landmarks[game.Visited[i + 1]];
                Line(picture, a.X, a.Y, b.X, b.Y);
            }

            // :50010 -- the leg in progress, growing out of the landmark just left.
            if (game.Finished)
                return;

            var from = MapGame.Landmarks[game.From];
            var to = MapGame.Landmarks[game.To];
            var t = game.LegProgress;
            Line(picture, from.X, from.Y,
                (int) Math.Round(from.X + (to.X - from.X) * t),
                (int) Math.Round(from.Y + (to.Y - from.Y) * t));
        }

        /// <summary>
        ///     Resamples to an exact size, scaling each axis on its own.
        ///     <para>
        ///         Used only to stretch the map vertically onto the shape of the console, so it is very nearly always
        ///         magnifying — and box-averaging degrades to plain row replication when it magnifies, which is what
        ///         one-bit line art wants. Nothing is blended that does not have to be.
        ///     </para>
        /// </summary>
        /// <param name="source">The picture to resample.</param>
        /// <param name="width">Target width in pixels.</param>
        /// <param name="height">Target height in pixels.</param>
        public static PixelBuffer FitTo(PixelBuffer source, int width, int height)
        {
            var target = new PixelBuffer(width, height);
            for (var y = 0; y < height; y++)
            {
                var top = y * source.Height / height;
                var bottom = Math.Max(top + 1, (y + 1) * source.Height / height);

                for (var x = 0; x < width; x++)
                {
                    var left = x * source.Width / width;
                    var right = Math.Max(left + 1, (x + 1) * source.Width / width);

                    int r = 0, g = 0, b = 0, n = 0;
                    for (var sy = top; sy < bottom; sy++)
                    for (var sx = left; sx < right; sx++)
                    {
                        var pixel = source.GetPixel(sx, sy);
                        r += pixel.R;
                        g += pixel.G;
                        b += pixel.B;
                        n++;
                    }

                    target.SetPixel(x, y, new Rgba32((byte) (r / n), (byte) (g / n), (byte) (b / n), 255));
                }
            }

            return target;
        }

        /// <summary>
        ///     A one-pixel line, which is all <c>HPLOT ... TO ...</c> is. Bresenham rather than the original's
        ///     slope-and-step, because the BASIC's <c>L = (Y2-Y1)/(X2-X1)</c> divides by zero on a vertical leg and
        ///     nothing on this trail happens to be vertical — a fragility not worth reproducing.
        /// </summary>
        public static void Line(PixelBuffer canvas, int x1, int y1, int x2, int y2)
        {
            var dx = Math.Abs(x2 - x1);
            var dy = -Math.Abs(y2 - y1);
            var sx = x1 < x2 ? 1 : -1;
            var sy = y1 < y2 ? 1 : -1;
            var err = dx + dy;

            while (true)
            {
                Plot(canvas, x1, y1);
                if (x1 == x2 && y1 == y2)
                    return;

                var e2 = 2 * err;
                if (e2 >= dy)
                {
                    err += dy;
                    x1 += sx;
                }

                if (e2 > dx)
                    continue;

                err += dx;
                y1 += sy;
            }
        }

        /// <summary>
        ///     Marks one pixel of the route and its neighbours, a 2x2 block. The map is drawn far wider than the
        ///     console has cells, so a single-pixel line would half-vanish into the reduction; a 2x2 mark keeps it as
        ///     solid as the printed route in the legend.
        /// </summary>
        public static void Plot(PixelBuffer canvas, int x, int y)
        {
            for (var dy = 0; dy < 2; dy++)
            for (var dx = 0; dx < 2; dx++)
            {
                var px = x + dx;
                var py = y + dy;
                if (px >= 0 && px < canvas.Width && py >= 0 && py < canvas.Height)
                    canvas.SetPixel(px, py, Palette.Black);
            }
        }
    }
}
