using System.Diagnostics;
using WolfCurses.Graphics;
using AssetStore = OregonTrailDotNet.Assets.AssetStore;

namespace OregonTrailDotNet.Minigames
{
    /// <summary>
    ///     The colours the minigames paint with, in one place, so a screen drawn <i>around</i> the DOS art matches the
    ///     art rather than approximating it.
    ///     <para>
    ///         This exists because picking colours by eye went wrong three times, each the same way. The Columbia raft
    ///         got invented grass banks and a shallows band the port never had; the river crossing got a hand-mixed mud
    ///         brown for a bank the port draws in arid sand; the travel screen got an orange ground that is in neither
    ///         palette. Every time, the correct value was already sitting in the sprite sheet being drawn on top of —
    ///         the sheets are colour-keyed, so their key colour <b>is</b> the water and their shore block <b>is</b> the
    ///         sand. Reading the art gives the right answer for free; guessing reliably does not.
    ///     </para>
    ///     <para>
    ///         <b>Every DOS colour below is verified present in <c>PAL.256</c></b> — the 768-byte trailer palette the
    ///         game loads at startup precisely so its 8bpp art has colours — and each cites its index. The static
    ///         constructor re-checks that against the real file whenever it can find one, so a colour cannot quietly
    ///         join the list just by looking plausible.
    ///     </para>
    ///     <para>
    ///         There is also a quick smell test, <see cref="IsDosColour" />: the palette was authored as 6-bit VGA DAC
    ///         values and stored pre-scaled, so every channel of every entry is a multiple of 4. It is worth being
    ///         clear that this is <b>necessary but nowhere near sufficient</b>. Of the three colours that were actually
    ///         wrong here it catches exactly one, pure <c>255,255,255</c>; the mud brown and the orange ground both
    ///         sail through, because a colour mixed by eye lands on a multiple of 4 roughly one time in sixteen and
    ///         those two did. Membership in the palette is the real test — this one is only for a quick look.
    ///     </para>
    /// </summary>
    public static class Palette
    {
        /// <summary>Palette index 0, and the backdrop the port letterboxes its framed scenes against.</summary>
        public static readonly Rgba32 Black = new(0, 0, 0, 255);

        /// <summary>
        ///     The art's white — <c>252,252,252</c>, not <c>255,255,255</c>. The DOS palette tops out at DAC 63, which
        ///     scales to 252, and pure white appears nowhere in any sheet: <c>float</c>, <c>scenery</c>, <c>animals</c>,
        ///     <c>hunter</c>, <c>travelox</c> and the landmark cards contain zero pixels of it between them.
        ///     Palette indices 3, 15, 16 and 255 all hold this.
        /// </summary>
        public static readonly Rgba32 White = new(252, 252, 252, 255);

        /// <summary>
        ///     River and lake water; palette index 133. This is <c>float.png</c>'s own key colour — the sheet is keyed
        ///     on it because the water was drawn by the game rather than blitted — so it is the water by definition.
        /// </summary>
        public static readonly Rgba32 Water = new(64, 176, 252, 255);

        /// <summary>
        ///     Arid ground: river banks, the Columbia's shores, and dry country generally; palette index 199. Taken
        ///     from the shore block <c>float.png</c> ships, and confirmed against captures of the port — its banks are
        ///     bare sand, with no grass and no shallows, meeting the water on one hard edge.
        /// </summary>
        public static readonly Rgba32 Sand = new(252, 184, 144, 255);

        /// <summary>Living ground on the plains; palette index 107.</summary>
        public static readonly Rgba32 Grass = new(4, 156, 0, 255);

        /// <summary>Snow cover, which is simply <see cref="White" /> — the port has no separate snow tone.</summary>
        public static readonly Rgba32 Snow = White;

        /// <summary>Fully transparent, for keying a colour out of a sprite.</summary>
        public static readonly Rgba32 Clear = new(0, 0, 0, 0);

        /// <summary>
        ///     The Apple II's six hi-res colours, for the few screens still drawn over 1985 artwork — the tombstone
        ///     is one. Matching <c>legacy/tools/apple2_art.py</c>, which is what decoded those PNGs.
        ///     <para>
        ///         Keep these strictly apart from the DOS colours above. They are a different machine's palette and
        ///         they do <b>not</b> agree: Apple II white is a true <c>255,255,255</c> where the DOS art's is
        ///         <c>252,252,252</c>. Either is right over its own artwork and wrong over the other's, so the thing
        ///         that decides is which PNG is underneath, not which looks better in isolation.
        ///     </para>
        /// </summary>
        public static class Apple2
        {
            /// <summary>Unlit pixel.</summary>
            public static readonly Rgba32 Black = new(0, 0, 0, 255);

            /// <summary>Adjacent lit pixels read as white — a true 255, unlike the DOS palette's.</summary>
            public static readonly Rgba32 White = new(255, 255, 255, 255);

            /// <summary>Even column, high bit clear.</summary>
            public static readonly Rgba32 Violet = new(255, 68, 253, 255);

            /// <summary>Odd column, high bit clear.</summary>
            public static readonly Rgba32 Green = new(20, 245, 60, 255);

            /// <summary>Even column, high bit set.</summary>
            public static readonly Rgba32 Blue = new(20, 207, 253, 255);

            /// <summary>Odd column, high bit set.</summary>
            public static readonly Rgba32 Orange = new(255, 106, 60, 255);
        }

        /// <summary>
        ///     Workbench chrome, kept deliberately apart from the colours above: contact-sheet backing, captions and
        ///     rules are <b>not</b> from either palette and should not be. They are meant to read as the harness
        ///     rather than as the game, so that a sprite's own edges stay obvious against them.
        /// </summary>
        public static class Chrome
        {
            /// <summary>Backing field for contact sheets — mid-grey, so transparent sprite edges stay visible.</summary>
            public static readonly Rgba32 Field = new(48, 48, 56, 255);

            /// <summary>Captions under sprite cells.</summary>
            public static readonly Rgba32 Label = new(200, 200, 200, 255);

            /// <summary>Measuring rules and guides drawn over a scene under inspection.</summary>
            public static readonly Rgba32 Rule = new(255, 96, 0, 255);
        }

        /// <summary>
        ///     Whether a colour could have come from the DOS palette, by the multiple-of-4 signature described on the
        ///     class. A good deal more useful than comparing screenshots by eye, and it costs one modulo.
        /// </summary>
        /// <param name="colour">The colour to check.</param>
        public static bool IsDosColour(Rgba32 colour) =>
            colour.R % 4 == 0 && colour.G % 4 == 0 && colour.B % 4 == 0;

        /// <summary>
        ///     Holds the DOS colours to the real palette, once, at first use.
        ///     <para>
        ///         A predicate nobody calls prevents nothing, and <see cref="IsDosColour" /> on its own is too weak to
        ///         lean on — it waves through most hand-mixed colours. So this reads <c>PAL.256</c> itself and checks
        ///         membership, which is the property actually claimed above. Debug-only: it guards a mistake made while
        ///         authoring this file, not a runtime condition.
        ///     </para>
        ///     <para>
        ///         Silently skipped when the palette resource is absent. The workbench is expected to run without it — the
        ///         art loader says so by handing back a missing-texture checkerboard rather than throwing — and a
        ///         verification step must not be the thing that makes a working setup fail.
        ///     </para>
        /// </summary>
        static Palette() => VerifyAgainstPal256();

        [Conditional("DEBUG")]
        private static void VerifyAgainstPal256()
        {
            var bytes = AssetStore.Bytes("data/PAL.256");
            if (bytes == null)
                return;

            // The palette is the last 768 bytes, behind the 0x0C marker that introduces a PCX 256-colour trailer.
            if (bytes.Length < 769 || bytes[^769] != 0x0C)
                return;

            var start = bytes.Length - 768;
            var entries = new HashSet<(byte, byte, byte)>();
            for (var i = 0; i < 256; i++)
                entries.Add((bytes[start + i * 3], bytes[start + i * 3 + 1], bytes[start + i * 3 + 2]));

            foreach (var (name, colour) in new (string, Rgba32)[]
                     {
                         (nameof(Black), Black), (nameof(White), White), (nameof(Water), Water),
                         (nameof(Sand), Sand), (nameof(Grass), Grass)
                     })
                Debug.Assert(entries.Contains((colour.R, colour.G, colour.B)),
                    $"Palette.{name} is {colour.R},{colour.G},{colour.B}, which is not in PAL.256 — so it is not a " +
                    "colour this game can draw, and was almost certainly mixed by eye. Read it off the sprite sheet " +
                    "being drawn on, or off PAL.256, rather than matching it against a screenshot.");
        }
    }
}
