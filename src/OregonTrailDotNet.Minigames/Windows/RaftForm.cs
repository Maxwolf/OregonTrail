using System.Text;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     The Columbia raft, played with the 1990 DOS port's sprites.
    ///     <para>
    ///         <see cref="RaftGame" /> still thinks in the Apple II's 280x192, because that is the space every constant
    ///         in <c>FLOAT</c> was written for and the space the port was validated in. Only at the moment of drawing
    ///         are lane positions scaled into the 320x200 MCGA surface, so the artwork can change without the logic
    ///         moving underneath it.
    ///     </para>
    /// </summary>
    [ParentWindow(typeof(MinigamesWindow))]
    public sealed class RaftForm : SceneForm
    {
        // float.png sprite ids, established by cutting the sheet and looking at it. The small raft is the steerable
        // one: the rocks are 14-15px, so it is about twice a rock across, where the big raft is over five times and
        // would leave nowhere to dodge on a 320px river.
        private const int SmallRaft = 5;
        private const int Rock = 4;

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
        private const int MarkerBlock = 6;

        private RaftGame _game = null!;
        private SpriteScene _scene = null!;
        private Sprite _raft = null!;
        private Sprite _sign = null!;
        private Sprite _landing = null!;
        private Sprite[] _rocks = null!;
        private RaftSteerEnum _pending = RaftSteerEnum.None;
        private Placement _wasRaft;
        private Placement[] _wasRocks = null!;
        private Placement _wasSign, _wasLanding;

        /// <summary>Initializes a new instance of the <see cref="RaftForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public RaftForm(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override int ReservedRows => 9;

        /// <summary>
        ///     Deliberately far slower than the other sections. A tick here is not a frame of animation — it is a whole
        ///     lane, a seventeenth of the river, so at the workbench's usual 20/sec the raft crosses the Columbia in
        ///     under a second and no key press can land in time.
        ///     <para>
        ///         The original cannot be read off the listing: <c>:1070</c>-<c>:1180</c> has no delay in it, so it ran
        ///         at whatever Applesoft managed, and every pass costs it two spawn rolls, up to six <c>&amp;</c> sprite
        ///         blits and four collision rectangles. That is a handful of passes a second, not twenty. Nine puts the
        ///         225-tick run at about twenty-five seconds, which is the length the landing windows are cut for —
        ///         <c>:1145</c> spaces the three signs sixty ticks apart, and they want to read as landmarks going by
        ///         rather than as a flicker.
        ///     </para>
        /// </summary>
        protected override int DefaultTicksPerSecond => 9;

        /// <inheritdoc />
        protected override void Build()
        {
            _game = new RaftGame();
            _scene = new SpriteScene(BuildRiver());

            // Draw order is list order: bank scenery first, then the rocks, then the raft over both.
            var markers = Assets.Dos("float", MarkerBlock);
            _sign = new Sprite(Marker(markers, false)) { Visible = false };
            _landing = new Sprite(Marker(markers, true)) { Visible = false };
            _rocks =
            [
                new Sprite(Assets.Dos("float", Rock)) { Visible = false },
                new Sprite(Assets.Dos("float", Rock)) { Visible = false }
            ];
            _raft = new Sprite(Assets.Dos("float", SmallRaft));

            _scene.Sprites.Add(_sign);
            _scene.Sprites.Add(_landing);
            foreach (var rock in _rocks)
                _scene.Sprites.Add(rock);
            _scene.Sprites.Add(_raft);

            Rewind();
            SyncSprites();
        }

        /// <summary>
        ///     Collapses the interpolation, so the next frame draws the current state outright rather than easing into
        ///     it from wherever the last run happened to leave things.
        /// </summary>
        private void Rewind()
        {
            _wasRaft = RaftNow();
            _wasRocks = _game.Rocks.Select(Where).ToArray();
            _wasSign = Where(_game.Sign);
            _wasLanding = Where(_game.Landing);
        }

        /// <summary>
        ///     Four frames to a step. A step moves the raft a whole lane — about ten pixels once scaled — and slides
        ///     every rock <c>(+8, -4)</c>, so drawing only on the step reads as a chunky hop rather than a river.
        ///     Quartering it puts the raft at roughly 2px a frame, which reads as motion instead of as a grid.
        ///     <para>
        ///         Four rather than more because the ceiling is the console, not the compositing: a frame costs about
        ///         1.8ms to compose but 65KB of ANSI to emit, so 4 x 9 = 36fps is already 2.3MB/sec of terminal
        ///         throughput. Going higher buys very little smoothness for a lot more bandwidth.
        ///     </para>
        ///     <para>
        ///         The cost is that what is drawn trails the simulation by up to one step — around 110ms here, 55ms on
        ///         average. That is the price of interpolating between two known positions rather than guessing at the
        ///         next one, and it is much the better trade: extrapolating would make the raft visibly snap backwards
        ///         every time the player changed the drift.
        ///     </para>
        /// </summary>
        protected override int FramesPerStep => 4;

        /// <inheritdoc />
        protected override void Advance()
        {
            var steer = _pending;
            _pending = RaftSteerEnum.None;

            // Remember where everything stood, so the frames between this step and the next can be drawn.
            Rewind();
            _game.Step(steer);
        }

        /// <inheritdoc />
        protected override void OnSectionKey(ConsoleKey key)
        {
            switch (key)
            {
                // Up and left are one direction, down and right the other, because the river runs diagonally. The
                // 1985 BASIC and the 1990 Pascal fold the keys the same way, five years and two languages apart.
                case ConsoleKey.LeftArrow:
                case ConsoleKey.UpArrow:
                    _pending = RaftSteerEnum.Far;
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.DownArrow:
                    _pending = RaftSteerEnum.Near;
                    break;
                case ConsoleKey.R:
                    _game.Reset();
                    Rewind();
                    SyncSprites();
                    Invalidate();
                    break;
            }
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            // Every frame, not just every step: this is where the in-between positions are laid down.
            SyncSprites();

            var text = new StringBuilder();
            text.AppendLine();
            text.AppendLine("COLUMBIA RIVER — guide the raft, then land at the Willamette trail.");

            var drift = _game.Drift switch
            {
                < 0 => "<< toward the far bank",
                > 0 => "toward the near bank >>",
                _ => "holding"
            };

            // No hit counters: a run now ends on the first contact, so they only ever read 0 or 1.
            text.AppendLine(
                $"tick {_game.Tick,3}/{RaftGame.LandingClosesAfter}   lane {_game.Lane,2}/{RaftGame.LastLane}   " +
                $"drift {_game.Drift,2} ({drift})");

            text.AppendLine(_game.Outcome switch
            {
                RaftOutcomeEnum.Landed => "*** LANDED ***  R to run it again.",
                RaftOutcomeEnum.Missed => "*** MISSED THE LANDING ***  R to run it again.",
                _ when _game.LandingWindowOpen =>
                    $"*** LANDING OPEN *** reach lane {RaftGame.LastLane} — " +
                    $"{RaftGame.LandingClosesAfter - _game.Tick} ticks left!",
                _ when _game.Tick > RaftGame.LandingAppearsTick =>
                    $"The landing is in sight — it opens in {RaftGame.LandingOpensAfter - _game.Tick + 1} ticks.",
                _ => _game.LastEvent
            });

            text.AppendLine(Footer("ARROWS steer (a nudge, not a move)   R restart"));
            text.Append(_scene.ToAnsi(PictureOptions()));
            return text.ToString();
        }

        /// <summary>
        ///     Copies simulation positions onto the sprites, scaling out of FLOAT's screen into MCGA's and easing each
        ///     one from where it stood at the last step toward where it stands now.
        /// </summary>
        private void SyncSprites()
        {
            Place(_raft, _wasRaft, RaftNow(), 0, 0);

            for (var i = 0; i < _rocks.Length; i++)
                Place(_rocks[i], _wasRocks[i], Where(_game.Rocks[i]),
                    RaftGame.RockDriftX, RaftGame.RockDriftY);

            Place(_sign, _wasSign, Where(_game.Sign), RaftGame.SceneryDriftX, RaftGame.SceneryDriftY);
            Place(_landing, _wasLanding, Where(_game.Landing), RaftGame.SceneryDriftX, RaftGame.SceneryDriftY);
        }

        /// <summary>
        ///     Puts one sprite somewhere between where it was and where it is.
        ///     <para>
        ///         A slot that has just filled has no previous position to ease from, and must <b>not</b> be eased from
        ///         where the last occupant died — that is usually most of a screen away, and would sweep the new rock
        ///         right across the water in front of the player. Instead the missing position is reconstructed by
        ///         stepping the spawn point back along the drift, so the rock slides in from off the edge rather than
        ///         popping into place and then sitting still for a step.
        ///     </para>
        /// </summary>
        /// <param name="sprite">The sprite to position.</param>
        /// <param name="was">Where it stood at the previous step.</param>
        /// <param name="now">Where it stands at the current one.</param>
        /// <param name="driftX">How far this kind of thing travels per step, used only to enter smoothly.</param>
        /// <param name="driftY">The vertical half of the same.</param>
        private void Place(Sprite sprite, Placement was, Placement now, double driftX, double driftY)
        {
            sprite.Visible = now.Visible;
            if (!now.Visible)
                return;

            var from = was.Visible ? was : new Placement(now.X - driftX, now.Y - driftY, true);
            sprite.X = SceneX(from.X + (now.X - from.X) * FrameProgress);
            sprite.Y = SceneY(from.Y + (now.Y - from.Y) * FrameProgress);
        }

        private Placement RaftNow() => new(RaftGame.LaneX(_game.Lane), RaftGame.LaneY(_game.Lane), true);

        private static Placement Where(RaftGame.Drifter drifter) => new(drifter.X, drifter.Y, drifter.Active);

        /// <summary>Where something stood at one instant, in FLOAT's own screen space.</summary>
        /// <param name="X">Left edge.</param>
        /// <param name="Y">Top edge.</param>
        /// <param name="Visible">Whether it was on screen at all.</param>
        private readonly record struct Placement(double X, double Y, bool Visible);

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
        private static PixelBuffer Marker(PixelBuffer block, bool right)
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

        private static int SceneX(double x) => (int) Math.Round(x * Assets.DosWidth / RaftGame.ScreenWidth);

        private static int SceneY(double y) => (int) Math.Round(y * Assets.DosHeight / RaftGame.ScreenHeight);

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
        private static PixelBuffer BuildRiver()
        {
            var river = new PixelBuffer(Assets.DosWidth, Assets.DosHeight);

            // Both colours are read off float.png itself rather than picked: the sheet is keyed on its water, and the
            // shore block it ships is that tan. Those two are the whole palette — the banks of the Columbia here are
            // bare arid sand, with no grass and no shallows, and the water meets them on one hard diagonal edge.
            var water = Palette.Water;
            var sand = Palette.Sand;

            for (var y = 0; y < river.Height; y++)
            for (var x = 0; x < river.Width; x++)
            {
                // Back into FLOAT's screen, then across the river into lane coordinates.
                var fx = x * (double) RaftGame.ScreenWidth / Assets.DosWidth;
                var fy = y * (double) RaftGame.ScreenHeight / Assets.DosHeight;
                var lane = RaftGame.LaneOf(fx, fy);

                river.SetPixel(x, y, lane is >= -0.5 and <= 17.5 ? water : sand);
            }

            return river;
        }
    }
}
