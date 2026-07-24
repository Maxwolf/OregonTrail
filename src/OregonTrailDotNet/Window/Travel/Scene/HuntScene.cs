using System;
using System.Collections.Generic;
using System.Text;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The hunt as the 1985 original played it: a real-time field drawn with the DOS port's sprites, the rifle
    ///     swinging a step at a time the shorter way round, one bullet in the air, animals whose species follow the
    ///     country the party is actually standing in, and scenery from the trail's own climate zone. The graphical
    ///     sibling of the word-typing <see cref="Hunt.Hunting" /> — which remains the headless implementation — fed
    ///     by the real simulation: the rifle carries the wagon's actual ammunition, and the bag goes back through
    ///     <see cref="HuntSceneResult" /> into the wagon's stores.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class HuntScene : SceneForm<TravelInfo>
    {
        /// <summary>The Apple II's expert aiming — the ring of keys around L, in compass order (0 = N, clockwise).</summary>
        private static readonly ConsoleKey[] ExpertKeys =
        [
            ConsoleKey.O, ConsoleKey.P, ConsoleKey.Oem1, ConsoleKey.Oem2,
            ConsoleKey.OemPeriod, ConsoleKey.OemComma, ConsoleKey.K, ConsoleKey.I
        ];

        /// <summary>The DOS port's expert aiming — the keypad as the compass it looks like, plus the top row.</summary>
        private static readonly Dictionary<ConsoleKey, int> KeypadAim = new()
        {
            [ConsoleKey.NumPad8] = 0, [ConsoleKey.NumPad9] = 1, [ConsoleKey.NumPad6] = 2,
            [ConsoleKey.NumPad3] = 3, [ConsoleKey.NumPad2] = 4, [ConsoleKey.NumPad1] = 5,
            [ConsoleKey.NumPad4] = 6, [ConsoleKey.NumPad7] = 7,
            [ConsoleKey.D8] = 0, [ConsoleKey.D9] = 1, [ConsoleKey.D6] = 2, [ConsoleKey.D3] = 3,
            [ConsoleKey.D2] = 4, [ConsoleKey.D1] = 5, [ConsoleKey.D4] = 6, [ConsoleKey.D7] = 7
        };

        private HuntGame _game;
        private SpriteScene _scene;
        private Sprite _hunter;
        private Sprite _bullet;
        private Sprite[] _animals;
        private readonly List<Sprite> _carcasses = new();

        /// <summary>Initializes a new instance of the <see cref="HuntScene" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public HuntScene(IWindow window) : base(window)
        {
        }

        /// <summary>Only the game's two status lines and the frame's newline sit outside the picture.</summary>
        protected override int ReservedRows => 4;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <inheritdoc />
        protected override void Build()
        {
            var game = GameSimulationApp.Instance;

            // The original call: bullets from the wagon, the roster flags from the landmark, the climate zone for
            // the ground. An unknown location (fail-soft) hunts the full roster in the middle country.
            var stop = OriginalTrail.ForLocation(game.Trail.CurrentLocation?.Name);
            var lm = stop?.Index ?? 8;
            var bullets = game.Vehicle.Inventory[EntitiesEnum.Ammo].Quantity;

            _game = new HuntGame(null, bullets,
                OriginalTrail.AntleredDeerAt(lm), OriginalTrail.BearAt(lm), OriginalTrail.BisonAt(lm));

            var landscape = HuntLandscape.Generate(Environment.TickCount, OriginalTrail.ClimateZone(lm),
                HuntGame.FieldWidth, HuntGame.FieldHeight,
                HuntGame.FieldWidth / 2, HuntGame.FieldHeight / 2);
            _game.Obstacles = landscape.Obstacles;

            _scene = new SpriteScene(BuildField(landscape));

            _animals =
            [
                new Sprite(Art.Dos("animals", 2)) { Visible = false },
                new Sprite(Art.Dos("animals", 2)) { Visible = false }
            ];
            foreach (var animal in _animals)
                _scene.Sprites.Add(animal);

            _bullet = new Sprite(BuildBullet()) { Visible = false };
            _scene.Sprites.Add(_bullet);

            _hunter = new Sprite(Art.Dos("hunter", 1));
            _scene.Sprites.Add(_hunter);

            SyncSprites();
        }

        /// <inheritdoc />
        protected override void Advance()
        {
            _game.Step();
            SyncSprites();

            // Out of time: the bag goes back to the wagon through the result screen.
            if (_game.Finished)
                GoToResult();
        }

        /// <summary>The shift tiebreak: unshifted , and . aim absolutely (Apple II), shifted &lt; &gt; rotate (DOS).</summary>
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
                    Fire();
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
        ///     RETURN toggles walking — bound here because the framework consumes ENTER as buffer control and it
        ///     never arrives as a key press; the original's own binding is the Return key.
        /// </summary>
        /// <param name="input">Ignored; the hunt reads keys, not lines.</param>
        public override void OnInputBufferReturned(string input)
        {
            _game.Walking = !_game.Walking;
        }

        /// <summary>ESC ends the hunt early and keeps the bag — the parity of the text hunt's stop words.</summary>
        protected override void OnEscape() => GoToResult();

        /// <summary>
        ///     Fires, with the DOS port's 10 ms muzzle pop when a round actually leaves — pitched at the muzzle's
        ///     screen row plus 50 Hz, so a shot fired high in the field thuds and one fired low cracks
        ///     (docs/legacy-sounds.md §1.2). A blocked trigger — bullet already out, ammunition gone — is silent,
        ///     and so are hits, misses and kills, exactly as the original left them.
        /// </summary>
        private void Fire()
        {
            var before = _game.Bullets;
            _game.Fire();

            if (_game.Bullets < before)
                Sfx.Gunshot(_game.Shot.Y + 50);
        }

        /// <summary>
        ///     The whole output is the field itself — no heads-up text at all, exactly as the original played it:
        ///     the hunter, the animals, and the country, and the tally waits for the result screen.
        /// </summary>
        protected override string Compose() => _scene.ToAnsi(PictureOptions());

        /// <summary>Hands the bag to the result screen, which applies the wrapper and charges the day.</summary>
        private void GoToResult()
        {
            UserData.HuntOutcome = new HuntOutcome(_game.Pounds, _game.ShotsFired, _game.Kills);
            SetForm(typeof(HuntSceneResult));
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

        /// <summary>The field: black ground (both ports) with the zone's scenery painted straight in.</summary>
        private static PixelBuffer BuildField(HuntLandscape landscape)
        {
            var field = new PixelBuffer(HuntGame.FieldWidth, HuntGame.FieldHeight);
            for (var y = 0; y < field.Height; y++)
                for (var x = 0; x < field.Width; x++)
                    field.SetPixel(x, y, Palette.Black);

            foreach (var prop in landscape.Props)
                field.DrawImage(Art.Dos("terrain", prop.SpriteId), prop.X, prop.Y);

            return field;
        }

        private static PixelBuffer BuildBullet()
        {
            var bullet = new PixelBuffer(2, 2);
            for (var y = 0; y < 2; y++)
                for (var x = 0; x < 2; x++)
                    bullet.SetPixel(x, y, Palette.White);

            return bullet;
        }
    }

    /// <summary>What a hunt brought back, handed from the scene to its result screen.</summary>
    /// <param name="RawPounds">Raw pounds shot, before the wrapper dresses and caps them.</param>
    /// <param name="ShotsFired">Trigger pulls — one round of the wagon's ammunition each.</param>
    /// <param name="Kills">Animals brought down, which feeds the Shoshoni guide's kill count.</param>
    public sealed record HuntOutcome(int RawPounds, int ShotsFired, int Kills);
}
