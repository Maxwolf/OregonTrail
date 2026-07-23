using OregonTrailDotNet.Presentation;
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
    public sealed class RaftForm : WorkbenchSceneForm
    {
        // The sprite ids, the marker-block split and the synthesized river all live in RaftArt now, comments and
        // all — they are the pieces a game-side raft scene reuses verbatim.
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
            _scene = new SpriteScene(RaftArt.BuildRiver());

            // Draw order is list order: bank scenery first, then the rocks, then the raft over both.
            var markers = Art.Dos("float", RaftArt.MarkerBlock);
            _sign = new Sprite(RaftArt.Marker(markers, false)) { Visible = false };
            _landing = new Sprite(RaftArt.Marker(markers, true)) { Visible = false };
            _rocks =
            [
                new Sprite(Art.Dos("float", RaftArt.Rock)) { Visible = false },
                new Sprite(Art.Dos("float", RaftArt.Rock)) { Visible = false }
            ];
            _raft = new Sprite(Art.Dos("float", RaftArt.SmallRaft));

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

        private static int SceneX(double x) => (int) Math.Round(x * Art.DosWidth / RaftGame.ScreenWidth);

        private static int SceneY(double y) => (int) Math.Round(y * Art.DosHeight / RaftGame.ScreenHeight);
    }
}
