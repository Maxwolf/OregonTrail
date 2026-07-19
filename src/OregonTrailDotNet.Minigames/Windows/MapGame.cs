namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>What a landmark is, which is what the map draws it as.</summary>
    public enum MapMarkerEnum
    {
        /// <summary>Independence — a star, labelled START.</summary>
        Start,

        /// <summary>A fort: a hollow square.</summary>
        Fort,

        /// <summary>A named landmark: a filled square.</summary>
        Landmark,

        /// <summary>A river crossing. <b>Unmarked</b> — the map draws nothing for these.</summary>
        River,

        /// <summary>The Willamette Valley — a star, labelled FINISH.</summary>
        Finish
    }

    /// <summary>
    ///     The trail map, transcribed from <c>legacy/source/v1.4-sideA/MAP.LIB</c>. Pure logic; it draws nothing.
    ///     <para>
    ///         <c>MAP.LIB</c> is only three lines, and all of the interesting content lives in <c>VAR.BIN</c>:
    ///     </para>
    ///     <code>
    /// 50000 HCOLOR=4: IF Q1-1 THEN FOR L = 0 TO Q1-2:
    ///         HPLOT MP%(Q(L),0),MP%(Q(L),1) TO MP%(Q(L+1),0),MP%(Q(L+1),1): NEXT
    /// 50010 IF LM &lt;&gt; NM THEN X1=MP%(NM,0):X2=MP%(LM,0):Y1=MP%(NM,1):Y2=MP%(LM,1):
    ///         L=(Y2-Y1)/(X2-X1): Z=D*(X2-X1)/DD: X=Z+X1: Y=Y2-L*(X2-X): HPLOT X2,Y2 TO X,Y
    ///     </code>
    ///     <para>
    ///         So the drawn route is two pieces: a polyline through <c>Q()</c>, the landmarks actually <i>visited</i>,
    ///         and one partial segment for the leg in progress. The partial one is worth reading closely —
    ///         <c>Z = D*(X2-X1)/DD</c> uses <c>D</c>, the distance <b>remaining</b>, so at the start of a leg
    ///         <c>X = X2</c> and the segment has no length, and it reaches the destination exactly as <c>D</c> hits 0.
    ///         The line grows out of the landmark you left.
    ///     </para>
    ///     <para>
    ///         It walks <c>Q()</c> rather than the landmark numbers because <b>the trail forks twice</b> — at South
    ///         Pass, for Fort Bridger or straight on to the Green River, and at the Blue Mountains, for Fort Walla
    ///         Walla or straight to The Dalles. Two runs can draw different maps.
    ///     </para>
    /// </summary>
    public sealed class MapGame
    {
        /// <summary>The DOS map's own size; <c>map.png</c> is a 640x200 one-bit picture.</summary>
        public const int MapWidth = 640;

        /// <summary>Map height; see <see cref="MapWidth" />.</summary>
        public const int MapHeight = 200;

        /// <summary>
        ///     The eighteen landmarks and where they sit on the DOS map.
        ///     <para>
        ///         The Apple II keeps these in <c>MP%</c> inside <c>VAR.BIN</c> — an integer array, so it hides from a
        ///         string/real-only scan the same way <c>L%</c> did — but those are coordinates on the <i>Apple II's</i>
        ///         map, and the DOS one was redrawn rather than rescaled. Fitting a transform between them leaves
        ///         residuals up to 18px, so these are measured off <c>map.png</c> directly instead: the legend gives the
        ///         exact marker glyphs (a fort is a 14x5 hollow rectangle, a landmark an 8x4 solid block — square on
        ///         screen, since a 640-wide mode has half-width pixels), and template-matching those finds 6 forts and
        ///         6 landmarks, whose left-to-right order matches <c>MP%</c>'s exactly. The two stars were read off by
        ///         hand from the pixels.
        ///     </para>
        ///     <para>
        ///         <b>The four river crossings have no marker at all</b> — the map simply does not draw them — so those
        ///         four are the only fitted positions here, placed by the <c>MP%</c>-to-map transform. Nothing is drawn
        ///         at them; they exist because the route line bends there.
        ///     </para>
        /// </summary>
        public static readonly MapLandmark[] Landmarks =
        [
            new("Independence", 578, 148, MapMarkerEnum.Start),
            new("the Kansas River crossing", 565, 153, MapMarkerEnum.River),
            new("the Big Blue River crossing", 546, 137, MapMarkerEnum.River),
            new("Fort Kearney", 503, 134, MapMarkerEnum.Fort),
            new("Chimney Rock", 462, 130, MapMarkerEnum.Landmark),
            new("Fort Laramie", 415, 123, MapMarkerEnum.Fort),
            new("Independence Rock", 372, 111, MapMarkerEnum.Landmark),
            new("South Pass", 338, 117, MapMarkerEnum.Landmark),
            new("Fort Bridger", 305, 136, MapMarkerEnum.Fort),
            new("the Green River crossing", 319, 117, MapMarkerEnum.River),
            new("Soda Springs", 292, 116, MapMarkerEnum.Landmark),
            new("Fort Hall", 257, 108, MapMarkerEnum.Fort),
            new("the Snake River crossing", 217, 111, MapMarkerEnum.River),
            new("Fort Boise", 195, 86, MapMarkerEnum.Fort),
            new("the Blue Mountains", 166, 72, MapMarkerEnum.Landmark),
            new("Fort Walla Walla", 161, 58, MapMarkerEnum.Fort),
            new("The Dalles", 140, 63, MapMarkerEnum.Landmark),
            new("the Willamette Valley", 108, 57, MapMarkerEnum.Finish)
        ];

        /// <summary>
        ///     <c>LM()</c> from <c>VAR.BIN</c>: how long each leg is and where it ends. Indices are route numbers, not
        ///     landmark numbers, which is what makes the forks expressible — routes 7 and 8 both leave South Pass.
        /// </summary>
        public static readonly MapRoute[] Routes =
        [
            new(102, 1), new(83, 2), new(119, 3), new(250, 4), new(86, 5), new(190, 6), new(102, 7),
            new(57, 9),    // South Pass -> Green River, the short way
            new(125, 8),   // South Pass -> Fort Bridger, the long way
            new(162, 10), new(144, 10), new(57, 11), new(182, 12), new(114, 13), new(160, 14),
            new(55, 15),   // Blue Mountains -> Fort Walla Walla
            new(125, 16),  // Blue Mountains -> The Dalles direct
            new(120, 16), new(100, 17)
        ];

        /// <summary>
        ///     <c>LM$(landmark, 1..2)</c>: the one or two routes leaving each landmark. A second entry of -1 means no
        ///     choice; the Willamette Valley's 99 is the original's end-of-trail marker and is -1 here.
        /// </summary>
        public static readonly (int First, int Second)[] Choices =
        [
            (0, -1), (1, -1), (2, -1), (3, -1), (4, -1), (5, -1), (6, -1),
            (7, 8),          // South Pass forks
            (9, -1), (10, -1), (11, -1), (12, -1), (13, -1), (14, -1),
            (15, 16),        // the Blue Mountains fork
            (17, -1), (18, -1), (-1, -1)
        ];

        private readonly List<int> _visited = [];
        private readonly Random _random;

        /// <summary>Initializes a new instance of the <see cref="MapGame" /> class.</summary>
        /// <param name="seed">Fixed seed for a reproducible run, or null to be seeded from the clock.</param>
        public MapGame(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            Reset();
        }

        /// <summary><c>Q()</c> — the landmarks reached so far, in order, starting at Independence.</summary>
        public IReadOnlyList<int> Visited => _visited;

        /// <summary><c>LM</c> — the landmark last reached.</summary>
        public int From { get; private set; }

        /// <summary><c>NM</c> — the landmark being travelled to, or -1 once the trail is done.</summary>
        public int To { get; private set; }

        /// <summary><c>DD</c> — how long this leg is.</summary>
        public double LegMiles { get; private set; }

        /// <summary><c>D</c> — how much of it is left.</summary>
        public double MilesRemaining { get; private set; }

        /// <summary>Total miles walked this run.</summary>
        public double MilesTravelled { get; private set; }

        /// <summary>True once the Willamette Valley has been reached.</summary>
        public bool Finished => To < 0;

        /// <summary>How far along the current leg the party is, 0 to 1. Used to draw the partial segment.</summary>
        public double LegProgress => LegMiles <= 0 ? 0 : (LegMiles - MilesRemaining) / LegMiles;

        /// <summary>
        ///     Which way to go at a fork. Null takes one at random, which is the point of having forks; setting it
        ///     makes a run reproducible so both branches can be looked at.
        /// </summary>
        public bool? PreferSecondAtForks { get; set; }

        /// <summary>Puts the party back at Independence with the whole trail to walk.</summary>
        public void Reset()
        {
            _visited.Clear();
            _visited.Add(0);
            From = 0;
            MilesTravelled = 0;
            Depart();
        }

        /// <summary>Walks a distance, arriving at landmarks and setting off again as it goes.</summary>
        /// <param name="miles">How far to travel.</param>
        public void Advance(double miles)
        {
            while (miles > 0 && !Finished)
            {
                var step = Math.Min(miles, MilesRemaining);
                MilesRemaining -= step;
                MilesTravelled += step;
                miles -= step;

                if (MilesRemaining > 0)
                    continue;

                // Arrived. The landmark joins Q(), which is what the finished part of the route is drawn from.
                From = To;
                _visited.Add(From);
                Depart();
            }
        }

        /// <summary>Chooses the route out of the current landmark and loads the leg.</summary>
        private void Depart()
        {
            var (first, second) = Choices[From];
            if (first < 0)
            {
                To = -1;
                LegMiles = MilesRemaining = 0;
                return;
            }

            var takeSecond = second >= 0 && (PreferSecondAtForks ?? _random.Next(2) == 1);
            var route = Routes[takeSecond ? second : first];
            To = route.Destination;
            LegMiles = MilesRemaining = route.Miles;
        }
    }

    /// <summary>One landmark: its name, where it sits on the DOS map, and how the map marks it.</summary>
    /// <param name="Name">As the game names it.</param>
    /// <param name="X">Column on the 640x200 map.</param>
    /// <param name="Y">Row on the 640x200 map.</param>
    /// <param name="Marker">What is drawn there, if anything.</param>
    public readonly record struct MapLandmark(string Name, int X, int Y, MapMarkerEnum Marker);

    /// <summary>One leg of the trail: how long it is and which landmark it ends at.</summary>
    /// <param name="Miles">Length in miles, from <c>LM(route,0)</c>.</param>
    /// <param name="Destination">Landmark index, from <c>LM(route,2)</c>.</param>
    public readonly record struct MapRoute(int Miles, int Destination);
}
