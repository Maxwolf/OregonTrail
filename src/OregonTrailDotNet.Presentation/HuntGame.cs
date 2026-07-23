namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     The hunt, rebuilt from the disassembled <c>&amp; HUNT</c> machine code and its BASIC wrapper (see
    ///     <c>docs/minigames.md</c>). Pure logic, no drawing.
    ///     <para>
    ///         Aim is eight compass points and rotation takes <b>time proportional to angular distance</b> — one step
    ///         per three ticks, the shorter way round — rather than snapping. Both ports do this, five years apart, so
    ///         it is a deliberate feel decision and the single most important thing to get right here.
    ///     </para>
    /// </summary>
    public sealed class HuntGame
    {
        /// <summary>The DOS field the hunt is drawn on, in MCGA pixels.</summary>
        public const int FieldWidth = 320;

        /// <summary>Field height; see <see cref="FieldWidth" />.</summary>
        public const int FieldHeight = 200;

        /// <summary>Widest the hunter gets, used to keep him on the field.</summary>
        public const int HunterWidth = 21;

        /// <summary>Tallest the hunter gets.</summary>
        public const int HunterHeight = 27;

        /// <summary>`$E6E4` — main-loop iterations a hunt lasts.</summary>
        public const int TimeLimit = 2500;

        /// <summary>`$E0D6` — animals alive on the field at once.</summary>
        public const int AnimalSlots = 2;

        /// <summary>`$E156` — this many carcasses on the ground blocks all further spawns.</summary>
        public const int CarcassSpawnBlock = 4;

        /// <summary>`$E15A` — spawn chance per free slot per tick. The shipped value rests on a byte-order bug.</summary>
        public const double SpawnChance = 0.008;

        /// <summary>`$EC21` — the key handler is polled every third tick, which is what paces rotation.</summary>
        public const int TicksPerRotationStep = 3;

        /// <summary>The wrapper's carry cap: 100 lb back to the wagon, however much was shot.</summary>
        public const int CarryCap = 100;

        /// <summary>
        ///     Aim vectors, indexed clockwise from North. The 2:1 ratio is the original's: a bullet steps twice these,
        ///     which is exactly the documented dx = ±4, dy = ±2.
        /// </summary>
        public static readonly (int X, int Y)[] AimVectors =
        [
            (0, -1), (2, -1), (2, 0), (2, 1), (0, 1), (-2, 1), (-2, 0), (-2, -1)
        ];

        /// <summary>
        ///     Which stored hunter pose draws each aim, and whether it is mirrored. Only five poses exist — all facing
        ///     left or away — and East/NE/SE are the mirror of West/NW/SW, via bit 7 of the pose byte.
        /// </summary>
        public static readonly (int Pose, bool Mirror)[] AimPoses =
        [
            (0, false), // N
            (1, true),  // NE  = mirrored NW
            (2, true),  // E   = mirrored W
            (3, true),  // SE  = mirrored SW
            (4, false), // S
            (3, false), // SW
            (2, false), // W
            (1, false)  // NW
        ];

        /// <summary>Walk cycle over each pose's three stored frames.</summary>
        public static readonly int[] WalkCycle = [0, 1, 0, 2];

        /// <summary>The six species, in the order the table at <c>$E068</c> stores them.</summary>
        public static readonly SpeciesInfo[] Species =
        [
            new("Antlered deer", 100, 140),
            new("Bear", 200, 400),
            new("Bison", 1700, 2000),
            new("Doe", 120, 150),
            new("Rabbit", 2, 10),
            new("Squirrel", 3, 7)
        ];

        private readonly Random _random;
        private int _rotationTimer;
        private int _walkTimer;

        /// <summary>Initializes a new instance of the <see cref="HuntGame" /> class.</summary>
        /// <param name="seed">Fixed seed for a reproducible run, or null for the clock.</param>
        public HuntGame(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            Animals = [new Animal(), new Animal()];
            Reset();
        }

        /// <summary>Ticks elapsed.</summary>
        public int Tick { get; private set; }

        /// <summary>Where the rifle is actually pointing, 0-7 clockwise from North.</summary>
        public int Aim { get; private set; }

        /// <summary>Where the player has asked it to point; the rifle rotates toward this.</summary>
        public int TargetAim { get; private set; }

        /// <summary>Index into <see cref="WalkCycle" />.</summary>
        public int WalkPhase { get; private set; }

        /// <summary>Whether the hunter is walking (RETURN toggles it).</summary>
        public bool Walking { get; set; }

        /// <summary>Hunter position, native Apple II pixels.</summary>
        public int HunterX { get; private set; }

        /// <summary>Hunter position, native Apple II pixels.</summary>
        public int HunterY { get; private set; }

        /// <summary>The live animal slots.</summary>
        public Animal[] Animals { get; }

        /// <summary>Animals shot and still lying on the field.</summary>
        public List<Carcass> Carcasses { get; } = [];

        /// <summary>The single bullet; the original allows exactly one in flight.</summary>
        public Bullet Shot { get; } = new();

        /// <summary>Rounds left.</summary>
        public int Bullets { get; private set; }

        /// <summary>Meat carried back, after the wrapper's halving and the 100 lb cap.</summary>
        public int Pounds { get; private set; }

        /// <summary>What just happened, for the heads-up display.</summary>
        public string LastEvent { get; private set; } = "Ready.";

        /// <summary>True once the countdown runs out.</summary>
        public bool Finished => Tick >= TimeLimit;

        /// <summary>Puts a fresh hunt on the field.</summary>
        public void Reset()
        {
            Tick = 0;
            Aim = 0;
            TargetAim = 0;
            WalkPhase = 0;
            Walking = false;
            HunterX = FieldWidth / 2;
            HunterY = FieldHeight / 2;
            Bullets = 20;
            Pounds = 0;
            LastEvent = "Ready.";
            Carcasses.Clear();
            Shot.Active = false;

            foreach (var animal in Animals)
                animal.Active = false;
        }

        /// <summary>Rotate the rifle one step, the novice control (the arrow keys).</summary>
        /// <param name="delta">+1 clockwise, -1 counter-clockwise.</param>
        public void Nudge(int delta) => TargetAim = (TargetAim + delta + 8) % 8;

        /// <summary>Aim at a compass point outright, the expert control (the ring of keys around L).</summary>
        /// <param name="direction">0-7 clockwise from North.</param>
        public void AimAt(int direction) => TargetAim = ((direction % 8) + 8) % 8;

        /// <summary>Fire, unless a bullet is already out or the ammunition is gone.</summary>
        public void Fire()
        {
            if (Shot.Active)
            {
                LastEvent = "A bullet is already in flight.";
                return;
            }

            if (Bullets <= 0)
            {
                LastEvent = "Out of bullets.";
                return;
            }

            Bullets--;
            var vector = AimVectors[Aim];
            Shot.X = HunterX + 10;
            Shot.Y = HunterY + 12;
            Shot.DeltaX = vector.X * 2;
            Shot.DeltaY = vector.Y * 2;
            Shot.Active = true;
            LastEvent = "Fired.";
        }

        /// <summary>Advances one tick of the hunt.</summary>
        public void Step()
        {
            if (Finished)
                return;

            Tick++;
            Rotate();
            Walk();
            MoveAnimals();
            MoveBullet();
            Spawn();
        }

        private void Rotate()
        {
            if (Aim == TargetAim)
                return;

            if (++_rotationTimer < TicksPerRotationStep)
                return;

            _rotationTimer = 0;

            // Shorter way round, one step at a time — the latency is the point.
            var difference = ((TargetAim - Aim + 8) % 8);
            Aim = (Aim + (difference <= 4 ? 1 : -1) + 8) % 8;
        }

        private void Walk()
        {
            if (!Walking)
                return;

            if (++_walkTimer >= 4)
            {
                _walkTimer = 0;
                WalkPhase = (WalkPhase + 1) % WalkCycle.Length;
            }

            var vector = AimVectors[Aim];
            HunterX = Math.Clamp(HunterX + vector.X, 0, FieldWidth - 21);
            HunterY = Math.Clamp(HunterY + vector.Y, 0, FieldHeight - 28);
        }

        private void MoveAnimals()
        {
            foreach (var animal in Animals)
            {
                if (!animal.Active)
                    continue;

                animal.X += animal.Facing;
                if (++animal.FrameTimer >= 6)
                {
                    animal.FrameTimer = 0;
                    animal.Frame = (animal.Frame + 1) % 3;
                }

                if (animal.X < -40 || animal.X > FieldWidth + 40)
                    animal.Active = false;
            }
        }

        private void MoveBullet()
        {
            if (!Shot.Active)
                return;

            Shot.X += Shot.DeltaX;
            Shot.Y += Shot.DeltaY;

            if (Shot.X < 0 || Shot.Y < 0 || Shot.X >= FieldWidth || Shot.Y >= FieldHeight)
            {
                Shot.Active = false;
                return;
            }

            foreach (var animal in Animals)
            {
                if (!animal.Active)
                    continue;

                var info = Species[animal.Species];
                if (Shot.X < animal.X || Shot.X >= animal.X + info.Width ||
                    Shot.Y < animal.Y || Shot.Y >= animal.Y + info.Height)
                    continue;

                Shot.Active = false;
                animal.Active = false;
                Carcasses.Add(new Carcass { Species = animal.Species, X = animal.X, Y = animal.Y });

                // The wrapper dresses the kill (halving anything from 3 lb up), then caps what the whole hunt
                // carries home at 100 lb — which is why a bison and a bear come to exactly the same thing.
                var shot = _random.Next(info.MinimumPounds, info.MaximumPounds + 1);
                var dressed = Bag(shot);
                var before = Pounds;
                Pounds = Math.Min(CarryCap, Pounds + dressed);
                var carried = Pounds - before;

                var name = info.Name.ToLowerInvariant();
                LastEvent = carried < dressed
                    ? $"Shot a {name} — {shot} lb, {dressed} lb dressed; carried {carried} lb ({CarryCap} lb cap)."
                    : $"Shot a {name} — {shot} lb, {dressed} lb dressed; carried {carried} lb.";
                return;
            }
        }

        private void Spawn()
        {
            if (Carcasses.Count >= CarcassSpawnBlock)
                return;

            foreach (var animal in Animals)
            {
                if (animal.Active || _random.NextDouble() >= SpawnChance)
                    continue;

                animal.Species = _random.Next(Species.Length);
                var fromLeft = _random.Next(2) == 0;
                animal.Facing = fromLeft ? 1 : -1;
                animal.X = fromLeft ? -30 : FieldWidth + 10;
                animal.Y = _random.Next(20, FieldHeight - 40);
                animal.Frame = 0;
                animal.FrameTimer = 0;
                animal.Active = true;
            }
        }

        /// <summary>
        ///     `HUNT.LIB:50011` — <c>L = INT(L / (2 - (L &lt; 3)))</c>. Applesoft relationals are 1/0, so anything
        ///     from three pounds up is halved and one- and two-pound kills pass through whole.
        /// </summary>
        public static int Bag(int pounds) => pounds < 3 ? pounds : pounds / 2;

        /// <summary>One of the six species in the table at <c>$E068</c>.</summary>
        /// <param name="Name">Inferred from the silhouette; the game holds no animal name strings at all.</param>
        /// <param name="MinimumPounds">Base yield.</param>
        /// <param name="MaximumPounds">Base plus the table's random range.</param>
        public sealed record SpeciesInfo(string Name, int MinimumPounds, int MaximumPounds)
        {
            /// <summary>Sprite width, taken from the extracted frames.</summary>
            public int Width => MinimumPounds >= 100 ? 28 : 21;

            /// <summary>Sprite height, taken from the extracted frames.</summary>
            public int Height => Name switch
            {
                "Antlered deer" => 20,
                "Bear" => 14,
                "Bison" => 17,
                "Doe" => 18,
                "Rabbit" => 10,
                _ => 8
            };
        }

        /// <summary>A live animal on the field.</summary>
        public sealed class Animal
        {
            /// <summary>Species id, 0-5.</summary>
            public int Species { get; set; }

            /// <summary>Left edge.</summary>
            public int X { get; set; }

            /// <summary>Top edge.</summary>
            public int Y { get; set; }

            /// <summary>+1 walking right, -1 walking left.</summary>
            public int Facing { get; set; } = 1;

            /// <summary>Which of the three stored frames is showing.</summary>
            public int Frame { get; set; }

            /// <summary>Ticks since the frame last changed.</summary>
            public int FrameTimer { get; set; }

            /// <summary>Whether the slot is in use.</summary>
            public bool Active { get; set; }
        }

        /// <summary>A shot animal lying where it fell.</summary>
        public sealed class Carcass
        {
            /// <summary>Species id, which also picks the dead pose.</summary>
            public int Species { get; set; }

            /// <summary>Left edge.</summary>
            public int X { get; set; }

            /// <summary>Top edge.</summary>
            public int Y { get; set; }
        }

        /// <summary>The one bullet the original allows in the air.</summary>
        public sealed class Bullet
        {
            /// <summary>Position.</summary>
            public int X { get; set; }

            /// <summary>Position.</summary>
            public int Y { get; set; }

            /// <summary>Velocity, twice the aim vector.</summary>
            public int DeltaX { get; set; }

            /// <summary>Velocity, twice the aim vector.</summary>
            public int DeltaY { get; set; }

            /// <summary>Whether it is in flight.</summary>
            public bool Active { get; set; }
        }
    }
}
