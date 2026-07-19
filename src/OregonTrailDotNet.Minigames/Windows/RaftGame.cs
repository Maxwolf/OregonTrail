namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>How the player is leaning on the tiller this tick.</summary>
    public enum RaftSteerEnum
    {
        /// <summary>No key. The raft keeps whatever drift it already had.</summary>
        None,

        /// <summary>LEFT or UP — the same direction, because the river is drawn on a diagonal.</summary>
        Far,

        /// <summary>RIGHT or DOWN — likewise one direction, toward the near bank and the landing.</summary>
        Near
    }

    /// <summary>How a run ended.</summary>
    public enum RaftOutcomeEnum
    {
        /// <summary>Still on the water.</summary>
        Running,

        /// <summary>Reached the landing inside the 20-tick window.</summary>
        Landed,

        /// <summary>The window closed with the raft still on the river; survivable, costs supplies.</summary>
        Missed
    }

    /// <summary>
    ///     The Columbia River raft, transcribed from <c>legacy/source/v1.1-sideB/FLOAT.txt</c>. Pure logic — it draws
    ///     nothing and knows nothing about WolfCurses, so it can be stepped in a test or a loop as easily as on screen.
    ///     Every constant below is the original's, cited to its line; see <c>docs/minigames.md</c> for the derivation.
    ///     <para>
    ///         <b>A collision does not end the run</b>, here or in the original. <c>:700</c> rolls the party and the
    ///         inventory against the odds below, <c>:800</c> lists what was lost, and the loop carries straight on; the
    ///         raft only breaks up when more than nine things go at once (<c>:730</c>, <c>IF Z &gt; 9</c>) or when the
    ///         last of the party drowns (<c>:760</c>). Both need an inventory and a party the workbench does not have,
    ///         so hits are counted here and the odds carried as text, ready for <c>:700</c> to be restored around them.
    ///     </para>
    ///     <para>
    ///         Making contact fatal was tried and is wrong — and instructively so. A rock's <c>(+8, -4)</c> drift
    ///         preserves the lane coordinate, so it lives its whole life in the lane it spawned in, and <c>:300</c>
    ///         cannot spawn one past lane 15.9. Lane 16 is therefore untouchable... and <c>:1030</c> starts the raft
    ///         there. Fatal collisions turn the game into "press right once, then wait": 4000 runs, 4000 landings, no
    ///         rock ever reached. The original is playable precisely <i>because</i> a hit costs supplies instead of the
    ///         run. See <c>docs/minigames.md</c> for the per-lane measurements.
    ///     </para>
    /// </summary>
    public sealed class RaftGame
    {
        /// <summary>
        ///     The Apple II screen <c>FLOAT</c> was written for. The logic keeps thinking in this space even though the
        ///     art is now the DOS port's, so every constant below stays the number the BASIC actually used; the form
        ///     scales positions into the drawing surface at the last moment.
        /// </summary>
        public const int ScreenWidth = 280;

        /// <summary>Apple II screen height; see <see cref="ScreenWidth" />.</summary>
        public const int ScreenHeight = 192;

        /// <summary>Lanes across the river, <c>HP</c> in the original (`:1030`).</summary>
        public const int FirstLane = 0;

        /// <summary>The near bank and the landing.</summary>
        public const int LastLane = 17;

        /// <summary>`:1030` — the raft starts near the landing bank...</summary>
        public const int StartLane = 16;

        /// <summary>...and is <i>already sliding away from it</i>. This is what makes the opening feel urgent.</summary>
        public const int StartDrift = -1;

        /// <summary>`XI` — rocks travel this far right each tick.</summary>
        public const int RockDriftX = 8;

        /// <summary>`YI` — and this far up (negative is up the screen).</summary>
        public const int RockDriftY = -4;

        /// <summary>`RF` — chance a free rock slot respawns, per slot per tick (`:1070`, `:1075`).</summary>
        public const double RockSpawnChance = 0.15;

        /// <summary>Scenery drifts along the bank faster than the rocks do.</summary>
        public const int SceneryDriftX = 6;

        /// <summary>Scenery vertical drift, up the screen.</summary>
        public const int SceneryDriftY = -3;

        /// <summary>`:1115` — past this tick, touching the last lane is a landing rather than a crash.</summary>
        public const int LandingOpensAfter = 205;

        /// <summary>`:1116` — and past this one the chance is gone.</summary>
        public const int LandingClosesAfter = 225;

        /// <summary>`:1070` — the tick the Willamette landing marker appears on the bank.</summary>
        public const int LandingAppearsTick = 175;

        /// <summary>Ticks the three direction signs appear on.</summary>
        public static readonly int[] SignTicks = [60, 120, 170];

        /// <summary>Ticks the first two signs are taken away again; the third simply drifts off.</summary>
        public static readonly int[] SignClearTicks = [97, 157];

        // Collision boxes are offsets INTO the sprites, not the sprite bounds: the raft's box is the log deck, not
        // the wagon cover standing on it (`:1110`/`:1120`, constants C2..C8).
        private const int RaftBoxX = 6, RaftBoxY = 18, RaftBoxW = 20, RaftBoxH = 7;
        private const int RockBoxX = 11, RockBoxY = 1, RockBoxW = 20, RockBoxH = 6;

        // Where bank scenery enters, straight out of the listing — the signs at `:1145` (`SX = 62 : SY = 189`) and the
        // landing at `:1146` (`TX = -8 : TY = 218`), which are NOT the same spot. Both work out at lane 20.4 and 19.8,
        // comfortably past the near bank at 17.5, so each rides the sand rather than the waterline; drifting +6/-3
        // holds that lane exactly, so they run along the bank. The landing starts below the bottom of the screen and
        // climbs into view, arriving beside the raft about when the window opens on tick 205.
        private const int SignStartX = 62, SignStartY = 189;
        private const int LandingStartX = -8, LandingStartY = 218;

        private readonly Random _random;

        /// <summary>Initializes a new instance of the <see cref="RaftGame" /> class.</summary>
        /// <param name="seed">Fixed seed for a reproducible run, or null to be seeded from the clock.</param>
        public RaftGame(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            Rocks = [new Drifter(), new Drifter()];
            Sign = new Drifter();
            Landing = new Drifter();
            Reset();
        }

        /// <summary>`HP` — which of the 18 lanes the raft is in.</summary>
        public int Lane { get; private set; }

        /// <summary>`DIR` — the drift the raft carries, one of -1, 0, +1.</summary>
        public int Drift { get; private set; }

        /// <summary>`TC` — ticks elapsed this run.</summary>
        public int Tick { get; private set; }

        /// <summary>The two rock slots.</summary>
        public Drifter[] Rocks { get; }

        /// <summary>The direction sign currently on the bank, if any.</summary>
        public Drifter Sign { get; }

        /// <summary>The Willamette landing marker, once it appears.</summary>
        public Drifter Landing { get; }

        /// <summary>How the run ended, or <see cref="RaftOutcomeEnum.Running" />.</summary>
        public RaftOutcomeEnum Outcome { get; private set; }

        /// <summary>Rocks struck this run.</summary>
        public int RockHits { get; private set; }

        /// <summary>Times the raft scraped a bank this run.</summary>
        public int ShoreHits { get; private set; }

        /// <summary>What just happened, for the heads-up display.</summary>
        public string LastEvent { get; private set; } = "Cast off.";

        /// <summary>True while the landing can actually be taken.</summary>
        public bool LandingWindowOpen => Tick > LandingOpensAfter && Tick <= LandingClosesAfter;

        /// <summary>Puts everything back to the state `:1030` leaves it in.</summary>
        public void Reset()
        {
            Lane = StartLane;
            Drift = StartDrift;
            Tick = 0;
            Outcome = RaftOutcomeEnum.Running;
            RockHits = 0;
            ShoreHits = 0;
            LastEvent = "Cast off.";

            foreach (var rock in Rocks)
                rock.Active = false;

            Sign.Active = false;
            Landing.Active = false;
        }

        /// <summary>Advances one tick of the original's main loop.</summary>
        /// <param name="steer">The key held this tick, if any.</param>
        public void Step(RaftSteerEnum steer)
        {
            if (Outcome != RaftOutcomeEnum.Running)
                return;

            Tick++;

            // `:1090`-`:1110`. The keys do not move the raft — they nudge a drift that is then applied. Stopping dead
            // takes one press against the drift; holding a lane takes alternating presses.
            if (steer == RaftSteerEnum.Far && Lane > FirstLane)
                Drift = Math.Max(-1, Drift - 1);
            else if (steer == RaftSteerEnum.Near && Lane < LastLane)
                Drift = Math.Min(1, Drift + 1);

            if (Drift != 0)
                Lane = Math.Clamp(Lane + Drift, FirstLane, LastLane);

            AdvanceRocks();
            AdvanceScenery();

            // The landing is checked before the bank, because after the window opens the same contact means the
            // opposite thing (`:1115`).
            if (Lane >= LastLane && LandingWindowOpen)
            {
                Outcome = RaftOutcomeEnum.Landed;
                LastEvent = "Landed at the trail to the Willamette Valley.";
                return;
            }

            if (Tick > LandingClosesAfter)
            {
                Outcome = RaftOutcomeEnum.Missed;
                LastEvent = "Missed the landing — supplies lost (0.50).";
                return;
            }

            CheckShore();
            CheckRocks();
        }

        private void AdvanceRocks()
        {
            foreach (var rock in Rocks)
            {
                if (rock.Active)
                {
                    rock.X += RockDriftX;
                    rock.Y += RockDriftY;

                    // `:600` — a rock lives only while it is still left of 240 and below row 10.
                    if (!(rock.X < 240 && rock.Y > 10))
                        rock.Active = false;

                    continue;
                }

                if (_random.NextDouble() >= RockSpawnChance)
                    continue;

                // `:300` — mostly along the left edge, occasionally out of the bottom-left corner.
                if (_random.NextDouble() < 0.13)
                {
                    rock.X = _random.Next(0, 10);
                    rock.Y = 175;
                }
                else
                {
                    rock.X = 0;
                    rock.Y = _random.Next(51, 171);
                }

                rock.Active = true;
            }
        }

        private void AdvanceScenery()
        {
            foreach (var piece in new[] { Sign, Landing })
            {
                if (!piece.Active)
                    continue;

                piece.X += SceneryDriftX;
                piece.Y += SceneryDriftY;
                if (piece.X > ScreenWidth || piece.Y < -40)
                    piece.Active = false;
            }

            if (Array.IndexOf(SignTicks, Tick) >= 0)
            {
                Sign.X = SignStartX;
                Sign.Y = SignStartY;
                Sign.Active = true;
            }
            else if (Array.IndexOf(SignClearTicks, Tick) >= 0)
            {
                Sign.Active = false;
            }

            if (Tick == LandingAppearsTick)
            {
                Landing.X = LandingStartX;
                Landing.Y = LandingStartY;
                Landing.Active = true;
            }
        }

        private void CheckShore()
        {
            // `:400` — the banks are lanes 0 and 17.
            if (Lane >= 1 && Lane <= 16)
                return;

            // `:700` charges it and the loop goes on; `:400` also reverses the drift, so the raft is pushed back
            // off the bank rather than grinding along it.
            ShoreHits++;
            Drift = -Drift;
            LastEvent = "The raft has hit the shore — drown 0.15 / oxen 0.30 / supplies 0.50.";
        }

        private void CheckRocks()
        {
            var raftX = LaneX(Lane) + RaftBoxX;
            var raftY = LaneY(Lane) + RaftBoxY;

            foreach (var rock in Rocks)
            {
                if (!rock.Active)
                    continue;

                if (!Overlaps(raftX, raftY, RaftBoxW, RaftBoxH,
                        rock.X + RockBoxX, rock.Y + RockBoxY, RockBoxW, RockBoxH))
                    continue;

                // `:710` clears the rock that struck (`FL(ROCK) = 0`) so the slot can respawn, and plays on.
                RockHits++;
                rock.Active = false;
                LastEvent = "The raft has hit a rock — drown 0.60 / oxen 0.60 / supplies 0.70.";
                return;
            }
        }

        /// <summary>`:1040` — lane index to screen column, in native Apple II hi-res pixels.</summary>
        public static int LaneX(int lane) => 84 + 8 * lane;

        /// <summary>`:1040` — lane index to screen row. Lane 0 sits partly off the top edge, exactly as it did.</summary>
        public static int LaneY(int lane) => 5 * lane - 6;

        /// <summary>
        ///     The inverse of <see cref="LaneX" />/<see cref="LaneY" />: which lane a point on the screen falls in,
        ///     measured <i>across</i> the river. Lane 0 is the far bank and 17 the near one, and the value runs on past
        ///     both onto dry land, which is what lets a renderer decide where the water stops.
        ///     <para>
        ///         Both of the original's drift vectors — a rock's <c>(+8, -4)</c> and bank scenery's <c>(+6, -3)</c> —
        ///         leave this value unchanged, i.e. everything the river carries stays in its lane and travels along the
        ///         bank. That is the check that this really is the axis the 1985 artist drew the river on.
        ///     </para>
        /// </summary>
        /// <param name="x">Screen column in the Apple II's 280-wide space.</param>
        /// <param name="y">Screen row in the Apple II's 192-tall space.</param>
        public static double LaneOf(double x, double y) => ((x - 84) * 4 + (y + 6) * 8) / 72.0;

        private static bool Overlaps(int ax, int ay, int aw, int ah, int bx, int by, int bw, int bh)
        {
            return ax < bx + bw && bx < ax + aw && ay < by + bh && by < ay + ah;
        }

        /// <summary>Something on the river or the bank that enters, drifts, and leaves.</summary>
        public sealed class Drifter
        {
            /// <summary>Left edge, in native Apple II pixels.</summary>
            public int X { get; set; }

            /// <summary>Top edge, in native Apple II pixels.</summary>
            public int Y { get; set; }

            /// <summary>Whether the slot currently holds anything.</summary>
            public bool Active { get; set; }
        }
    }
}
