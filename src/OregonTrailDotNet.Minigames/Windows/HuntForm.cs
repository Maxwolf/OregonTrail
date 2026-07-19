using System.Text;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     The hunt, drawn with the 1990 DOS port's sprites on a 320x200 field.
    ///     <para>
    ///         Neither the hunter nor the animals need flipping at draw time here, unlike the Apple II set: the DOS
    ///         sheets ship the mirrored strips as real pixels, so a facing is just a different frame id.
    ///     </para>
    /// </summary>
    [ParentWindow(typeof(MinigamesWindow))]
    public sealed class HuntForm : SceneForm
    {
        /// <summary>The ring of keys around L, in compass order from North — the original's expert aiming.</summary>
        private static readonly ConsoleKey[] ExpertKeys =
        [
            ConsoleKey.O, ConsoleKey.P, ConsoleKey.Oem1, ConsoleKey.Oem2,
            ConsoleKey.OemPeriod, ConsoleKey.OemComma, ConsoleKey.K, ConsoleKey.I
        ];

        /// <summary>
        ///     Our compass (0=N, clockwise) onto the hunter sheet's groups of three. The sheet stores each drawn strip
        ///     immediately followed by its mirror, so its groups run N, NE, NW, E, W, SE, SW, S rather than clockwise.
        /// </summary>
        private static readonly int[] AimToGroup = [0, 1, 3, 5, 7, 6, 4, 2];

        /// <summary>
        ///     Our species order is the Apple II's table at <c>$E068</c>; the DOS sheet's rows are bison, bear, antlered
        ///     deer, doe, rabbit, squirrel. This maps one onto the other.
        /// </summary>
        private static readonly int[] SpeciesToRow = [2, 1, 0, 3, 4, 5];

        private HuntGame _game = null!;
        private HuntLandscape _landscape = null!;

        /// <summary>`ZO`, which on the trail is <c>(LM&gt;2)+(LM&gt;5)+(LM&gt;10)+(LM&gt;13)</c>. No trail here, so it is a key.</summary>
        private int _zone;
        private SpriteScene _scene = null!;
        private Sprite _hunter = null!;
        private Sprite _bullet = null!;
        private Sprite[] _animals = null!;
        private readonly List<Sprite> _carcasses = [];

        /// <summary>Initializes a new instance of the <see cref="HuntForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public HuntForm(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override int ReservedRows => 10;

        /// <inheritdoc />
        protected override void Build()
        {
            _game = new HuntGame();
            _scene = new SpriteScene(NewGround());

            _animals =
            [
                new Sprite(Assets.Dos("animals", 2)) { Visible = false },
                new Sprite(Assets.Dos("animals", 2)) { Visible = false }
            ];
            foreach (var animal in _animals)
                _scene.Sprites.Add(animal);

            _bullet = new Sprite(BuildBullet()) { Visible = false };
            _scene.Sprites.Add(_bullet);

            _hunter = new Sprite(Assets.Dos("hunter", 1));
            _scene.Sprites.Add(_hunter);

            SyncSprites();
        }

        /// <inheritdoc />
        protected override void Advance()
        {
            _game.Step();
            SyncSprites();
        }

        /// <inheritdoc />
        protected override void OnSectionKey(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    _game.Nudge(-1);
                    return;
                case ConsoleKey.RightArrow:
                    _game.Nudge(1);
                    return;
                case ConsoleKey.UpArrow:
                    _game.AimAt(0);
                    return;
                case ConsoleKey.DownArrow:
                    _game.AimAt(4);
                    return;
                case ConsoleKey.Spacebar:
                    _game.Fire();
                    return;
                case ConsoleKey.Enter:
                    _game.Walking = !_game.Walking;
                    return;
                case ConsoleKey.R:
                    // A new hunt is new country, so the ground is rescattered with it.
                    _game.Reset();
                    ClearCarcassSprites();
                    RebuildScene();
                    SyncSprites();
                    Invalidate();
                    return;
                case ConsoleKey.L:
                    // Just the ground, leaving the hunt running — for judging the scatter without replaying.
                    RebuildScene();
                    SyncSprites();
                    Invalidate();
                    return;
                case ConsoleKey.Z:
                    // Walk the trail's five climate zones, which is the whole of what varies the country.
                    _zone = (_zone + 1) % HuntLandscape.ZoneScenery.Length;
                    RebuildScene();
                    SyncSprites();
                    Invalidate();
                    return;
            }

            var expert = Array.IndexOf(ExpertKeys, key);
            if (expert >= 0)
                _game.AimAt(expert);
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var compass = new[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
            var text = new StringBuilder();
            text.AppendLine();
            text.AppendLine("HUNTING — the rifle turns one step per three ticks, the shorter way round.");
            text.AppendLine(
                $"tick {_game.Tick,4}/{HuntGame.TimeLimit}   aim {compass[_game.Aim],-2} -> {compass[_game.TargetAim],-2}   " +
                $"bullets {_game.Bullets,2}   carried {_game.Pounds,3} lb   " +
                $"{(_game.Walking ? "walking" : "still")}   carcasses {_game.Carcasses.Count}");
            text.AppendLine(_game.Finished ? "*** OUT OF TIME ***  R to hunt again." : _game.LastEvent);
            text.AppendLine(
                $"ground: ZO {_landscape.Zone} {_landscape.ZoneName}   " +
                $"{_landscape.Props.Count} pieces (RND 0-3 +4)   seed {_landscape.Seed}");
            text.AppendLine(Footer(
                "ARROWS turn/snap   I O P ; / . , K aim   SPACE fire   ENTER walk   Z zone   L ground   R restart"));
            text.Append(_scene.ToAnsi(PictureOptions()));
            return text.ToString();
        }

        private void SyncSprites()
        {
            var frame = HuntGame.WalkCycle[_game.Walking ? _game.WalkPhase : 0];
            _hunter.Image = Assets.Dos("hunter", AimToGroup[_game.Aim] * 3 + frame + 1);
            _hunter.X = _game.HunterX;
            _hunter.Y = _game.HunterY;

            for (var i = 0; i < _animals.Length; i++)
            {
                var animal = _game.Animals[i];
                _animals[i].Visible = animal.Active;
                if (!animal.Active)
                    continue;

                _animals[i].Image = Assets.Dos("animals", AnimalFrame(animal.Species, animal.Frame, animal.Facing));
                _animals[i].X = animal.X;
                _animals[i].Y = animal.Y;
            }

            // Carcasses accumulate, so each gets a sprite as it appears — underneath everything that still moves.
            while (_carcasses.Count < _game.Carcasses.Count)
            {
                var carcass = _game.Carcasses[_carcasses.Count];
                var sprite = new Sprite(Assets.Dos("animals", DeadFrame(carcass.Species)), carcass.X, carcass.Y);
                _carcasses.Add(sprite);
                _scene.Sprites.Insert(0, sprite);
            }

            _bullet.Visible = _game.Shot.Active;
            _bullet.X = _game.Shot.X;
            _bullet.Y = _game.Shot.Y;
        }

        /// <summary>
        ///     A walking frame. Each sheet row is <c>[dead, walk1..3, mirrored walk3..1, mirrored dead]</c>, so walking
        ///     the other way is the mirrored half read backwards, which keeps the cycle in step.
        /// </summary>
        private static int AnimalFrame(int species, int frame, int facing)
        {
            var column = facing < 0 ? 6 - frame : 1 + frame;
            return SpeciesToRow[species] * 8 + column + 1;
        }

        private static int DeadFrame(int species) => SpeciesToRow[species] * 8 + 1;

        private void ClearCarcassSprites()
        {
            foreach (var sprite in _carcasses)
                _scene.Sprites.Remove(sprite);

            _carcasses.Clear();
        }

        /// <summary>
        ///     Swaps in freshly scattered ground. The scene's background cannot be replaced in place, so the sprites
        ///     are lifted off and rehung on a new one — carried across in their existing order, which is their draw
        ///     order, so carcasses stay behind the animals that are still on their feet.
        /// </summary>
        private void RebuildScene()
        {
            var standing = _scene.Sprites.ToList();
            _scene = new SpriteScene(NewGround());
            foreach (var sprite in standing)
                _scene.Sprites.Add(sprite);
        }

        /// <summary>
        ///     Scatters a fresh hunting ground and paints it. The seed is taken from the clock rather than fixed, so
        ///     every hunt looks different; it is reported on screen so a ground worth keeping can be asked for again.
        /// </summary>
        private PixelBuffer NewGround()
        {
            _landscape = HuntLandscape.Generate(Environment.TickCount, _zone,
                HuntGame.FieldWidth, HuntGame.FieldHeight,
                HuntGame.FieldWidth / 2, HuntGame.FieldHeight / 2);

            return BuildField();
        }

        /// <summary>Grass with the scenery painted straight in, since none of it ever moves.</summary>
        private PixelBuffer BuildField()
        {
            var field = new PixelBuffer(HuntGame.FieldWidth, HuntGame.FieldHeight);
            var ground = new Rgba32(4, 156, 0, 255);
            for (var y = 0; y < field.Height; y++)
            for (var x = 0; x < field.Width; x++)
                field.SetPixel(x, y, ground);

            foreach (var prop in _landscape.Props)
                field.DrawImage(Assets.Dos("terrain", prop.SpriteId), prop.X, prop.Y);

            return field;
        }

        private static PixelBuffer BuildBullet()
        {
            var bullet = new PixelBuffer(2, 2);
            var white = new Rgba32(252, 252, 252, 255);
            for (var y = 0; y < 2; y++)
            for (var x = 0; x < 2; x++)
                bullet.SetPixel(x, y, white);

            return bullet;
        }
    }
}
