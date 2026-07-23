using OregonTrailDotNet.Presentation;
using System.Text;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     The travel screen: the team walking, the world sliding past, and the ground changing colour under it.
    ///     <para>
    ///         Everything here is assembled, because the original assembled it — there is no travel picture on either
    ///         disk. The layers, bottom to top, are a flooded ground box, a full-width horizon strip, the leg's one
    ///         piece of scenery, and the team. See <see cref="TravelGame" /> for where each came from in the BASIC.
    ///     </para>
    ///     <para>
    ///         Two liberties are taken, both because the DOS art is the better art and it is packed differently. The
    ///         Apple II kept its two horizon strips on <i>separate floppy sides</i> — <c>PRAIRIE.IMA</c> with landmarks
    ///         0-4 and <c>MOUNTAINS.IMA</c> with 5-17 — so the terrain changed at the disk flip and the code never chose
    ///         it at all. The DOS port carries both strips in one <c>scenery</c> sheet, so here it is a key press.
    ///         And the DOS sheet has <b>no clouds</b>, where the Apple II's mountains table drew three; there is nothing
    ///         to composite, so the sky is flat.
    ///     </para>
    /// </summary>
    [ParentWindow(typeof(MinigamesWindow))]
    public sealed class TravelForm : WorkbenchSceneForm
    {
        // The sheet frame ids, backdrop layering and status-panel drawing live in TravelArt now — they are the
        // pieces a game-side drive scene reuses verbatim.
        private const int FirstWalkFrame = TravelArt.FirstWalkFrame;
        private const int BrokenWagon = TravelArt.BrokenWagon;
        private const int BurningWagon = TravelArt.BurningWagon;

        private TravelGame _game = null!;
        private SpriteScene _scene = null!;
        private Sprite _scenery = null!;
        private Sprite _wagon = null!;
        private Sprite _eventIcon = null!;
        private string _backdropKey = string.Empty;

        /// <summary>
        ///     Which event picture is parked in the sky, or null for none.
        ///     <para>
        ///         The events themselves are deliberately <b>not</b> wired up — nothing here rolls for weather or takes
        ///         a day off the clock. This is a cycle key so the art is loaded, positioned and checkable, ready for
        ///         whatever drives it later.
        ///     </para>
        /// </summary>
        private EventIconEnum? _icon;

        /// <summary>Initializes a new instance of the <see cref="TravelForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public TravelForm(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override int ReservedRows => 9;

        /// <summary>
        ///     Faster than the workbench default, to pay back the small per-tick distances
        ///     <see cref="TravelGame.MilesPerTick" /> has to use so the walk cycle cannot alias. A leg runs about
        ///     16 seconds at a grueling pace and 40 at a steady one, and the team's legs cycle roughly two and a half
        ///     times a second against one — which is the difference pace is meant to show and previously did not.
        /// </summary>
        protected override int DefaultTicksPerSecond => 15;

        /// <inheritdoc />
        protected override void Build()
        {
            _game = new TravelGame();

            _scenery = new Sprite(Art.Dos("scenery", _game.Scenery.SpriteId));
            _wagon = new Sprite(Art.Dos("travelox", FirstWalkFrame));
            _eventIcon = new Sprite(DosFrames.EventIcon(EventIconEnum.Thunderstorm)) { Visible = false };

            RebuildScene();
            SyncSprites();
        }

        /// <inheritdoc />
        protected override void Advance()
        {
            _game.Step();

            // Roll straight into the next leg so the screen keeps moving; a leg that has arrived has its scenery
            // parked at FX, which is worth seeing but not worth sitting in.
            if (_game.Arrived)
                _game.NextLeg();

            SyncSprites();
        }

        /// <inheritdoc />
        protected override void OnSectionKey(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.T:
                    _game.ToggleTerrain();
                    break;
                case ConsoleKey.W:
                    _game.CycleWeather();
                    break;
                case ConsoleKey.P:
                    _game.CyclePace();
                    break;
                case ConsoleKey.B:
                    _game.CycleWagon();
                    break;
                case ConsoleKey.N:
                    _game.NextLeg();
                    break;
                case ConsoleKey.E:
                    CycleEventIcon();
                    break;
                case ConsoleKey.R:
                    _game.Reset();
                    break;
                default:
                    return;
            }

            SyncSprites();
            Invalidate();
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var text = new StringBuilder();
            text.AppendLine();
            text.AppendLine("THE TRAIL — the team is nailed to one spot and the world slides past it.");

            var terrain = _game.Terrain == TravelTerrainEnum.Plains
                ? "plains  (PRAIRIE.IMA, side A)"
                : "mountains  (MOUNTAINS.IMA, side B)";

            text.AppendLine(
                $"leg {_game.Leg + 1,2}/{TravelGame.Trail.Length}   toward {_game.Scenery.Toward}   " +
                $"{_game.MilesRemaining,5:0.0} miles to go   pace {_game.Pace}   " +
                $"strides this tick {_game.LastStrides}");

            // The Apple II reference is on screen because this mapping is the whole point of the leg table: the
            // scenery is a miniature of the landmark ahead, and that is easiest to believe while watching it.
            text.AppendLine(
                $"{terrain}   ground {_game.Weather}   " +
                $"ahead: {_game.Scenery.Name} ({_game.Scenery.Apple})   " +
                $"world offset {_game.SceneryX,3}/{TravelGame.SceneryRestX}");

            text.AppendLine(_game.Wagon switch
            {
                TravelWagonEnum.Broken => "*** BROKEN WAGON *** — travelox frame 4. Nothing moves. B to cycle.",
                TravelWagonEnum.Burning => "*** WAGON FIRE *** — travelox frame 5. Nothing moves. B to cycle.",
                _ => $"walk frame {_game.WalkFrame}/3 — advanced once per stride, not once per tick, " +
                     "so the legs cycle faster the harder you drive."
            });

            text.AppendLine(Footer(
                $"T terrain   W weather   P pace   B wagon   N next leg   E sky ({_icon?.ToString() ?? "none"})   " +
                "R restart"));

            // Compose to pixels first so the panel can be drawn over the finished frame, then resample once.
            var frame = _scene.Compose();
            DrawStatusPanel(frame);
            text.Append(AnsiImage.FromPixels(frame).ToAnsi(PictureOptions()));
            return text.ToString();
        }

        /// <summary>
        ///     Steps through the event pictures and back to none, so every one can be seen in place. Cycling art is
        ///     all this does — no event fires, nothing is lost and no time passes.
        /// </summary>
        private void CycleEventIcon()
        {
            var icons = Enum.GetValues<EventIconEnum>();
            var next = _icon is null ? 0 : Array.IndexOf(icons, _icon.Value) + 1;
            _icon = next >= icons.Length ? null : icons[next];
        }

        /// <summary>Seats the sprites on the ground line and points the wagon at the right frame.</summary>
        private void SyncSprites()
        {
            var backdrop = $"{_game.Terrain}/{_game.Weather}";
            if (backdrop != _backdropKey)
                RebuildScene();

            _wagon.Image = Art.Dos("travelox", _game.Wagon switch
            {
                TravelWagonEnum.Broken => BrokenWagon,
                TravelWagonEnum.Burning => BurningWagon,
                _ => _game.WalkFrame
            });

            _scenery.Image = Art.Dos("scenery", _game.Scenery.SpriteId);

            // Everything stands on the ground line rather than at a hard-coded y, which is what keeps the composition
            // together when a frame changes height — the broken wagon is 3px taller than the walking one.
            _wagon.X = TravelGame.WagonX;
            _wagon.Y = TravelGame.GroundY - _wagon.Image.Height;

            _scenery.X = _game.SceneryX - _scenery.Image.Width;
            _scenery.Y = TravelGame.GroundY - _scenery.Image.Height;

            _eventIcon.Visible = _icon.HasValue;
            if (!_icon.HasValue)
                return;

            _eventIcon.Image = DosFrames.EventIcon(_icon.Value);
            var (x, y) = DosFrames.EventIconSpot(_icon.Value);
            _eventIcon.X = x * TravelGame.ScreenWidth / Art.Apple2Width;

            // Anything on the ground is seated on the ground line, exactly as the scenery and the team are, rather
            // than positioned by a y meant for a differently sized 1985 sprite.
            _eventIcon.Y = DosFrames.EventIconStandsOnGround(_icon.Value)
                ? TravelGame.GroundY - _eventIcon.Image.Height
                : y * TravelGame.ScreenHeight / Art.Apple2Height;
        }

        /// <summary>
        ///     Rebuilds the backdrop for the current terrain and weather, and rehangs the sprites on it. Draw order is
        ///     list order: the scenery is behind the team, so the team passes in front of a fort rather than through it.
        /// </summary>
        private void RebuildScene()
        {
            _backdropKey = $"{_game.Terrain}/{_game.Weather}";
            _scene = new SpriteScene(TravelArt.BuildBackdrop(_game.Terrain, _game.Weather));
            _scene.Sprites.Add(_scenery);
            _scene.Sprites.Add(_wagon);

            // Last, so an event picture sits over the horizon strip rather than behind it — the original blits it
            // after the scene is drawn, for the same reason.
            _scene.Sprites.Add(_eventIcon);
        }

        /// <summary>
        ///     Draws the prompt and the status panel over a composed frame, feeding TravelArt the workbench's
        ///     stand-in readings — the game's drive scene feeds the same rows from the real simulation.
        /// </summary>
        private void DrawStatusPanel(PixelBuffer frame)
        {
            TravelArt.DrawPrompt(frame, "Press ENTER to size up the situation");
            TravelArt.DrawStatusPanel(frame, new (string Label, string Value)[]
            {
                ("Date:", _game.Date.ToString("MMMM d, yyyy")),
                ("Weather:", _game.WeatherWord),
                ("Health:", _game.Health),
                ("Food:", $"{_game.Food} pounds"),
                ("Next landmark:", $"{Math.Ceiling(_game.MilesRemaining):0} miles"),
                ("Miles traveled:", $"{_game.MilesTravelled:0} miles")
            });
        }
    }
}
