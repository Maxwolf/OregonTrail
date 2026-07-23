using WolfCurses.Graphics;

namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     Where a walking thing's frames live on the DOS sheets — the single answer to "which cut sprite is species
    ///     <i>s</i>, walk frame <i>f</i>, facing <i>d</i>".
    ///     <para>
    ///         This is deliberately shared rather than kept next to the code that draws the hunt. The sprite viewer's
    ///         animation page exists to catch a bad frame order or a flipped mirror, and it can only do that if it
    ///         asks the <b>same</b> question the game asks. A second copy of the mapping would agree with itself and
    ///         prove nothing.
    ///     </para>
    /// </summary>
    public static class DosFrames
    {
        /// <summary>
        ///     Our compass (0 = N, clockwise) onto the hunter sheet's groups of three. The sheet stores each drawn
        ///     strip immediately followed by its mirror, so its groups run N, NE, NW, E, W, SE, SW, S rather than
        ///     clockwise.
        /// </summary>
        public static readonly int[] AimToGroup = [0, 1, 3, 5, 7, 6, 4, 2];

        /// <summary>
        ///     Our species order is the Apple II's table at <c>$E068</c> (antlered deer, bear, bison, doe, rabbit,
        ///     squirrel); the DOS sheet's rows run bison, bear, antlered deer, doe, rabbit, squirrel. This maps one
        ///     onto the other.
        /// </summary>
        public static readonly int[] SpeciesToRow = [2, 1, 0, 3, 4, 5];

        /// <summary>Compass names for the eight headings, in our order.</summary>
        public static readonly string[] Compass = ["N", "NE", "E", "SE", "S", "SW", "W", "NW"];

        /// <summary>A hunter frame: eight headings, three walk frames each.</summary>
        /// <param name="aim">Heading, 0 = N clockwise.</param>
        /// <param name="walkFrame">Walk frame 0-2, as <see cref="HuntGame.WalkCycle" /> orders them.</param>
        public static PixelBuffer Hunter(int aim, int walkFrame) =>
            Art.Dos("hunter", AimToGroup[aim] * 3 + walkFrame + 1);

        /// <summary>
        ///     Which column of a sheet row holds the dead pose.
        ///     <para>
        ///         Five of the six rows are <c>[dead, walk1..3, mirrored walk3..1, mirrored dead]</c>. <b>The
        ///         squirrel's is not</b>: it runs <c>[walk1..3, dead, mirrored walk3..1, mirrored dead]</c>, with the
        ///         dead pose sitting <i>fourth</i> rather than first. Established from the sheet itself — every row is
        ///         a palindrome of four size-matched pairs, and the squirrel's pair up as
        ///         <c>(0,6) (1,5) (2,4) (3,7)</c> where the rest pair as <c>(0,7) (1,6) (2,5) (3,4)</c>. Confirmed by
        ///         eye: id 44 is the squirrel on its back with its legs up, and id 41 is a live one standing.
        ///     </para>
        ///     <para>
        ///         Assuming the common layout for all six put the dead frame into the squirrel's right-facing walk
        ///         cycle — it flipped onto its back every third frame — and made a dead squirrel render as a live one.
        ///     </para>
        /// </summary>
        private static readonly int[] DeadColumnByRow = [0, 0, 0, 0, 0, 3];

        /// <summary>
        ///     A walking animal. Walking left is the mirrored half read backwards, which keeps the cycle in step; that
        ///     half sits at columns 6, 5, 4 whichever layout the row uses. Walking right starts wherever the walk
        ///     frames begin — column 1 normally, column 0 when the dead pose has been pushed to column 3.
        /// </summary>
        /// <param name="species">Species id 0-5, our order.</param>
        /// <param name="frame">Walk frame 0-2.</param>
        /// <param name="facing">Positive faces right, negative faces left.</param>
        public static PixelBuffer Animal(int species, int frame, int facing)
        {
            var row = SpeciesToRow[species];
            var column = facing < 0 ? 6 - frame : (DeadColumnByRow[row] == 0 ? 1 : 0) + frame;
            return Art.Dos("animals", row * 8 + column + 1);
        }

        /// <summary>The dead pose, wherever this species' row happens to keep it.</summary>
        /// <param name="species">Species id 0-5, our order.</param>
        public static PixelBuffer DeadAnimal(int species)
        {
            var row = SpeciesToRow[species];
            return Art.Dos("animals", row * 8 + DeadColumnByRow[row] + 1);
        }

        /// <summary>
        ///     A frame of the ox team's walk. The <c>travelox</c> sheet needs no mapping — frames 1-3 are the cycle in
        ///     order, and 4 and 5 are the wrecked and burning wagons the loss events swap in.
        /// </summary>
        /// <param name="walkFrame">Walk frame 0-2.</param>
        public static PixelBuffer Ox(int walkFrame) => Art.Dos("travelox", walkFrame + 1);

        /// <summary>
        ///     The picture an event paints into the travel screen's sky.
        ///     <para>
        ///         Named from the Apple II's <c>EVENTS.IMA</c>, whose slots the BASIC calls out by number, so these are
        ///         not guesses at what the artist drew: <c>:10605</c> blits <c>I=1</c> for "Severe thunderstorm" and
        ///         <c>I=3</c> for "Severe blizzard", <c>:10705</c> blits 2 for "Hail storm.", <c>:11100</c> blits 6 for
        ///         "Find wild fruit." and <c>:10000</c> blits 8 for "Snow bound". Lining that table up against the DOS
        ///         sheet settles which cloud is which — the one shedding cyan stones is hail and the white-flecked one
        ///         is the blizzard, which is the opposite of what the colours suggest at a glance.
        ///     </para>
        ///     <para>
        ///         The DOS sheet carries seven where the Apple II carries eight: it drops that table's plain and
        ///         burning wagons, which the port keeps on <c>travelox</c> as frames 4 and 5 instead.
        ///     </para>
        /// </summary>
        /// <param name="icon">Which event picture is wanted.</param>
        public static PixelBuffer EventIcon(EventIconEnum icon) => Art.Dos("events", (int) icon);

        /// <summary>
        ///     Where each event picture is blitted, in the Apple II's 280x192 space.
        ///     <para>
        ///         These are <b>not</b> one shared spot — the BASIC gives every event its own coordinates, and putting
        ///         them all in the same place looks wrong immediately: the storms hang at the top of the sky
        ///         (<c>105,0</c> and <c>118,0</c>) while the rest belong down on the ground.
        ///     </para>
        ///     <para>
        ///         <see cref="EventIconEnum.Trader" /> is the one with no coordinate to copy. It has no counterpart in
        ///         the Apple II table at all — that table's remaining slots are the plain and burning wagons, which the
        ///         DOS port keeps on <c>travelox</c> — so it is placed beside the figure it resembles rather than
        ///         given a made-up position of its own.
        ///     </para>
        /// </summary>
        /// <param name="icon">Which event picture is wanted.</param>
        public static (int X, int Y) EventIconSpot(EventIconEnum icon) => icon switch
        {
            EventIconEnum.Thunderstorm => (105, 0),   // :10605, I=1
            EventIconEnum.Blizzard => (105, 0),       // :10605, I=3 -- same call, same spot
            EventIconEnum.HailStorm => (118, 0),      // :10705
            EventIconEnum.WildFruit => (150, 32),     // :11100
            EventIconEnum.SnowBound => (130, 47),     // :10000
            EventIconEnum.Traveller => (263, 35),     // :11330
            _ => (263, 35)
        };

        /// <summary>
        ///     Whether this picture stands on the ground rather than hanging in the sky, in which case only the
        ///     <c>X</c> of <see cref="EventIconSpot" /> is any use.
        ///     <para>
        ///         The listing's <c>Y</c> values cannot be copied across for these, because they are not really
        ///         positions — they are <c>60 - height</c> for the <i>Apple II's</i> sprite, and 60 is where
        ///         <c>:310</c> starts flooding the ground. It comes out exact for three of the four: the burning wagon
        ///         (32+28), the figure (35+25) and the snow drift (47+13) all finish precisely on 60. The DOS art is
        ///         cut tight to its content and is a different size, so reusing those numbers leaves it hanging in
        ///         mid-air — which is exactly what it did.
        ///     </para>
        ///     <para>
        ///         Wild fruit is the one that does not fit the rule: its <c>32+12</c> stops at 44, a good 16 rows shy
        ///         of the ground. Seated here anyway, because a berry bush floating at chest height reads as a bug
        ///         whatever the 1985 listing says, and the DOS bush is a taller sprite than the Apple II's besides.
        ///     </para>
        /// </summary>
        /// <param name="icon">Which event picture is wanted.</param>
        public static bool EventIconStandsOnGround(EventIconEnum icon) =>
            icon is not (EventIconEnum.Thunderstorm or EventIconEnum.Blizzard or EventIconEnum.HailStorm);
    }

    /// <summary>
    ///     The <c>events</c> sheet in cut order. Values are the sprite ids themselves, so the enum can be cast
    ///     straight to one.
    /// </summary>
    public enum EventIconEnum
    {
        /// <summary>Berries on the bush — the Apple II's slot 6, <c>:11100</c> "Find wild fruit."</summary>
        WildFruit = 1,

        /// <summary>A bank of drifted snow — slot 8, <c>:10000</c> "Snow bound".</summary>
        SnowBound = 2,

        /// <summary>Cloud shedding white flakes — slot 3, <c>:10605</c> with <c>I=3</c>, "Severe blizzard".</summary>
        Blizzard = 3,

        /// <summary>Cloud shedding cyan stones — slot 2, <c>:10705</c> "Hail storm."</summary>
        HailStorm = 4,

        /// <summary>Cloud with a yellow bolt — slot 1, <c>:10605</c> with <c>I=1</c>, "Severe thunderstorm".</summary>
        Thunderstorm = 5,

        /// <summary>A figure with a bundle. The Apple II's nearest is slot 7, blitted at <c>:11330</c>.</summary>
        Traveller = 6,

        /// <summary>A figure with goods to hand — the trading counterpart of <see cref="Traveller" />.</summary>
        Trader = 7
    }
}
