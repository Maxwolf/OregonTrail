using System.Text;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     The trail map, filling in as the party walks it — the screen the original puts up for "Look at map".
    ///     <para>
    ///         The picture is the DOS port's <c>map.png</c> and the route is drawn over it, which is exactly what the
    ///         original does: <c>M0.PCK</c> is loaded as a backdrop and <c>MAP.LIB</c> <c>HPLOT</c>s the line on top.
    ///         See <see cref="MapGame" /> for the three lines of BASIC this is.
    ///     </para>
    /// </summary>
    [ParentWindow(typeof(MinigamesWindow))]
    public sealed class MapForm : SceneForm
    {
        private MapGame _game = null!;
        private PixelBuffer _map = null!;
        private bool _paused;

        /// <summary>Initializes a new instance of the <see cref="MapForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public MapForm(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Exactly what the layout costs, and no more: the window's own chrome line, the blank that starts the
        ///     picture on a line of its own, one status line, and the two-line footer. Five — the picture takes every
        ///     other row.
        ///     <para>
        ///         There is no spare, deliberately, because on this section a spare row is map. The safety that buys
        ///         is instead in the caption being short enough not to wrap: at their longest the status and footer
        ///         run 69 and 78 characters, so they hold on anything 80 columns or wider.
        ///     </para>
        /// </summary>
        protected override int ReservedRows => 5;

        /// <summary>
        ///     Left-aligned, because this is the one section that draws its picture before any text. See
        ///     <see cref="SceneForm.CenterPicture" /> — centring would leave the columns beside the map holding
        ///     whatever the previous screen put there, which is the menu.
        /// </summary>
        protected override bool CenterPicture => false;

        /// <summary>
        ///     Slow enough to watch the line creep. The whole trail is a little over 2000 miles and this covers
        ///     <see cref="MilesPerTick" /> of it a tick, so a full run takes about half a minute.
        /// </summary>
        protected override int DefaultTicksPerSecond => 12;

        /// <summary>Miles a tick buys — chosen for the run to take about 30 seconds end to end.</summary>
        private const double MilesPerTick = 6.0;

        /// <inheritdoc />
        protected override void Build()
        {
            _game = new MapGame();
            _map = Assets.Load("dos/mcga/map.png");
        }

        /// <inheritdoc />
        protected override void Advance()
        {
            if (!_paused && !_game.Finished)
                _game.Advance(MilesPerTick);
        }

        /// <inheritdoc />
        protected override void OnSectionKey(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.Spacebar:
                    _paused = !_paused;
                    break;
                case ConsoleKey.R:
                    _game.Reset();
                    break;
                case ConsoleKey.F:
                    // Step through: random at forks, then force each branch, so both can be seen.
                    _game.PreferSecondAtForks = _game.PreferSecondAtForks switch
                    {
                        null => false,
                        false => true,
                        _ => null
                    };
                    _game.Reset();
                    break;
                default:
                    return;
            }

            Invalidate();
        }

        /// <summary>
        ///     Picture first, then the words.
        ///     <para>
        ///         The other sections print their heads-up above the artwork, which suits them — they are small scenes
        ///         with a lot to say about them. This one is the opposite: the map is the whole point, it wants every
        ///         row it can get, and the readable-lettering problem is entirely a matter of how many rows are left
        ///         over. So the caption sits underneath and is kept to two lines, which is where the map gets the
        ///         space from — enough of it that the close-up this section used to carry is gone.
        ///     </para>
        /// </summary>
        protected override string Compose()
        {
            var text = new StringBuilder();

            // The leading newline matters: without it the first row of the picture is appended to the window's own
            // chrome line and the whole block lands shifted across the screen.
            text.AppendLine();
            text.Append(AnsiImage.FromPixels(Picture()).ToAnsi(PictureOptions()));
            // Terminates the picture's last row; it is not a spacer. On this section a spare row is map.
            text.AppendLine();

            var forks = _game.PreferSecondAtForks switch
            {
                null => "random",
                false => "Green River / Walla Walla",
                _ => "Fort Bridger / Dalles direct"
            };

            // Kept to one short line on purpose: it is below the picture now, so anything that wraps steals a row
            // from the map. The fork setting rides in the footer beside the key that changes it.
            text.AppendLine(_game.Finished
                ? $"*** THE WILLAMETTE VALLEY *** {_game.MilesTravelled:0} miles, {_game.Visited.Count} landmarks"
                : $"{_game.Visited.Count,2}/18  {_game.MilesTravelled,5:0} mi  —  " +
                  $"{MapGame.Landmarks[_game.From].Name} to {MapGame.Landmarks[_game.To].Name}, " +
                  $"{_game.MilesRemaining,4:0} of {_game.LegMiles} to go");

            text.Append(Footer(
                $"SPACE {(_paused ? "resume" : "pause")}   F forks: {forks}   R restart"));
            return text.ToString();
        }

        /// <summary>
        ///     The whole map, shaped to fill the console.
        ///     <para>
        ///         It used to be a choice between this and a close-up that cropped around the party, because at the
        ///         sizes the map was then drawn its 4x5 lettering reduced to about a pixel and no care in the
        ///         resampling brought it back. Giving the map the whole window instead is what made the labels read,
        ///         and it retired the close-up: there is nothing it could show that is not already legible here.
        ///     </para>
        ///     <para>
        ///         <b>Works at the map's own 640-wide grid, unsqueezed.</b> Halving the width first is what makes the
        ///         aspect true — a 640x200 mode has half-width pixels — but it costs half the horizontal detail, and
        ///         in a terminal there is none to spare. It also strands the picture: at 320x200 the map is 1.6 wide
        ///         to 1 tall against a console nearer 2.7, so it fits by height and leaves the last third of the
        ///         window empty. At 640 it is 3.2 and fills the width instead, which is both the bigger picture and
        ///         nearly twice the lettering. The cost is that the map reads wider than its true proportions.
        ///     </para>
        /// </summary>
        private PixelBuffer Picture()
        {
            var frame = Plotted(_map);
            var (columns, rows) = Available();

            // To fill the space rather than letterbox into it, match its shape by stretching the map vertically --
            // and stop there. Resampling all the way down to the cell grid here instead looks obviously wrong: the
            // renderer resolves finer than one map pixel per cell, so handing it a picture already reduced to that
            // grid throws the difference away and the whole map comes out a grey blur. Full width in, let the
            // renderer do the reduction.
            //
            // Stretching at all is faithful: the DOS game spreads this same 640x200 picture over a whole screen.
            var stretched = frame.Width * (rows * 2) / Math.Max(1, columns);
            return stretched == frame.Height ? frame : FitTo(frame, frame.Width, Math.Max(1, stretched));
        }

        /// <summary>
        ///     The picture area the console has left, in cells: one map pixel across and, being a half-block, two
        ///     down. Falls back to a small guess where there is no real console to ask.
        /// </summary>
        private (int Columns, int Rows) Available()
        {
            try
            {
                return (Console.WindowWidth, Math.Max(4, Console.WindowHeight - ReservedRows));
            }
            catch (Exception)
            {
                return (80, Math.Max(4, 24 - ReservedRows));
            }
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
        private static PixelBuffer FitTo(PixelBuffer source, int width, int height)
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
        ///     Copies the map and plots the route over it, at the map's own 640-wide scale.
        ///     <para>
        ///         Returns the frame <b>unsqueezed</b>, deliberately: the landmark coordinates are in this space, and
        ///         the caller decides whether it wants the whole thing halved for an overview or a slice of it at full
        ///         width. Squeezing here instead is what broke it — <see cref="Picture" /> then halved an already
        ///         halved frame and ran straight off the end of it.
        ///     </para>
        /// </summary>
        private PixelBuffer Plotted(PixelBuffer map)
        {
            var frame = new PixelBuffer(map.Width, map.Height);
            frame.DrawImage(map, 0, 0);

            // :50000 -- the finished route, a polyline through every landmark actually reached.
            for (var i = 0; i + 1 < _game.Visited.Count; i++)
            {
                var a = MapGame.Landmarks[_game.Visited[i]];
                var b = MapGame.Landmarks[_game.Visited[i + 1]];
                Line(frame, a.X, a.Y, b.X, b.Y);
            }

            // :50010 -- the leg in progress, growing out of the landmark just left.
            if (_game.Finished)
                return frame;

            var from = MapGame.Landmarks[_game.From];
            var to = MapGame.Landmarks[_game.To];
            var t = _game.LegProgress;
            Line(frame, from.X, from.Y,
                (int) Math.Round(from.X + (to.X - from.X) * t),
                (int) Math.Round(from.Y + (to.Y - from.Y) * t));

            return frame;
        }

        /// <summary>
        ///     A one-pixel line, which is all <c>HPLOT ... TO ...</c> is. Bresenham rather than the original's
        ///     slope-and-step, because the BASIC's <c>L = (Y2-Y1)/(X2-X1)</c> divides by zero on a vertical leg and
        ///     nothing on this trail happens to be vertical — a fragility not worth reproducing.
        /// </summary>
        private static void Line(PixelBuffer canvas, int x1, int y1, int x2, int y2)
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
        private static void Plot(PixelBuffer canvas, int x, int y)
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
