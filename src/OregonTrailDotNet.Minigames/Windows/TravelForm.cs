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
    public sealed class TravelForm : SceneForm
    {
        // travelox.png, cut in reading order: the three walk frames first, then the two the loss events swap in.
        private const int FirstWalkFrame = 1;
        private const int BrokenWagon = 4;
        private const int BurningWagon = 5;

        // scenery.png's two full-width horizon strips.
        private const int PlainsStrip = 1;
        private const int MountainStrip = 2;

        private TravelGame _game = null!;
        private SpriteScene _scene = null!;
        private Sprite _scenery = null!;
        private Sprite _wagon = null!;
        private string _backdropKey = string.Empty;

        /// <summary>Initializes a new instance of the <see cref="TravelForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public TravelForm(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override int ReservedRows => 9;

        /// <inheritdoc />
        protected override void Build()
        {
            _game = new TravelGame();

            _scenery = new Sprite(Assets.Dos("scenery", _game.Scenery.SpriteId));
            _wagon = new Sprite(Assets.Dos("travelox", FirstWalkFrame));

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
                $"leg {_game.Leg,2}   {_game.MilesRemaining,5:0.0} miles to go   " +
                $"{_game.MilesTravelled,6:0} travelled   pace {_game.Pace}   " +
                $"strides this tick {_game.LastStrides}");

            text.AppendLine(
                $"{terrain}   ground {_game.Weather}   " +
                $"ahead: {_game.Scenery.Name}   world offset {_game.SceneryX,3}/{TravelGame.SceneryRestX}");

            text.AppendLine(_game.Wagon switch
            {
                TravelWagonEnum.Broken => "*** BROKEN WAGON *** — travelox frame 4. Nothing moves. B to cycle.",
                TravelWagonEnum.Burning => "*** WAGON FIRE *** — travelox frame 5. Nothing moves. B to cycle.",
                _ => $"walk frame {_game.WalkFrame}/3 — advanced once per stride, not once per tick, " +
                     "so the legs cycle faster the harder you drive."
            });

            text.AppendLine(Footer("T terrain   W weather   P pace   B wagon   N next leg   R restart"));
            text.Append(_scene.ToAnsi(PictureOptions()));
            return text.ToString();
        }

        /// <summary>Seats the sprites on the ground line and points the wagon at the right frame.</summary>
        private void SyncSprites()
        {
            var backdrop = $"{_game.Terrain}/{_game.Weather}";
            if (backdrop != _backdropKey)
                RebuildScene();

            _wagon.Image = Assets.Dos("travelox", _game.Wagon switch
            {
                TravelWagonEnum.Broken => BrokenWagon,
                TravelWagonEnum.Burning => BurningWagon,
                _ => _game.WalkFrame
            });

            _scenery.Image = Assets.Dos("scenery", _game.Scenery.SpriteId);

            // Everything stands on the ground line rather than at a hard-coded y, which is what keeps the composition
            // together when a frame changes height — the broken wagon is 3px taller than the walking one.
            _wagon.X = TravelGame.WagonX;
            _wagon.Y = TravelGame.GroundY - _wagon.Image.Height;

            _scenery.X = _game.SceneryX - _scenery.Image.Width;
            _scenery.Y = TravelGame.GroundY - _scenery.Image.Height;
        }

        /// <summary>
        ///     Rebuilds the backdrop for the current terrain and weather, and rehangs the sprites on it. Draw order is
        ///     list order: the scenery is behind the team, so the team passes in front of a fort rather than through it.
        /// </summary>
        private void RebuildScene()
        {
            _backdropKey = $"{_game.Terrain}/{_game.Weather}";
            _scene = new SpriteScene(BuildBackdrop());
            _scene.Sprites.Add(_scenery);
            _scene.Sprites.Add(_wagon);
        }

        /// <summary>
        ///     Sky, ground, horizon. The ground colour is the whole of what weather does to this screen, and it is one
        ///     line in the original (`:310`):
        ///     <code>CC = 3*(AS&gt;=1) + (AS&lt;1)*(5*(AR&lt;=.2) + (AR&gt;.2))</code>
        ///     Colour 3 is white, 5 orange, 1 green — snow if there is any lying (which before April there always is),
        ///     otherwise arid if the rain accumulator has stayed low, otherwise green. It is a flood fill, not a
        ///     picture, which is why the whole country can change season for the cost of one <c>&amp; BOX</c>.
        /// </summary>
        private PixelBuffer BuildBackdrop()
        {
            var backdrop = new PixelBuffer(TravelGame.ScreenWidth, TravelGame.ScreenHeight);

            var sky = _game.Weather == TravelWeatherEnum.Snow
                ? new Rgba32(168, 192, 216, 255)     // overcast, so white ground still reads against it
                : new Rgba32(84, 168, 252, 255);

            var ground = _game.Weather switch
            {
                TravelWeatherEnum.Snow => new Rgba32(252, 252, 252, 255),      // colour 3
                TravelWeatherEnum.Arid => new Rgba32(216, 132, 60, 255),       // colour 5
                _ => new Rgba32(4, 156, 0, 255)                                // colour 1
            };

            for (var y = 0; y < backdrop.Height; y++)
            {
                var colour = y < TravelGame.GroundY ? sky : ground;
                for (var x = 0; x < backdrop.Width; x++)
                    backdrop.SetPixel(x, y, colour);
            }

            // The strip is exactly as wide as the screen, and hangs from the ground line so the hills rise out of the
            // horizon instead of floating above it.
            var strip = Assets.Dos("scenery", _game.Terrain == TravelTerrainEnum.Plains ? PlainsStrip : MountainStrip);
            backdrop.DrawImage(strip, 0, TravelGame.GroundY - strip.Height);

            return backdrop;
        }
    }
}
