namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     One stop of the 1985 original's eighteen-landmark trail, carrying everything the presentation layer needs
    ///     to key off it: card art, tune, map position, travel-screen scenery, and the original's own display name.
    /// </summary>
    /// <param name="Index">The original's landmark index, <c>LM</c> 0-17.</param>
    /// <param name="GameName">The main game's <c>TrailRegistry</c> location name — the lookup key.</param>
    /// <param name="OriginalName">
    ///     <c>LM$</c> verbatim, lowercase article and all (<c>the Kansas River crossing</c>). The card caption's
    ///     <c>Z$</c> strips a leading <c>"the "</c> from this — the game's title-case registry names cannot produce
    ///     the original's mixed-case caption, which is why this column exists.
    /// </param>
    /// <param name="Marker">What the map draws here; <see cref="MapMarkerEnum.River" /> stops draw nothing.</param>
    /// <param name="MapX">X on the DOS <c>map.png</c> (640x200); fitted, not measured, for river crossings.</param>
    /// <param name="MapY">Y on the DOS <c>map.png</c>; see <see cref="MapX" />.</param>
    /// <param name="MusicSlug">
    ///     The stop's tune under <c>music/landmarks/</c>, or null where no tune belongs. Landmark 0 carries its slug
    ///     (<c>00-independence</c>) but the original never plays it on arrival — <c>:1005</c> is guarded by
    ///     <c>IF LM THEN</c>; that tune belongs to the opening.
    /// </param>
    /// <param name="CardArt">The landmark card <c>art/landmarks/p{n}.png</c>, or -1 for stops with no card.</param>
    /// <param name="Mountains">
    ///     Terrain of the leg <b>toward</b> this stop: prairie for destinations LM 1-5 (Kansas River through
    ///     Fort Laramie), mountains from LM 6 (Independence Rock) on — the workbench's <c>PrairieLegs = 5</c>
    ///     reading. (minigames.md's "after Chimney Rock" refers to the card files' disk sides, not the travel legs.)
    /// </param>
    /// <param name="ScenerySpriteId">
    ///     The DOS <c>scenery</c> sprite for the leg toward this stop — the roadside miniature of the landmark
    ///     ahead, from <c>L%</c> — or -1 where no leg leads here (Independence, and the two Dalles branches, whose
    ///     leg reuses The Dalles' own piece).
    /// </param>
    /// <param name="SceneryRestX">
    ///     The original's <c>L%(NM-1,1)</c> resting-offset column, carried <b>for reference only</b> — it was
    ///     authored against the Apple II art and does not position the DOS pieces. The travel screen rests every
    ///     piece at the solved <see cref="TravelGame.SceneryRestX" /> instead (right edge just left of the wagon);
    ///     using this column as a position parks the scenery far short of the team.
    /// </param>
    public sealed record OriginalStop(
        int Index,
        string GameName,
        string OriginalName,
        MapMarkerEnum Marker,
        int MapX,
        int MapY,
        string? MusicSlug,
        int CardArt,
        bool Mountains,
        int ScenerySpriteId,
        int SceneryRestX);

    /// <summary>
    ///     The identity map between the main game's trail and the 1985 original's eighteen landmarks — the one table
    ///     every presentation feature keys off: hunt roster gating and climate zone by <c>LM</c>, map coordinates,
    ///     card art and music by index, and the travel screen's per-leg scenery.
    ///     <para>
    ///         Keyed by <b>location name</b>, never by <c>TrailModule.LocationIndex</c>: the game's location list is
    ///         mutable — chosen fork branches are spliced in at runtime — so indices drift past a taken fork and
    ///         never equal <c>LM</c> beyond South Pass. Names are the stable identity.
    ///     </para>
    ///     <para>
    ///         Lookups fail soft: an unknown name (the debug <c>WinTrail</c>, some future location) returns null and
    ///         the caller degrades — no card, no tune, no marker — rather than throwing mid-game.
    ///     </para>
    /// </summary>
    public static class OriginalTrail
    {
        /// <summary>
        ///     The eighteen stops plus the game's three additions: <c>Oregon City</c> stands where the original ends
        ///     (the Willamette Valley), and The Dalles fork's two spliced branches — the Columbia River and the
        ///     Barlow Toll Road — are real locations the party arrives at, departs from, and hunts from, so they
        ///     carry The Dalles' LM 16 for roster/zone purposes and fitted map points on the Dalles-to-valley line.
        ///     Map coordinates for the eighteen match <see cref="MapGame.Landmarks" /> entry for entry.
        /// </summary>
        private static readonly OriginalStop[] Stops =
        [
            new(0, "Independence", "Independence", MapMarkerEnum.Start, 578, 148, "00-independence", 0, false, -1, 0),
            new(1, "Kansas River Crossing", "the Kansas River crossing", MapMarkerEnum.River, 565, 153, "01-kansas-river", 1, false, 11, 102),
            new(2, "Big Blue River Crossing", "the Big Blue River crossing", MapMarkerEnum.River, 546, 137, "02-big-blue-river", 2, false, 11, 102),
            new(3, "Fort Kearney", "Fort Kearney", MapMarkerEnum.Fort, 503, 134, "03-fort-kearney", 3, false, 3, 130),
            new(4, "Chimney Rock", "Chimney Rock", MapMarkerEnum.Landmark, 462, 130, "04-chimney-rock", 4, false, 10, 144),
            new(5, "Fort Laramie", "Fort Laramie", MapMarkerEnum.Fort, 415, 123, "05-fort-laramie", 5, false, 6, 130),
            new(6, "Independence Rock", "Independence Rock", MapMarkerEnum.Landmark, 372, 111, "06-independence-rock", 6, true, 13, 144),
            new(7, "South Pass", "South Pass", MapMarkerEnum.Landmark, 338, 117, "07-south-pass", 7, true, 14, 124),
            new(8, "Fort Bridger", "Fort Bridger", MapMarkerEnum.Fort, 305, 136, "08-fort-bridger", 8, true, 5, 124),
            new(9, "Green River Crossing", "the Green River crossing", MapMarkerEnum.River, 319, 117, "09-green-river", 9, true, 11, 102),
            new(10, "Soda Springs", "Soda Springs", MapMarkerEnum.Landmark, 292, 116, "10-soda-springs", 10, true, 15, 116),
            new(11, "Fort Hall", "Fort Hall", MapMarkerEnum.Fort, 257, 108, "11-fort-hall", 11, true, 4, 110),
            new(12, "Snake River Crossing", "the Snake River crossing", MapMarkerEnum.River, 217, 111, "12-snake-river", 12, true, 11, 102),
            new(13, "Fort Boise", "Fort Boise", MapMarkerEnum.Fort, 195, 86, "13-fort-boise", 13, true, 7, 124),
            new(14, "Blue Mountains", "the Blue Mountains", MapMarkerEnum.Landmark, 166, 72, "14-blue-mountains", 14, true, 16, 124),
            new(15, "Fort Walla Walla", "Fort Walla Walla", MapMarkerEnum.Fort, 161, 58, "15-fort-walla-walla", 15, true, 8, 124),
            new(16, "The Dalles", "The Dalles", MapMarkerEnum.Landmark, 140, 63, "16-the-dalles", 16, true, 17, 68),

            // The game ends at Oregon City where the original ends at the Willamette Valley; same stop, same card,
            // same tune, and the caption prints the original's name.
            new(17, "Oregon City", "the Willamette Valley", MapMarkerEnum.Finish, 108, 57, "17-willamette-valley", 17, true, 12, 130),

            // The Dalles fork's spliced branches. LM 16 for hunting roster/zone; fitted points on the
            // Dalles-to-valley line (the Barlow road swings south of the river); no marker, card or tune of their
            // own, and their leg to Oregon City reuses The Dalles' scenery treatment.
            new(16, "Columbia River", "the Columbia River", MapMarkerEnum.River, 128, 60, null, -1, true, -1, 0),
            new(16, "Barlow Toll Road", "the Barlow Toll Road", MapMarkerEnum.River, 126, 65, null, -1, true, -1, 0)
        ];

        private static readonly Dictionary<string, OriginalStop> ByName =
            Stops.ToDictionary(stop => stop.GameName, StringComparer.Ordinal);

        /// <summary>All rows, in landmark order with the two spliced branches last.</summary>
        public static IReadOnlyList<OriginalStop> All => Stops;

        /// <summary>
        ///     The stop for a game location name, or null for a name the original trail does not know — callers
        ///     degrade rather than throw.
        /// </summary>
        public static OriginalStop? ForLocation(string? locationName) =>
            locationName != null && ByName.TryGetValue(locationName, out var stop) ? stop : null;

        /// <summary>
        ///     The climate zone <c>ZO = (LM&gt;2)+(LM&gt;5)+(LM&gt;10)+(LM&gt;13)</c> — the hunt's scenery region
        ///     (Missouri woodland, treeless plains, Rockies, sagebrush, Blue Mountains evergreen). Its only effect
        ///     is scenery; the original writes it to <c>$EE6D</c> and gameplay never reads it.
        /// </summary>
        public static int ClimateZone(int lm) => (lm > 2 ? 1 : 0) + (lm > 5 ? 1 : 0) + (lm > 10 ? 1 : 0) + (lm > 13 ? 1 : 0);

        /// <summary>Species 0, the antlered deer, roams <c>LM &gt; 3 AND LM &lt; 13</c> — bison-to-bear country.</summary>
        public static bool AntleredDeerAt(int lm) => lm > 3 && lm < 13;

        /// <summary>Species 1, the bear, roams <c>LM &gt; 6</c> — the mountains.</summary>
        public static bool BearAt(int lm) => lm > 6;

        /// <summary>Species 2, the bison, roams <c>LM &lt; 7</c> — the plains.</summary>
        public static bool BisonAt(int lm) => lm < 7;
    }
}
