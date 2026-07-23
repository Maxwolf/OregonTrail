namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>Which horizon the leg is drawn against.</summary>
    public enum TravelTerrainEnum
    {
        /// <summary>Rolling green plains — the Apple II's <c>PRAIRIE.IMA</c>, side A, landmarks 0-4.</summary>
        Plains,

        /// <summary>Snow-capped peaks — the Apple II's <c>MOUNTAINS.IMA</c>, side B, landmarks 5-17.</summary>
        Mountains
    }

    /// <summary>
    ///     What the ground is doing, which in the original is the <i>only</i> thing weather changes about this screen.
    /// </summary>
    public enum TravelWeatherEnum
    {
        /// <summary>Green: the fall-through case.</summary>
        Temperate,

        /// <summary>Orange: the rain accumulator <c>AR</c> has stayed at or below .2.</summary>
        Arid,

        /// <summary>White: <c>AS</c> is non-zero, which it only is before April.</summary>
        Snow
    }

    /// <summary>Which team frame is showing. The last two are the loss events swapping the wagon out.</summary>
    public enum TravelWagonEnum
    {
        /// <summary>The three-frame walk cycle.</summary>
        Rolling,

        /// <summary>A wheel is off and the wagon is down — nothing moves.</summary>
        Broken,

        /// <summary>The wagon is alight — nothing moves.</summary>
        Burning
    }

    /// <summary>How hard the team is being driven, which shows up directly as how fast the oxen's legs cycle.</summary>
    public enum TravelPaceEnum
    {
        /// <summary>Steady.</summary>
        Steady,

        /// <summary>Strenuous.</summary>
        Strenuous,

        /// <summary>Grueling.</summary>
        Grueling
    }

    /// <summary>
    ///     The travel screen, modelled from <c>legacy/source/v1.4-sideA/OREGON TRAIL.txt</c> lines <c>310</c>,
    ///     <c>312</c>, <c>3000</c> and <c>3246</c>. Pure logic: it draws nothing and knows nothing about WolfCurses.
    ///     <para>
    ///         The screen is <b>never a picture</b>. It is a flood-filled ground box, one full-width horizon strip, one
    ///         piece of roadside scenery and the team, reassembled every frame. The animation is the interesting part:
    ///     </para>
    ///     <code>
    ///     VP = INT(((FX - D*C2 - IX)/C4) + .5): IF VP THEN FOR L4 = 1 TO VP:
    ///         WG = WG*(WG&lt;3)+1: IX = IX + C4:
    ///         &amp; DFT,7,IA(C0): &amp; IMAGE,1,IX,31:
    ///         &amp; DFT,7,IA(WG): &amp; IMAGE,1,186,32: NEXT
    ///     </code>
    ///     <para>
    ///         Two things fall out of that loop, and both are reproduced here. First, the wagon is <b>nailed to
    ///         x=186</b> and the world slides past it, so the team never actually crosses the screen. Second,
    ///         <c>WG</c> (the walk frame) and <c>IX</c> (the world offset) advance <b>in the same iteration</b>, which
    ///         means the oxen's legs are tied to <i>ground covered</i> rather than to elapsed time — drive harder and
    ///         you watch them stride faster. The step count <c>VP</c> is a distance delta, not a frame rate.
    ///     </para>
    ///     <para>
    ///         Third, and least obvious: <c>IX = FX - D*C2</c> makes the single scenery piece a <b>progress bar</b>.
    ///         <c>D</c> is the distance still to run, so as the leg closes the scenery slides in from the left and comes
    ///         to rest at <c>FX</c> at the exact moment you arrive. It is not a passing landscape; it is the next
    ///         landmark approaching.
    ///     </para>
    /// </summary>
    public sealed class TravelGame
    {
        /// <summary>
        ///     Composition width. Unlike the raft, this screen is built in the <b>DOS port's</b> 320x200 rather than the
        ///     Apple II's 280x192, because the horizon strips are 320-pixel-wide art that has to line up with the screen
        ///     edges exactly. The handful of Apple II constants the BASIC does pin down are converted once, below.
        /// </summary>
        public const int ScreenWidth = 320;

        /// <summary>`:3246` — the wagon's x in Apple II pixels. It never changes; the world moves instead.</summary>
        public const int Apple2WagonX = 186;

        /// <summary>Composition height: the whole 320x200 screen, status panel and all.</summary>
        public const int ScreenHeight = 200;

        /// <summary>
        ///     The wagon's x. Converting the Apple II's 186 gives <b>212</b>, and measuring a screen capture of the DOS
        ///     port gives 212 with a 78-pixel frame — five years apart, the two ports park the wagon on the same pixel.
        /// </summary>
        public const int WagonX = Apple2WagonX * ScreenWidth / Assets.Apple2Width;

        /// <summary>
        ///     Where the horizon strip hangs. It is <b>not</b> seated on the ground: a wide black band separates the
        ///     bottom of the strip from the top of the ground box, and that band is where the team walks. Seating the
        ///     strip on the ground line instead — which looks like the sensible thing to do, and is what this port did
        ///     first — turns a distant horizon into a hedgerow standing at the oxen's feet.
        /// </summary>
        public const int StripY = 17;

        /// <summary>
        ///     The top of the flooded ground box, and the line every standing thing is seated on. The BASIC's
        ///     <c>&amp; BOX,0,60,...</c> (`:310`) converts to 62, but the DOS port measures at <b>71</b>; since the art
        ///     here is the DOS port's, its own number is the one that lines up with its own sprites. Seating the wagon
        ///     on it puts the team on rows 40-70 with the ground opening at 71, which is what the capture shows.
        /// </summary>
        public const int GroundY = 71;

        /// <summary>
        ///     The bottom of the ground box. Here the two ports agree: the Apple II's <c>Y5 = 111</c> (`:29004`, with
        ///     `:355` painting the status bar at <c>Y5</c>) scales to 115, and the DOS capture measures 115.
        /// </summary>
        public const int GroundBottomY = 115;

        /// <summary>The black band under the picture, carrying the prompt.</summary>
        public const int PromptY = 118;

        /// <summary>The top of the white status panel.</summary>
        public const int PanelY = 128;

        /// <summary>The bottom of it. The panel stops short of the screen foot, leaving a black margin.</summary>
        public const int PanelBottomY = 189;

        /// <summary>Status-panel labels are right-aligned so their colons end on this column.</summary>
        public const int PanelLabelRight = 160;

        /// <summary>...and their values all start on this one.</summary>
        public const int PanelValueLeft = 168;

        /// <summary>Rows between status-panel lines.</summary>
        public const int PanelLineHeight = 10;

        /// <summary>`C4` — how far the world slides per step. The original's value is not recoverable, so this is a
        ///     tuning knob; it only sets how finely distance is quantised into strides.</summary>
        public const int StepPixels = 2;

        /// <summary>`FX = 180 - L%(NM-1,1)` — where the leg's scenery comes to rest. The table is not decoded, so the
        ///     bare 180 stands in for it.</summary>
        public const int SceneryRestX = 180 * ScreenWidth / Assets.Apple2Width;

        /// <summary>Miles in a leg. Stands in for the trail's real spacing, which this workbench has no trail for.</summary>
        public const int LegMiles = 120;

        /// <summary>
        ///     The real trail, in order: what scrolls past on the way to each landmark.
        ///     <para>
        ///         This was guesswork — two pools picked at random by terrain — until <c>L%</c> was decoded out of
        ///         <c>VAR.BIN</c>. It is <c>DIM L%(16,1)</c>, indexed by <b>destination landmark - 1</b>, and
        ///         <c>OREGON TRAIL:3000</c> reads it as <c>NI = L%(NM-1,0)</c> for the image and
        ///         <c>FX = 180 - L%(NM-1,1)</c> for where that image parks. See the table at the foot of
        ///         <c>legacy/source/VAR.BIN.txt</c>, which <c>tools/varbin.py</c> now emits.
        ///     </para>
        ///     <para>
        ///         What it says is that the roadside object is <b>a miniature of the landmark you are heading
        ///         toward</b> — so the travel screen is a slow reveal of the next stop, drawing level with the wagon
        ///         exactly as the leg runs out, rather than the random scenery this had before.
        ///     </para>
        ///     <para>
        ///         <c>Apple</c> records the original's own <c>(table, NI)</c> so the mapping stays checkable against
        ///         the disk. The DOS sprite ids are a <b>correspondence</b>, not a decode: the DOS port re-drew this
        ///         art and its own table is locked inside an LZEXE-packed <c>OREGON.EXE</c>. The two sets do match
        ///         one-to-one — 15 roadside pieces each — and every pairing below is by subject, but they have not
        ///         been read out of the DOS binary.
        ///     </para>
        /// </summary>
        public static readonly TravelLeg[] Trail =
        [
            new("the Kansas River crossing", 11, "river", "PRAIRIE #8", 102),
            new("the Big Blue River crossing", 11, "river", "PRAIRIE #8", 102),
            new("Fort Kearney", 3, "timber stockade", "PRAIRIE #5", 130),
            new("Chimney Rock", 10, "rock spire", "PRAIRIE #6", 144),
            new("Fort Laramie", 6, "adobe fort", "PRAIRIE #7", 130),
            new("Independence Rock", 13, "bare dome", "MOUNTAINS #6", 144),
            new("South Pass", 14, "grey outcrops", "MOUNTAINS #7", 124),
            new("Fort Bridger", 5, "log stockade", "MOUNTAINS #8", 124),
            new("the Green River crossing", 11, "river", "MOUNTAINS #14", 102),
            new("Soda Springs", 15, "spring shallows", "MOUNTAINS #9", 116),
            new("Fort Hall", 4, "adobe walls", "MOUNTAINS #10", 110),
            new("the Snake River crossing", 11, "river", "MOUNTAINS #14", 102),
            new("Fort Boise", 7, "fort complex", "MOUNTAINS #11", 124),
            new("the Blue Mountains", 16, "snow-capped ridge", "MOUNTAINS #15", 124),
            new("Fort Walla Walla", 8, "post and colours", "MOUNTAINS #12", 124),
            new("The Dalles", 17, "river through timber", "MOUNTAINS #13", 68),
            new("the Willamette Valley", 12, "valley timber", "MOUNTAINS #16", 130)
        ];

        /// <summary>Initializes a new instance of the <see cref="TravelGame" /> class.</summary>
        public TravelGame() => Reset();

        /// <summary>Which horizon strip and scenery pool the leg uses.</summary>
        public TravelTerrainEnum Terrain { get; private set; } = TravelTerrainEnum.Plains;

        /// <summary>What colour the ground floods to.</summary>
        public TravelWeatherEnum Weather { get; private set; } = TravelWeatherEnum.Temperate;

        /// <summary>Whether the team is walking, or is one of the two wrecked frames.</summary>
        public TravelWagonEnum Wagon { get; private set; } = TravelWagonEnum.Rolling;

        /// <summary>How hard the team is driven, which sets how many strides a tick buys.</summary>
        public TravelPaceEnum Pace { get; private set; } = TravelPaceEnum.Steady;

        /// <summary>`WG` — the walk frame, 1-3, advanced once per stride rather than once per tick.</summary>
        public int WalkFrame { get; private set; } = 1;

        /// <summary>`IX` — how far the world has slid. Derived from <see cref="MilesRemaining" />, never accumulated.</summary>
        public int SceneryX { get; private set; }

        /// <summary>`D` — miles still to run on this leg.</summary>
        public double MilesRemaining { get; private set; }

        /// <summary>Miles run since the workbench started, for the readout.</summary>
        public double MilesTravelled { get; private set; }

        /// <summary>
        ///     Days on the trail, at the original's rough fifteen miles to a day. There is no simulation behind this
        ///     workbench, so this and the three readings below are <b>stand-ins derived from distance</b> — enough for
        ///     the status panel to look alive and for its layout to be checked, and not to be mistaken for game state.
        /// </summary>
        private int Days => (int) (MilesTravelled / 15.0);

        /// <summary>The date, counted off from the departure the screenshot happens to show.</summary>
        public DateTime Date => new DateTime(1848, 6, 15).AddDays(Days);

        /// <summary>Pounds of food left. A stand-in; see <see cref="Days" />.</summary>
        public int Food => Math.Max(0, 1895 - Days * 5);

        /// <summary>The party's health. A stand-in; see <see cref="Days" />.</summary>
        public string Health => "good";

        /// <summary>What the panel calls the weather, as against what the ground does about it.</summary>
        public string WeatherWord => Weather switch
        {
            TravelWeatherEnum.Arid => "hot",
            TravelWeatherEnum.Snow => "snowy",
            _ => "fair"
        };

        /// <summary>The leg being travelled, and so the scenery scrolling past.</summary>
        public TravelLeg Scenery { get; private set; }

        /// <summary>Which leg of the trail this is, 0-16.</summary>
        public int Leg { get; private set; }

        /// <summary>Strides taken in the last tick — the original's <c>VP</c>, and the reason the legs cycle.</summary>
        public int LastStrides { get; private set; }

        /// <summary>True once the leg has run out, which is when the scenery has reached its resting place.</summary>
        public bool Arrived => MilesRemaining <= 0;

        /// <summary>
        ///     Miles a tick buys at the current pace. Kept in the same 1 : 1.7 : 2.5 proportion the original drives
        ///     the team at, but small in absolute terms, and that part is not a taste decision — see below.
        ///     <para>
        ///         <b>These numbers have to stay low enough that a tick is well under one walk cycle.</b> The walk is
        ///         three frames and the frame advances once per stride, so if a tick covers three strides the legs land
        ///         back on the frame they started on and the team appears frozen. That is not a subtle effect: at the
        ///         first values here (1.2 / 2.0 / 3.0 miles) grueling worked out at 3.24 strides a tick and looked
        ///         <i>slower</i> than steady's 1.29, which is the exact opposite of what pace is supposed to show.
        ///     </para>
        ///     <para>
        ///         At these values the fastest pace is around half a stride per tick, so the cycle can never alias, and
        ///         <see cref="TravelForm" /> runs its clock faster to keep the journey taking about as long as before.
        ///         The rule of thumb for changing them: strides a tick is roughly <c>miles x 1.1</c>, and it wants to
        ///         stay under about 1.5 even for the widest scenery.
        ///     </para>
        /// </summary>
        public double MilesPerTick => Pace switch
        {
            TravelPaceEnum.Strenuous => 0.33,
            TravelPaceEnum.Grueling => 0.50,
            _ => 0.20
        };

        /// <summary>
        ///     Pixels of world travel per mile — the original's <c>C2</c>. Solved rather than chosen: the scenery has to
        ///     start just off the left edge with the full leg to run, and rest at <see cref="SceneryRestX" /> with none
        ///     of it left.
        /// </summary>
        private double PixelsPerMile => (SceneryRestX + Scenery.Width) / (double) LegMiles;

        /// <summary>Starts a fresh leg, keeping the terrain, weather and pace the operator has dialled in.</summary>
        public void Reset()
        {
            MilesRemaining = LegMiles;
            MilesTravelled = 0;
            Leg = 0;
            Wagon = TravelWagonEnum.Rolling;
            NextLeg(false);
        }

        /// <summary>Moves on to the next leg of the trail and refills it.</summary>
        /// <param name="count">Whether this counts as a leg completed, as opposed to a restart or a terrain change.</param>
        public void NextLeg(bool count = true)
        {
            if (count)
                Leg = (Leg + 1) % Trail.Length;

            // The terrain follows the trail rather than the operator's switch, because the original's does: the
            // world turns from green rolling hills to purple peaks at the disk flip, which falls after Fort
            // Laramie -- PRAIRIE.IMA ships on side A with L0-L4.PCK, MOUNTAINS.IMA on side B with the rest.
            Terrain = Leg < PrairieLegs ? TravelTerrainEnum.Plains : TravelTerrainEnum.Mountains;
            Scenery = Trail[Leg];
            MilesRemaining = LegMiles;

            Recompute();
        }

        /// <summary>
        ///     Legs still on side A, and so still on the prairie. The fifth leg ends at Fort Laramie; everything
        ///     after it is drawn from <c>MOUNTAINS.IMA</c>.
        /// </summary>
        private const int PrairieLegs = 5;

        /// <summary>
        ///     One tick. Converts the tick's miles into whole strides and takes them, which is the original's inner
        ///     <c>FOR</c> loop: each stride advances the walk frame and slides the world by <see cref="StepPixels" />.
        /// </summary>
        public void Step()
        {
            LastStrides = 0;

            // A wrecked wagon is not going anywhere, and the two damage frames replace the walk cycle rather than
            // animating alongside it.
            if (Wagon != TravelWagonEnum.Rolling || Arrived)
                return;

            var before = SceneryX;

            MilesRemaining = Math.Max(0, MilesRemaining - MilesPerTick);
            MilesTravelled += MilesPerTick;
            Recompute();

            // VP: the number of whole steps the world moved. The walk frame advances once per step, so the team's
            // stride rate is a readout of distance covered and not of elapsed time.
            LastStrides = Math.Max(0, (SceneryX - before) / StepPixels);
            for (var stride = 0; stride < LastStrides; stride++)
                WalkFrame = WalkFrame % 3 + 1;   // WG = WG*(WG<3)+1
        }

        /// <summary>Flips the horizon, which on the Apple II meant turning the disk over.</summary>
        public void ToggleTerrain()
        {
            // Terrain is a property of where you are on the trail now, not a switch, so this jumps to the other
            // side of the disk flip rather than repainting the leg you are on.
            Leg = Terrain == TravelTerrainEnum.Plains ? PrairieLegs : 0;
            NextLeg(false);
        }

        /// <summary>Cycles the ground colour through the three cases of the original's one-line formula.</summary>
        public void CycleWeather() =>
            Weather = Weather switch
            {
                TravelWeatherEnum.Temperate => TravelWeatherEnum.Arid,
                TravelWeatherEnum.Arid => TravelWeatherEnum.Snow,
                _ => TravelWeatherEnum.Temperate
            };

        /// <summary>Cycles the pace, and with it the stride rate.</summary>
        public void CyclePace() =>
            Pace = Pace switch
            {
                TravelPaceEnum.Steady => TravelPaceEnum.Strenuous,
                TravelPaceEnum.Strenuous => TravelPaceEnum.Grueling,
                _ => TravelPaceEnum.Steady
            };

        /// <summary>Cycles the team through walking, broken down, and on fire.</summary>
        public void CycleWagon() =>
            Wagon = Wagon switch
            {
                TravelWagonEnum.Rolling => TravelWagonEnum.Broken,
                TravelWagonEnum.Broken => TravelWagonEnum.Burning,
                _ => TravelWagonEnum.Rolling
            };

        /// <summary>`IX = FX - D*C2`, quantised down to whole steps so the world only ever moves in strides.</summary>
        private void Recompute()
        {
            var exact = SceneryRestX - MilesRemaining * PixelsPerMile;
            SceneryX = (int) Math.Floor(exact / StepPixels) * StepPixels;
        }
    }

    /// <summary>One leg of the trail, and the roadside piece that scrolls past on the way to its landmark.</summary>
    /// <param name="Toward">The landmark this leg ends at — which is what chooses the scenery.</param>
    /// <param name="SpriteId">1-based id within the scenery sheet (<c>art/sprites/scenery/NN.png</c>).</param>
    /// <param name="Name">What the artist drew, for the readout.</param>
    /// <param name="Apple">
    ///     The original's own <c>(terrain table, NI)</c>, kept so the mapping stays checkable against the disk.
    /// </param>
    /// <param name="RestX">
    ///     <c>FX = 180 - L%(NM-1,1)</c>, in the Apple II's 280-wide space: where the piece stops. Carried for
    ///     reference — this workbench solves its own scroll from the art's width, since the DOS art is not the
    ///     art those offsets were authored against.
    /// </param>
    public readonly record struct TravelLeg(string Toward, int SpriteId, string Name, string Apple, int RestX)
    {
        /// <summary>The art's own width, which the leg's pixels-per-mile is solved against.</summary>
        public int Width => Assets.Dos("scenery", SpriteId).Width;
    }
}
