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
        /// <summary>
        ///     The Apple II's expert aiming, verified against its own key table at <c>$ED1E</c>:
        ///     <c>3B 50 4F 49 4B 2C 2E 2F</c> = <c>; P O I K , . /</c> — the ring of keys around <b>L</b>, which the
        ///     handler at <c>$ECC8</c> searches for the pressed key and uses the table index as an absolute heading.
        ///     Listed here in our compass order (0 = N, clockwise).
        /// </summary>
        private static readonly ConsoleKey[] ExpertKeys =
        [
            ConsoleKey.O, ConsoleKey.P, ConsoleKey.Oem1, ConsoleKey.Oem2,
            ConsoleKey.OemPeriod, ConsoleKey.OemComma, ConsoleKey.K, ConsoleKey.I
        ];

        /// <summary>
        ///     The <b>1990 DOS port's</b> expert aiming: the numeric keypad read as the compass it already looks like.
        ///     The Apple II never used it — its ring is <see cref="ExpertKeys" /> — but both ports keep every aiming
        ///     scheme live at once rather than switching modes, so adding this one alongside is in that spirit.
        ///     <para>
        ///         The top-row digits are mapped to the same headings purely as a convenience for keyboards with no
        ///         separate keypad; that part is ours, not either port's.
        ///     </para>
        /// </summary>
        private static readonly Dictionary<ConsoleKey, int> KeypadAim = new()
        {
            [ConsoleKey.NumPad8] = 0, [ConsoleKey.NumPad9] = 1, [ConsoleKey.NumPad6] = 2,
            [ConsoleKey.NumPad3] = 3, [ConsoleKey.NumPad2] = 4, [ConsoleKey.NumPad1] = 5,
            [ConsoleKey.NumPad4] = 6, [ConsoleKey.NumPad7] = 7,
            [ConsoleKey.D8] = 0, [ConsoleKey.D9] = 1, [ConsoleKey.D6] = 2, [ConsoleKey.D3] = 3,
            [ConsoleKey.D2] = 4, [ConsoleKey.D1] = 5, [ConsoleKey.D4] = 6, [ConsoleKey.D7] = 7
        };

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

        /// <summary>
        ///     The 1990 DOS port's <b>novice</b> aiming, which rotates one step per press. Its instruction screen
        ///     draws these as two keycaps — <c>&lt;</c> printed above <c>,</c>, and <c>&gt;</c> above <c>.</c> — so
        ///     they are the comma and period <i>keys</i>, exactly the two the Apple II's expert ring already uses for
        ///     absolute SW and S. The two ports collide on the same physical keys.
        ///     <para>
        ///         Since this workbench runs both schemes at once, the collision is resolved by shift: <b>unshifted
        ///         <c>,</c> and <c>.</c> aim absolutely</b> (Apple II), <b>shifted <c>&lt;</c> and <c>&gt;</c> rotate a
        ///         step</b> (DOS). That reconciliation is ours — neither port had to make it — and it needs
        ///         <c>KeyChar</c>, because <c>ConsoleKey.OemComma</c> is reported for both.
        ///     </para>
        /// </summary>
        /// <param name="keyInfo">The key exactly as the console reported it.</param>
        protected override void OnSectionKey(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.KeyChar)
            {
                case '<':
                    _game.Nudge(-1);
                    return;
                case '>':
                    _game.Nudge(1);
                    return;
                default:
                    base.OnSectionKey(keyInfo);
                    return;
            }
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
                case ConsoleKey.NumPad5:
                case ConsoleKey.D5:
                    // SPACE is the original's ($EC6B). The keypad's centre is ours, to keep a hand on the pad.
                    _game.Fire();
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
            {
                _game.AimAt(expert);
                return;
            }

            if (KeypadAim.TryGetValue(key, out var heading))
                _game.AimAt(heading);
        }

        /// <summary>
        ///     Walking is toggled here rather than from <see cref="OnSectionKey" /> because <b>ENTER never arrives as
        ///     a key press</b>: WolfCurses' <c>InputManager.SendConsoleKey</c> treats it as "submit the buffer" and
        ///     returns without calling <c>SendKeyPress</c>, so a <c>case ConsoleKey.Enter</c> in the key handler is
        ///     dead code — which is exactly what it was, and why walking did nothing.
        ///     <para>
        ///         RETURN is the original's own binding for it: <c>HUNT.LIB</c>'s instruction screen prints "Return
        ///         Key" against "To start or stop walking", and the handler at <c>$EC7F</c> flips bit 7 of
        ///         <c>$EE08</c> on <c>$0D</c>.
        ///     </para>
        /// </summary>
        /// <param name="input">Ignored; this section reads keys, not lines.</param>
        public override void OnInputBufferReturned(string input)
        {
            _game.Walking = !_game.Walking;
            Invalidate();
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
                "ARROWS or < > turn   I O P ; / . , K or NUMPAD aim   SPACE/5 fire   ENTER walk   " +
                "Z zone   L ground   R restart"));
            text.Append(_scene.ToAnsi(PictureOptions()));
            return text.ToString();
        }

        private void SyncSprites()
        {
            var frame = HuntGame.WalkCycle[_game.Walking ? _game.WalkPhase : 0];
            _hunter.Image = DosFrames.Hunter(_game.Aim, frame);
            _hunter.X = _game.HunterX;
            _hunter.Y = _game.HunterY;

            for (var i = 0; i < _animals.Length; i++)
            {
                var animal = _game.Animals[i];
                _animals[i].Visible = animal.Active;
                if (!animal.Active)
                    continue;

                _animals[i].Image = DosFrames.Animal(animal.Species, animal.Frame, animal.Facing);
                _animals[i].X = animal.X;
                _animals[i].Y = animal.Y;
            }

            // Carcasses accumulate, so each gets a sprite as it appears — underneath everything that still moves.
            while (_carcasses.Count < _game.Carcasses.Count)
            {
                var carcass = _game.Carcasses[_carcasses.Count];
                var sprite = new Sprite(DosFrames.DeadAnimal(carcass.Species), carcass.X, carcass.Y);
                _carcasses.Add(sprite);
                _scene.Sprites.Insert(0, sprite);
            }

            _bullet.Visible = _game.Shot.Active;
            _bullet.X = _game.Shot.X;
            _bullet.Y = _game.Shot.Y;
        }



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

        /// <summary>
        ///     The field, with the scenery painted straight in since none of it ever moves.
        ///     <para>
        ///         <b>The ground is black</b>, in both ports — there is no grass. This port drew it green for a while,
        ///         which was invention: the Apple II OR-blits its scenery onto a cleared hi-res screen (which is why
        ///         <c>apple2_hunt.py</c> decodes black as transparent), and a capture of the DOS hunt shows the same
        ///         thing, scenery and hunter standing on plain black.
        ///     </para>
        /// </summary>
        private PixelBuffer BuildField()
        {
            var field = new PixelBuffer(HuntGame.FieldWidth, HuntGame.FieldHeight);
            var ground = Palette.Black;
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
            var white = Palette.White;
            for (var y = 0; y < 2; y++)
            for (var x = 0; x < 2; x++)
                bullet.SetPixel(x, y, white);

            return bullet;
        }
    }
}
