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
        private const int ShoreSign = 6;

        private RaftGame _game = null!;
        private SpriteScene _scene = null!;
        private Sprite _raft = null!;
        private Sprite _sign = null!;
        private Sprite _landing = null!;
        private Sprite[] _rocks = null!;
        private RaftSteerEnum _pending = RaftSteerEnum.None;

        /// <summary>Initializes a new instance of the <see cref="RaftForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public RaftForm(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override int ReservedRows => 9;

        /// <inheritdoc />
        protected override void Build()
        {
            _game = new RaftGame();
            _scene = new SpriteScene(BuildRiver());

            // Draw order is list order: bank scenery first, then the rocks, then the raft over both.
            _sign = new Sprite(Assets.Dos("float", ShoreSign)) { Visible = false };
            _landing = new Sprite(Assets.Dos("float", ShoreSign)) { Visible = false };
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

            SyncSprites();
        }

        /// <inheritdoc />
        protected override void Advance()
        {
            var steer = _pending;
            _pending = RaftSteerEnum.None;

            _game.Step(steer);
            SyncSprites();
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
                    SyncSprites();
                    Invalidate();
                    break;
            }
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var text = new StringBuilder();
            text.AppendLine();
            text.AppendLine("COLUMBIA RIVER — guide the raft, then land at the Willamette trail.");

            var drift = _game.Drift switch
            {
                < 0 => "<< toward the far bank",
                > 0 => "toward the near bank >>",
                _ => "holding"
            };

            text.AppendLine(
                $"tick {_game.Tick,3}/{RaftGame.LandingClosesAfter}   lane {_game.Lane,2}/{RaftGame.LastLane}   " +
                $"drift {_game.Drift,2} ({drift})   rocks hit {_game.RockHits}   bank {_game.ShoreHits}");

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

        /// <summary>Copies simulation positions onto the sprites, scaling out of FLOAT's screen into MCGA's.</summary>
        private void SyncSprites()
        {
            _raft.X = SceneX(RaftGame.LaneX(_game.Lane));
            _raft.Y = SceneY(RaftGame.LaneY(_game.Lane));

            for (var i = 0; i < _rocks.Length; i++)
            {
                _rocks[i].Visible = _game.Rocks[i].Active;
                _rocks[i].X = SceneX(_game.Rocks[i].X);
                _rocks[i].Y = SceneY(_game.Rocks[i].Y);
            }

            _sign.Visible = _game.Sign.Active;
            _sign.X = SceneX(_game.Sign.X);
            _sign.Y = SceneY(_game.Sign.Y);

            _landing.Visible = _game.Landing.Active;
            _landing.X = SceneX(_game.Landing.X);
            _landing.Y = SceneY(_game.Landing.Y);
        }

        private static int SceneX(int x) => x * Assets.DosWidth / RaftGame.ScreenWidth;

        private static int SceneY(int y) => y * Assets.DosHeight / RaftGame.ScreenHeight;

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

            var water = new Rgba32(64, 176, 252, 255);   // float.png's own key colour: the DOS water
            var shallow = new Rgba32(120, 204, 252, 255);
            var sand = new Rgba32(252, 184, 144, 255);   // the shore block's tan
            var grass = new Rgba32(4, 156, 0, 255);      // MCGA grass, as used on the landmark cards

            for (var y = 0; y < river.Height; y++)
            for (var x = 0; x < river.Width; x++)
            {
                // Back into FLOAT's screen, then across the river into lane coordinates.
                var fx = x * (double) RaftGame.ScreenWidth / Assets.DosWidth;
                var fy = y * (double) RaftGame.ScreenHeight / Assets.DosHeight;
                var lane = RaftGame.LaneOf(fx, fy);

                var colour = lane switch
                {
                    < -1.5 => grass,
                    < -0.3 => sand,
                    < 0.7 => shallow,
                    <= 16.3 => water,
                    <= 17.3 => shallow,
                    <= 18.6 => sand,
                    _ => grass
                };

                river.SetPixel(x, y, colour);
            }

            return river;
        }
    }
}
