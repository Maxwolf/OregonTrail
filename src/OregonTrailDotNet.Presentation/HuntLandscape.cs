namespace OregonTrailDotNet.Presentation
{
    /// <summary>One piece of scenery: which sprite it is, and where it stands.</summary>
    /// <param name="SpriteId">1-based id on the DOS <c>terrain</c> sheet.</param>
    /// <param name="X">Left edge in field pixels.</param>
    /// <param name="Y">Top edge in field pixels.</param>
    public readonly record struct ScenicProp(int SpriteId, int X, int Y);

    /// <summary>
    ///     The hunting ground, generated the way the 1985 original generates it.
    ///     <para>
    ///         This is <b>recovered, not invented</b>. The routine is <c>$E2A7</c> in the hunt's language-card blob
    ///         (v1.4 side A), called once from the setup at <c>$E6B5</c>. In full:
    ///     </para>
    ///     <code>
    ///     $E2B1  base  = arg[6] * 6              ; arg 6 is ZO, the climate zone
    ///     $E2BC  count = RND(0..3) + 4           ; 4 to 7 pieces
    ///     $E2D3  idx   = table[base + RND(0..5)] ; the zone's own six-entry window
    ///     $E2DD  id    = idx + 40                ; -> one of the 15 scenery images
    ///     $E30C  x     = RND(0..slots-1) * 14    ; slots = (24 - width)/2 + 1
    ///     $E32E  y     = RND(0..191 - height)
    ///     $E33E  if it hits the hunter or anything already placed -> re-roll the position
    ///     $E373  repeat until count pieces are down
    ///     </code>
    ///     <para>
    ///         <b>The ground is regional.</b> <c>ZO</c> — which <c>HUNT.LIB:50011</c> passes straight through as the
    ///         sixth argument to <c>&amp; HUNT</c>, and which the trail computes as
    ///         <c>(LM&gt;2)+(LM&gt;5)+(LM&gt;10)+(LM&gt;13)</c> — indexes a 30-byte table at <c>$E04A</c>, five zones of
    ///         six entries each. The fifteen scenery images are five families of three, and each zone draws from two of
    ///         them (sometimes the same one twice, which narrows it to one). The result tracks the real country:
    ///         woodland in Missouri, a <b>treeless</b> high plains, conifers and boulders in the Rockies, sagebrush on
    ///         the Snake River plain, and solid evergreen over the Blue Mountains.
    ///     </para>
    ///     <para>
    ///         Two details are the Apple II showing through. <b>x lands on a 14-pixel grid</b> — two byte columns —
    ///         which is what keeps a hi-res sprite's NTSC colour phase intact; y is unquantised because vertical
    ///         position does not affect colour. And the re-roll has <b>no attempt limit</b>: with at most seven pieces
    ///         on an open field it always terminates, but it is a spin, not a bounded search.
    ///     </para>
    /// </summary>
    public sealed class HuntLandscape
    {
        /// <summary>
        ///     The table at <c>$E04A</c>, verbatim: five zones, six scenery indices each. It is exactly 30 bytes —
        ///     reading a sixth row runs straight into the species table at <c>$E068</c>, which is how the length is
        ///     known rather than guessed.
        /// </summary>
        public static readonly byte[][] ZoneScenery =
        [
            [0, 1, 2, 6, 7, 8],       // broadleaf + scrub
            [6, 7, 8, 6, 7, 8],       // scrub only
            [3, 4, 5, 12, 13, 14],    // conifers + boulders
            [9, 10, 11, 12, 13, 14],  // flowering shrubs + boulders
            [3, 4, 5, 3, 4, 5]        // conifers only
        ];

        /// <summary>What each zone is, for the readout.</summary>
        public static readonly string[] ZoneNames =
        [
            "Missouri woodland (LM 0-2)",
            "high plains, treeless (LM 3-5)",
            "Rockies (LM 6-10)",
            "Snake River sage (LM 11-13)",
            "Blue Mountains evergreen (LM 14+)"
        ];

        /// <summary>
        ///     The original's fifteen scenery images mapped onto the DOS <c>terrain</c> sheet, family by family:
        ///     broadleaf, conifer, scrub, flowering, boulders. The DOS sheet carries only <b>two</b> boulder clusters
        ///     against the Apple II's three, so index 14 repeats index 12; everything else is one-for-one.
        /// </summary>
        public static readonly int[] SceneryToDos =
        [
            1, 9, 10,     // broadleaf trees
            2, 4, 11,     // conifers
            3, 5, 7,      // low scrub and grass
            6, 8, 12,     // flowering shrubs
            13, 14, 13    // boulders -- only two exist, so the third repeats
        ];

        /// <summary>`$E30C` — the horizontal grid the original snaps to, in pixels.</summary>
        private const int GridX = 14;

        /// <summary>The hunter's frame, kept clear because `$E39D` tests every candidate against him first.</summary>
        private const int HunterWidth = 21, HunterHeight = 28;

        private HuntLandscape(int seed, int zone, IReadOnlyList<ScenicProp> props)
        {
            Seed = seed;
            Zone = zone;
            Props = props;
        }

        /// <summary>The seed this ground grew from. Shown on screen so a good one can be asked for again.</summary>
        public int Seed { get; }

        /// <summary>Which climate zone it was grown for, 0-4.</summary>
        public int Zone { get; }

        /// <summary>What this zone's country is called.</summary>
        public string ZoneName => ZoneNames[Zone];

        /// <summary>
        ///     Everything standing on it, <b>ordered by foot position</b> so anything nearer the bottom paints later
        ///     and therefore in front. The original blits in placement order and does not sort — it does not need to,
        ///     since nothing may overlap anything else.
        /// </summary>
        public IReadOnlyList<ScenicProp> Props { get; }

        /// <summary>
        ///     Grows a ground for a zone, following <c>$E2A7</c>.
        /// </summary>
        /// <param name="seed">Seed to grow from; the same seed and zone always give the same ground.</param>
        /// <param name="zone">Climate zone 0-4, the original's <c>ZO</c>.</param>
        /// <param name="width">Field width. The original's is 24 byte columns (168px); this field is wider, so the
        ///     count of horizontal slots differs while the 14-pixel grid does not.</param>
        /// <param name="height">Field height.</param>
        /// <param name="hunterX">Where the hunter stands, kept clear.</param>
        /// <param name="hunterY">Where the hunter stands, kept clear.</param>
        public static HuntLandscape Generate(int seed, int zone, int width, int height, int hunterX, int hunterY)
        {
            var random = new Random(seed);
            var window = ZoneScenery[Math.Clamp(zone, 0, ZoneScenery.Length - 1)];
            var props = new List<ScenicProp>();

            // $E39D tests the hunter first, then every piece already down, so he goes in as the first obstacle.
            var placed = new List<(int X, int Y, int W, int H)> { (hunterX, hunterY, HunterWidth, HunterHeight) };

            var count = random.Next(0, 4) + 4;      // $E2BC: RND(0..3) + 4
            for (var piece = 0; piece < count; piece++)
            {
                // $E2D3: the image is chosen once, from this zone's window...
                var spriteId = SceneryToDos[window[random.Next(window.Length)]];
                var sprite = Art.Dos("terrain", spriteId);
                var slots = Math.Max(1, (width - sprite.Width) / GridX + 1);

                // ...and only the position is re-rolled on a clash ($E341 branches back to $E30C, not $E2D3).
                // The original spins here without a cap; this stops instead of hanging if a field is ever too full.
                for (var attempt = 0; attempt < 200; attempt++)
                {
                    var x = random.Next(slots) * GridX;
                    var y = random.Next(0, Math.Max(1, height - sprite.Height));

                    if (placed.Any(box => x < box.X + box.W && box.X < x + sprite.Width &&
                                          y < box.Y + box.H && box.Y < y + sprite.Height))
                        continue;

                    placed.Add((x, y, sprite.Width, sprite.Height));
                    props.Add(new ScenicProp(spriteId, x, y));
                    break;
                }
            }

            props.Sort((a, b) =>
                (a.Y + Art.Dos("terrain", a.SpriteId).Height)
                .CompareTo(b.Y + Art.Dos("terrain", b.SpriteId).Height));

            return new HuntLandscape(seed, zone, props);
        }
    }
}
