using WolfCurses.Graphics;

namespace OregonTrailDotNet.Minigames.Windows
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
            Assets.Dos("hunter", AimToGroup[aim] * 3 + walkFrame + 1);

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
            return Assets.Dos("animals", row * 8 + column + 1);
        }

        /// <summary>The dead pose, wherever this species' row happens to keep it.</summary>
        /// <param name="species">Species id 0-5, our order.</param>
        public static PixelBuffer DeadAnimal(int species)
        {
            var row = SpeciesToRow[species];
            return Assets.Dos("animals", row * 8 + DeadColumnByRow[row] + 1);
        }

        /// <summary>
        ///     A frame of the ox team's walk. The <c>travelox</c> sheet needs no mapping — frames 1-3 are the cycle in
        ///     order, and 4 and 5 are the wrecked and burning wagons the loss events swap in.
        /// </summary>
        /// <param name="walkFrame">Walk frame 0-2.</param>
        public static PixelBuffer Ox(int walkFrame) => Assets.Dos("travelox", walkFrame + 1);
    }
}
