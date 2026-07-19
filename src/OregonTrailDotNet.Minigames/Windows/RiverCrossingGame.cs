namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>How the party is trying to get across. This is the menu the player picks from.</summary>
    public enum CrossingMethodEnum
    {
        /// <summary>"attempt to ford the river" — pull the wagon across a shallow part, oxen still attached.</summary>
        Ford,

        /// <summary>"caulk wagon and float it across" — seal it and float it like a boat.</summary>
        Float,

        /// <summary>"take a ferry across" — offered only by rivers whose option column is 2.</summary>
        Ferry,

        /// <summary>"hire an Indian to help" — offered only by rivers whose option column is 1.</summary>
        IndianGuide
    }

    /// <summary>What the crossing did, which is what decides the last beat of the animation.</summary>
    public enum CrossingOutcomeEnum
    {
        /// <summary>Across clean.</summary>
        Success,

        /// <summary>A muddy bottom held the wagon. Costs a day, loses nothing.</summary>
        StuckInMud,

        /// <summary>A rough bottom tipped the wagon. May cost supplies.</summary>
        Tipped,

        /// <summary>Shallow enough to survive, deep enough to soak the load.</summary>
        SuppliesWet,

        /// <summary>Too deep to ford: supplies, oxen and people all at risk.</summary>
        TooDeep,

        /// <summary>The floating wagon went over.</summary>
        Capsized,

        /// <summary>The ferry parted from its moorings.</summary>
        BrokeLoose,

        /// <summary>The crossing never happened — too shallow to float, or the ferry is not running.</summary>
        Refused
    }

    /// <summary>Which beat of the animation is on screen.</summary>
    public enum CrossingPhaseEnum
    {
        /// <summary>The framed scene is up and holding, before anything moves.</summary>
        Present,

        /// <summary>The near bank is filling with water as the party pulls away from it.</summary>
        Crossing,

        /// <summary>The outcome is playing: landing, swamping, or the wreck.</summary>
        Outcome,

        /// <summary>Finished, holding before the next scenario.</summary>
        Done
    }

    /// <summary>One of the four rivers, exactly as <c>VAR.BIN</c>'s <c>RC()</c> table has it.</summary>
    /// <param name="Name">The landmark's name.</param>
    /// <param name="Landmark">Its <c>LM</c> index, which is what selects the row.</param>
    /// <param name="Depth">Base depth in feet; the live depth is this plus <c>2*AR</c>.</param>
    /// <param name="Width">Base width in feet; live width adds <c>15*AR</c>.</param>
    /// <param name="Speed">Base swiftness; live speed adds <c>AR</c>.</param>
    /// <param name="Bottom">0 none, 1 muddy, 2 rough — only consulted when fording a shallow river.</param>
    /// <param name="Option">0 none, 1 Indian guide, 2 ferry — the third menu entry this river offers.</param>
    public readonly record struct RiverConfig(
        string Name, int Landmark, double Depth, int Width, double Speed, int Bottom, int Option);

    /// <summary>One scripted crossing: a river, a method, and the outcome to demonstrate.</summary>
    /// <param name="River">Which river.</param>
    /// <param name="Method">How they are trying to cross.</param>
    /// <param name="Outcome">What happens — forced, so every visual state gets shown.</param>
    /// <param name="Lost">
    ///     Whether anything was actually lost, the original's <c>Z &gt; 0</c>. This is <b>not</b> the same as the
    ///     crossing going wrong, and it is what <c>CROSS.LIB:50020</c> branches on: a rough bottom can tip the wagon
    ///     and cost nothing, which sets <c>Z = 0</c> and plays the ordinary landing.
    /// </param>
    /// <param name="Note">Why this case exists, shown on screen.</param>
    public readonly record struct CrossingScenario(
        RiverConfig River, CrossingMethodEnum Method, CrossingOutcomeEnum Outcome, bool Lost, string Note);

    /// <summary>
    ///     River crossings, from <c>legacy/source/v1.4-sideA/RIVER.LIB.txt</c> (the choice and its consequences) and
    ///     <c>CROSS.LIB.txt</c> (the animation). Pure logic; it draws nothing.
    ///     <para>
    ///         <b>The scene is four layers and one sprite.</b> <c>CROSS.LIB:50000</c> draws a framed box, fills it with
    ///         the river, and blits <i>one</i> vehicle sprite into it — <c>&amp; IMAGE,I,102,63</c>, where <c>I</c> was
    ///         set by whichever branch of <c>RIVER.LIB</c> ran. That single variable is the whole of "which sprite":
    ///         <b>1 ferry, 2 fording wagon, 3 floating wagon, 4 wreck, 5 the river itself</b>.
    ///     </para>
    ///     <para>
    ///         <b>Black means land.</b> The river picture is water with a black triangle in the bottom-right corner —
    ///         the bank you are leaving. Once that is understood the two animation sweeps stop looking arbitrary and
    ///         become the same idea twice. <c>:50010</c> fans <b>blue</b> lines across that bottom-right triangle: the
    ///         near bank turns to water as you pull away from it. <c>:50030</c>, on success, fans <b>black</b> lines
    ///         across the top-left: land appears ahead of you, and that is the far shore. Departure and arrival, drawn
    ///         with the same primitive in opposite corners.
    ///     </para>
    ///     <para>
    ///         Failure is drawn per vehicle (<c>:50040</c>, <c>ON I GOSUB 50050,50060,50050</c>): the ferry and the
    ///         floating wagon are <b>swapped for the wreck sprite in place</b>, while the fording wagon instead gets a
    ///         wedge of blue fanned <i>over</i> it — it is not replaced, it is swamped where it stands.
    ///     </para>
    ///     <para>
    ///         Note the whole sequence is one <b>fall-through</b>: <c>:50000</c> has no <c>RETURN</c>, so it runs on
    ///         into <c>:50010</c> and then <c>:50020</c>, which is the only line that returns.
    ///     </para>
    /// </summary>
    public sealed class RiverCrossingGame
    {
        /// <summary>Composition width; the DOS surface, as with the travel screen.</summary>
        public const int ScreenWidth = 320;

        /// <summary>Composition height.</summary>
        public const int ScreenHeight = 200;

        // CROSS.LIB works in the Apple II's 280x192. Every constant below is the BASIC's own number, converted once
        // here rather than sprinkled through the drawing code.
        private const int Apple2FrameX1 = 54, Apple2FrameY1 = 20, Apple2FrameX2 = 222, Apple2FrameY2 = 140;
        private const int Apple2VehicleX = 102, Apple2VehicleY = 63;

        /// <summary>`:50000` — <c>&amp; BOX,X1,Y1,X2,Y2</c> then three inset rectangles: the picture frame.</summary>
        public static readonly int FrameX1 = ToX(Apple2FrameX1), FrameY1 = ToY(Apple2FrameY1);

        /// <summary>The frame's far corner.</summary>
        public static readonly int FrameX2 = ToX(Apple2FrameX2), FrameY2 = ToY(Apple2FrameY2);

        /// <summary>`&amp; IMAGE,I,102,63` — where the vehicle sits, whichever vehicle it is.</summary>
        public static readonly int VehicleX = ToX(Apple2VehicleX), VehicleY = ToY(Apple2VehicleY);

        /// <summary>`:50010` — the near-bank triangle, filled with water over 53 steps as the party pulls away.</summary>
        public const int CrossingSteps = 53;

        /// <summary>`:50030` — the far-bank triangle, filled with land over 61 steps on a clean crossing.</summary>
        public const int LandingSteps = 61;

        /// <summary>`:50060` — the wedge of water fanned over a fording wagon that got into trouble.</summary>
        public const int SwampSteps = 22;

        /// <summary>Ticks the framed scene holds before anything moves, and again once it is over.</summary>
        public const int HoldTicks = 14;

        /// <summary>
        ///     The <c>RC()</c> table, verbatim. <c>RIVER.LIB:50000</c> selects a row with
        ///     <c>RC = (LM=2) + 2*(LM=9) + 3*(LM=12)</c> — so three named rivers get their own row and
        ///     <b>every other crossing falls through to row 0</b>.
        /// </summary>
        public static readonly RiverConfig[] Rivers =
        [
            new("the Kansas River crossing", 1, 1.0, 600, 3.0, 0, 2),
            new("the Big Blue River crossing", 2, 1.0, 220, 2.0, 1, 0),
            new("Green River crossing", 9, 20.0, 400, 5.0, 2, 2),
            new("the Snake River crossing", 12, 6.0, 1000, 7.0, 2, 1)
        ];

        /// <summary>
        ///     Every case worth looking at, in order. Outcomes are <b>forced</b> rather than rolled, because the point
        ///     of the workbench is to see all of them — the real probabilities are on <see cref="RiskOf" /> and are
        ///     what a wired-up game would roll against.
        /// </summary>
        public static readonly CrossingScenario[] Scenarios =
        [
            new(Rivers[0], CrossingMethodEnum.Ford, CrossingOutcomeEnum.Success, false,
                "Shallow, no bottom hazard: the plain case."),
            new(Rivers[1], CrossingMethodEnum.Ford, CrossingOutcomeEnum.StuckInMud, false,
                "Bottom 1 (muddy). :50060 costs a day and forces Z=0, so it still lands normally."),
            new(Rivers[3], CrossingMethodEnum.Ford, CrossingOutcomeEnum.Tipped, false,
                "Bottom 2 (rough), tipped but lost nothing — Z=0, so this too lands normally."),
            new(Rivers[3], CrossingMethodEnum.Ford, CrossingOutcomeEnum.Tipped, true,
                "The same tip, but :50205 took supplies. Z>0 now, and the disaster branch runs."),
            new(Rivers[0], CrossingMethodEnum.Ford, CrossingOutcomeEnum.SuppliesWet, false,
                "Depth 2.5-3 ft: costs a day, forces Z=0."),
            new(Rivers[2], CrossingMethodEnum.Ford, CrossingOutcomeEnum.TooDeep, true,
                "Depth 20 ft. :50040 risks supplies, then oxen, then lives."),
            new(Rivers[0], CrossingMethodEnum.Float, CrossingOutcomeEnum.Refused, false,
                "Under 1.5 ft, so :50080 refuses — nothing to float in."),
            new(Rivers[2], CrossingMethodEnum.Float, CrossingOutcomeEnum.Success, false,
                "Deep and slow enough to float. Always costs a day."),
            new(Rivers[3], CrossingMethodEnum.Float, CrossingOutcomeEnum.Capsized, true,
                "Speed 7: risk is (RD>2.5)*(RS/20), and this one goes over."),
            new(Rivers[0], CrossingMethodEnum.Ferry, CrossingOutcomeEnum.Refused, false,
                "Under 2.5 ft: the operator will not run today."),
            new(Rivers[2], CrossingMethodEnum.Ferry, CrossingOutcomeEnum.Success, false,
                "$5 and a 2-6 day wait, and the safest way over."),
            new(Rivers[2], CrossingMethodEnum.Ferry, CrossingOutcomeEnum.BrokeLoose, true,
                "Risk is .05*(RS>5) + .1*(RS>10) — it parts from its moorings."),
            new(Rivers[3], CrossingMethodEnum.IndianGuide, CrossingOutcomeEnum.Success, false,
                "Costs 2-3 sets of clothing, sets IX=5, and reuses the float sprite.")
        ];

        private int _phaseStep;

        /// <summary>Initializes a new instance of the <see cref="RiverCrossingGame" /> class.</summary>
        public RiverCrossingGame() => Reset();

        /// <summary>Which scripted crossing is showing.</summary>
        public int Index { get; private set; }

        /// <summary>The scenario itself.</summary>
        public CrossingScenario Scenario => Scenarios[Index];

        /// <summary>Which beat of the animation is running.</summary>
        public CrossingPhaseEnum Phase { get; private set; }

        /// <summary>How far into the current beat, in the original's own step counts.</summary>
        public int Step => _phaseStep;

        /// <summary>Whether the run is paused on the current frame.</summary>
        public bool Paused { get; private set; }

        /// <summary>
        ///     <c>I</c> — the one variable that decides which sprite gets blitted. The Indian guide has <b>no sprite of
        ///     its own</b>: <c>:50140</c> sends it to the ford or the float branch depending on depth, so it borrows
        ///     whichever of those two the river calls for.
        /// </summary>
        public CrossingMethodEnum Vehicle =>
            Scenario.Method != CrossingMethodEnum.IndianGuide
                ? Scenario.Method
                : LiveDepth > 2.4
                    ? CrossingMethodEnum.Float
                    : CrossingMethodEnum.Ford;

        /// <summary>Whether the failure is drawn by swapping in the wreck rather than by swamping in place.</summary>
        public bool ShowsWreck =>
            Failed && Vehicle != CrossingMethodEnum.Ford;

        /// <summary>Whether a fording wagon is being swamped where it stands.</summary>
        public bool ShowsSwamping =>
            Failed && Vehicle == CrossingMethodEnum.Ford;

        /// <summary>
        ///     The original's <c>Z &gt; 0</c>, and the <b>only</b> thing the animation branches on
        ///     (<c>:50020 ON (Z&gt;0)+1 GOSUB 50030,50040</c>). Note this asks whether anything was <i>lost</i>, not
        ///     whether the crossing was eventful: getting stuck in mud, soaking the supplies, and tipping without loss
        ///     all force <c>Z = 0</c> and land exactly like a clean crossing.
        /// </summary>
        public bool Failed => Scenario.Lost;

        /// <summary>Whether the party never got in the water.</summary>
        public bool Refused => Scenario.Outcome == CrossingOutcomeEnum.Refused;

        /// <summary>
        ///     Rain, <c>AR</c>. Every river reading is this plus a base, which is why a wet spring can open a ferry
        ///     that was refusing to run and close a ford that was safe.
        /// </summary>
        public double Rain { get; private set; } = 0.3;

        /// <summary>`RD = INT((base + 2*AR)*10 + .5)/10`.</summary>
        public double LiveDepth => Math.Round(Scenario.River.Depth + 2 * Rain, 1);

        /// <summary>`RW = INT(base + 15*AR)`.</summary>
        public int LiveWidth => (int) (Scenario.River.Width + 15 * Rain);

        /// <summary>`RS = base + AR`.</summary>
        public double LiveSpeed => Scenario.River.Speed + Rain;

        /// <summary>Steps in the beat currently running.</summary>
        public int PhaseLength => Phase switch
        {
            CrossingPhaseEnum.Crossing => CrossingSteps,
            CrossingPhaseEnum.Outcome => Failed
                ? Vehicle == CrossingMethodEnum.Ford ? SwampSteps : HoldTicks
                : LandingSteps,
            _ => HoldTicks
        };

        /// <summary>The message the original would print, from the branch that produced this outcome.</summary>
        public string Message => Scenario.Outcome switch
        {
            CrossingOutcomeEnum.Success when Scenario.Method == CrossingMethodEnum.Ferry =>
                "The ferry got your party and wagon safely across.",
            CrossingOutcomeEnum.Success when Vehicle == CrossingMethodEnum.Float =>
                "You had no trouble floating the wagon across.",
            CrossingOutcomeEnum.Success => "You made the crossing successfully.",
            CrossingOutcomeEnum.StuckInMud => "You become stuck in the mud.  Lose 1 day.",
            CrossingOutcomeEnum.Tipped when !Scenario.Lost =>
                "The wagon tipped over but you did not lose anything.",
            CrossingOutcomeEnum.Tipped => "The wagon tipped over.  You lose:",
            CrossingOutcomeEnum.BrokeLoose when !Scenario.Lost =>
                "Some trouble in crossing but nothing was lost.",
            CrossingOutcomeEnum.SuppliesWet => "Your supplies got wet.  Lose 1 day.",
            CrossingOutcomeEnum.TooDeep => "The river is too deep to ford.  You lose:",
            CrossingOutcomeEnum.Capsized => "The wagon tipped over while floating.  You lose:",
            CrossingOutcomeEnum.BrokeLoose => "The ferry broke loose from moorings. You lose:",
            CrossingOutcomeEnum.Refused when Scenario.Method == CrossingMethodEnum.Ferry =>
                "The ferry is not operating today because the river is to shallow.",
            _ => "The river is too shallow to float across."
        };

        /// <summary>
        ///     The real probability this outcome would be rolled against, as a readable formula. <c>IX</c> is the risk
        ///     divisor and is 1 for everything except the Indian guide, which sets it to <b>5</b> — the guide does not
        ///     change what happens, only how often it goes wrong.
        /// </summary>
        public string RiskOf => Scenario.Outcome switch
        {
            CrossingOutcomeEnum.StuckInMud => "RND < 0.4 / IX",
            CrossingOutcomeEnum.Tipped => "RND < 0.16 / IX",
            CrossingOutcomeEnum.TooDeep => "supplies RD/10, oxen (RD-1)/10, lives (RD-2.5)/10, all / IX",
            CrossingOutcomeEnum.Capsized => "RND < (RD>2.5) * (RS/20) / IX",
            CrossingOutcomeEnum.BrokeLoose => "RND < .05*(RS>5) + .1*(RS>10)",
            CrossingOutcomeEnum.SuppliesWet => "RD < 3 (certain, not rolled)",
            CrossingOutcomeEnum.Refused => "gated on depth, never rolled",
            _ => "no roll"
        };

        /// <summary>What this crossing costs in days before anything goes wrong.</summary>
        public string Toll => Scenario.Method switch
        {
            CrossingMethodEnum.Ferry => "$5 and a 2-6 day wait",
            CrossingMethodEnum.Float => "1 day",
            CrossingMethodEnum.IndianGuide => "2-3 sets of clothing",
            _ => "none"
        };

        /// <summary>Which third option this river puts on the menu, if any.</summary>
        public static string OptionName(int option) => option switch
        {
            1 => "hire an Indian to help",
            2 => "take a ferry across",
            _ => "(none — ford or float only)"
        };

        /// <summary>Restarts at the first scenario.</summary>
        public void Reset()
        {
            Index = 0;
            Restart();
        }

        /// <summary>Replays the current scenario from its first frame.</summary>
        public void Restart()
        {
            Phase = CrossingPhaseEnum.Present;
            _phaseStep = 0;
        }

        /// <summary>Moves to the next scripted crossing.</summary>
        public void Next()
        {
            Index = (Index + 1) % Scenarios.Length;
            Restart();
        }

        /// <summary>Moves to the previous one.</summary>
        public void Previous()
        {
            Index = (Index - 1 + Scenarios.Length) % Scenarios.Length;
            Restart();
        }

        /// <summary>Holds or releases the animation.</summary>
        public void TogglePause() => Paused = !Paused;

        /// <summary>Nudges the rain accumulator, which moves every reading the river has.</summary>
        public void AdjustRain(double delta) => Rain = Math.Clamp(Rain + delta, 0, 1);

        /// <summary>
        ///     One tick. Advances the current beat and rolls into the next when it runs out, looping the whole script
        ///     so the sections can be watched hands-off.
        /// </summary>
        public void Advance()
        {
            if (Paused)
                return;

            _phaseStep++;
            if (_phaseStep < PhaseLength)
                return;

            _phaseStep = 0;
            switch (Phase)
            {
                // A refused crossing never gets in the water, so it skips the two wet beats entirely.
                case CrossingPhaseEnum.Present:
                    Phase = Refused ? CrossingPhaseEnum.Done : CrossingPhaseEnum.Crossing;
                    break;
                case CrossingPhaseEnum.Crossing:
                    Phase = CrossingPhaseEnum.Outcome;
                    break;
                case CrossingPhaseEnum.Outcome:
                    Phase = CrossingPhaseEnum.Done;
                    break;
                default:
                    Next();
                    break;
            }
        }

        /// <summary>
        ///     Converts one of <c>CROSS.LIB</c>'s x coordinates into the composition surface. Public so the drawing
        ///     code can quote the BASIC's own numbers rather than re-deriving the shapes by hand — which is how the
        ///     near bank first came out as a band across the middle instead of a corner.
        /// </summary>
        public static int ToX(int apple2X) => apple2X * ScreenWidth / Assets.Apple2Width;

        /// <summary>Converts one of <c>CROSS.LIB</c>'s y coordinates; see <see cref="ToX" />.</summary>
        public static int ToY(int apple2Y) => apple2Y * ScreenHeight / Assets.Apple2Height;

        /// <summary>
        ///     Fractional forms of the same conversion. The sweeps step one Apple II pixel at a time, and widening
        ///     280 to 320 pushes adjacent lines about 1.3 pixels apart — enough to leave the filled banks visibly
        ///     striped. Drawing the fans at sub-step positions closes that up without changing their shape.
        /// </summary>
        public static int ToX(double apple2X) => (int) Math.Round(apple2X * ScreenWidth / Assets.Apple2Width);

        /// <summary>Fractional y conversion; see <see cref="ToX(double)" />.</summary>
        public static int ToY(double apple2Y) => (int) Math.Round(apple2Y * ScreenHeight / Assets.Apple2Height);

        /// <summary>Sub-steps drawn per original step, to keep the swept regions solid.</summary>
        public const int FanSubdivision = 3;
    }
}
