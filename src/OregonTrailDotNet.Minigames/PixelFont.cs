using WolfCurses.Graphics;

namespace OregonTrailDotNet.Minigames
{
    /// <summary>
    ///     A 5x7 bitmap font drawn straight into a <see cref="PixelBuffer" />.
    ///     <para>
    ///         Text on the tombstone has to be part of the <i>picture</i>, not terminal characters printed near it —
    ///         the original positioned its letters in hi-res pixel coordinates on the stone, so matching that layout
    ///         means being able to place a glyph at an exact (x, y) in the same 280x192 space. Splicing characters
    ///         into an already-rendered ANSI frame would mean walking colour escapes to find a column, and would only
    ///         ever land on whole character cells anyway.
    ///     </para>
    /// </summary>
    public static class PixelFont
    {
        /// <summary>Glyph cell width in pixels, before scaling.</summary>
        public const int GlyphWidth = 5;

        /// <summary>Glyph cell height in pixels, before scaling.</summary>
        public const int GlyphHeight = 7;

        /// <summary>Blank columns between glyphs, before scaling.</summary>
        public const int Tracking = 1;

        /// <summary>Rows are 5 bits wide, most significant bit leftmost. Unknown characters render as a space.</summary>
        private static readonly Dictionary<char, byte[]> Glyphs = new()
        {
            ['A'] = [0x0E, 0x11, 0x11, 0x1F, 0x11, 0x11, 0x11],
            ['B'] = [0x1E, 0x11, 0x11, 0x1E, 0x11, 0x11, 0x1E],
            ['C'] = [0x0E, 0x11, 0x10, 0x10, 0x10, 0x11, 0x0E],
            ['D'] = [0x1E, 0x11, 0x11, 0x11, 0x11, 0x11, 0x1E],
            ['E'] = [0x1F, 0x10, 0x10, 0x1E, 0x10, 0x10, 0x1F],
            ['F'] = [0x1F, 0x10, 0x10, 0x1E, 0x10, 0x10, 0x10],
            ['G'] = [0x0E, 0x11, 0x10, 0x17, 0x11, 0x11, 0x0F],
            ['H'] = [0x11, 0x11, 0x11, 0x1F, 0x11, 0x11, 0x11],
            ['I'] = [0x0E, 0x04, 0x04, 0x04, 0x04, 0x04, 0x0E],
            ['J'] = [0x07, 0x02, 0x02, 0x02, 0x02, 0x12, 0x0C],
            ['K'] = [0x11, 0x12, 0x14, 0x18, 0x14, 0x12, 0x11],
            ['L'] = [0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x1F],
            ['M'] = [0x11, 0x1B, 0x15, 0x15, 0x11, 0x11, 0x11],
            ['N'] = [0x11, 0x19, 0x15, 0x13, 0x11, 0x11, 0x11],
            ['O'] = [0x0E, 0x11, 0x11, 0x11, 0x11, 0x11, 0x0E],
            ['P'] = [0x1E, 0x11, 0x11, 0x1E, 0x10, 0x10, 0x10],
            ['Q'] = [0x0E, 0x11, 0x11, 0x11, 0x15, 0x12, 0x0D],
            ['R'] = [0x1E, 0x11, 0x11, 0x1E, 0x14, 0x12, 0x11],
            ['S'] = [0x0F, 0x10, 0x10, 0x0E, 0x01, 0x01, 0x1E],
            ['T'] = [0x1F, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04],
            ['U'] = [0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x0E],
            ['V'] = [0x11, 0x11, 0x11, 0x11, 0x11, 0x0A, 0x04],
            ['W'] = [0x11, 0x11, 0x11, 0x15, 0x15, 0x1B, 0x11],
            ['X'] = [0x11, 0x11, 0x0A, 0x04, 0x0A, 0x11, 0x11],
            ['Y'] = [0x11, 0x11, 0x0A, 0x04, 0x04, 0x04, 0x04],
            ['Z'] = [0x1F, 0x01, 0x02, 0x04, 0x08, 0x10, 0x1F],
            // Lowercase sits on a five-row x-height with true descenders on g, j, p, q and y. The tombstone is
            // carved capitals and never needed these, but the travel screen's status panel is mixed case and reads
            // wrong without them.
            ['a'] = [0x00, 0x00, 0x0E, 0x01, 0x0F, 0x11, 0x0F],
            ['b'] = [0x10, 0x10, 0x1E, 0x11, 0x11, 0x11, 0x1E],
            ['c'] = [0x00, 0x00, 0x0E, 0x11, 0x10, 0x11, 0x0E],
            ['d'] = [0x01, 0x01, 0x0F, 0x11, 0x11, 0x11, 0x0F],
            ['e'] = [0x00, 0x00, 0x0E, 0x11, 0x1F, 0x10, 0x0E],
            ['f'] = [0x06, 0x09, 0x08, 0x1E, 0x08, 0x08, 0x08],
            ['g'] = [0x00, 0x00, 0x0F, 0x11, 0x0F, 0x01, 0x1E],
            ['h'] = [0x10, 0x10, 0x1E, 0x11, 0x11, 0x11, 0x11],
            ['i'] = [0x04, 0x00, 0x0C, 0x04, 0x04, 0x04, 0x0E],
            ['j'] = [0x02, 0x00, 0x06, 0x02, 0x02, 0x12, 0x0C],
            ['k'] = [0x10, 0x10, 0x12, 0x14, 0x18, 0x14, 0x12],
            ['l'] = [0x0C, 0x04, 0x04, 0x04, 0x04, 0x04, 0x0E],
            ['m'] = [0x00, 0x00, 0x1A, 0x15, 0x15, 0x15, 0x15],
            ['n'] = [0x00, 0x00, 0x1E, 0x11, 0x11, 0x11, 0x11],
            ['o'] = [0x00, 0x00, 0x0E, 0x11, 0x11, 0x11, 0x0E],
            ['p'] = [0x00, 0x00, 0x1E, 0x11, 0x1E, 0x10, 0x10],
            ['q'] = [0x00, 0x00, 0x0F, 0x11, 0x0F, 0x01, 0x01],
            ['r'] = [0x00, 0x00, 0x16, 0x19, 0x10, 0x10, 0x10],
            ['s'] = [0x00, 0x00, 0x0F, 0x10, 0x0E, 0x01, 0x1E],
            ['t'] = [0x08, 0x08, 0x1E, 0x08, 0x08, 0x09, 0x06],
            ['u'] = [0x00, 0x00, 0x11, 0x11, 0x11, 0x13, 0x0D],
            ['v'] = [0x00, 0x00, 0x11, 0x11, 0x11, 0x0A, 0x04],
            ['w'] = [0x00, 0x00, 0x11, 0x11, 0x15, 0x15, 0x0A],
            ['x'] = [0x00, 0x00, 0x11, 0x0A, 0x04, 0x0A, 0x11],
            ['y'] = [0x00, 0x00, 0x11, 0x11, 0x0F, 0x01, 0x1E],
            ['z'] = [0x00, 0x00, 0x1F, 0x02, 0x04, 0x08, 0x1F],

            ['0'] = [0x0E, 0x11, 0x13, 0x15, 0x19, 0x11, 0x0E],
            ['1'] = [0x04, 0x0C, 0x04, 0x04, 0x04, 0x04, 0x0E],
            ['2'] = [0x0E, 0x11, 0x01, 0x02, 0x04, 0x08, 0x1F],
            ['3'] = [0x1F, 0x02, 0x04, 0x02, 0x01, 0x11, 0x0E],
            ['4'] = [0x02, 0x06, 0x0A, 0x12, 0x1F, 0x02, 0x02],
            ['5'] = [0x1F, 0x10, 0x1E, 0x01, 0x01, 0x11, 0x0E],
            ['6'] = [0x06, 0x08, 0x10, 0x1E, 0x11, 0x11, 0x0E],
            ['7'] = [0x1F, 0x01, 0x02, 0x04, 0x08, 0x08, 0x08],
            ['8'] = [0x0E, 0x11, 0x11, 0x0E, 0x11, 0x11, 0x0E],
            ['9'] = [0x0E, 0x11, 0x11, 0x0F, 0x01, 0x02, 0x0C],
            ['.'] = [0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x0C],
            [','] = [0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x0C],
            ['\''] = [0x04, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00],
            ['"'] = [0x0A, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00],
            ['-'] = [0x00, 0x00, 0x00, 0x1F, 0x00, 0x00, 0x00],
            ['!'] = [0x04, 0x04, 0x04, 0x04, 0x04, 0x00, 0x04],
            ['?'] = [0x0E, 0x11, 0x01, 0x02, 0x04, 0x00, 0x04],
            [':'] = [0x00, 0x0C, 0x0C, 0x00, 0x0C, 0x0C, 0x00],
            [';'] = [0x00, 0x0C, 0x0C, 0x00, 0x06, 0x0C, 0x00],
            ['/'] = [0x01, 0x02, 0x02, 0x04, 0x08, 0x08, 0x10],
            ['('] = [0x02, 0x04, 0x08, 0x08, 0x08, 0x04, 0x02],
            [')'] = [0x08, 0x04, 0x02, 0x02, 0x02, 0x04, 0x08],
            ['&'] = [0x0C, 0x12, 0x14, 0x08, 0x15, 0x12, 0x0D],
            ['+'] = [0x00, 0x04, 0x04, 0x1F, 0x04, 0x04, 0x00],
            ['<'] = [0x02, 0x04, 0x08, 0x10, 0x08, 0x04, 0x02],
            ['>'] = [0x08, 0x04, 0x02, 0x01, 0x02, 0x04, 0x08],
            ['='] = [0x00, 0x00, 0x1F, 0x00, 0x1F, 0x00, 0x00],
            ['*'] = [0x00, 0x0A, 0x04, 0x1F, 0x04, 0x0A, 0x00]
        };

        /// <summary>Width in pixels a string occupies when drawn, trailing tracking excluded.</summary>
        public static int Measure(string text, int scale = 1)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return (text.Length * (GlyphWidth + Tracking) - Tracking) * scale;
        }

        /// <summary>Height in pixels a line occupies when drawn.</summary>
        public static int LineHeight(int scale = 1) => GlyphHeight * scale;

        /// <summary>
        ///     Draws a single line, upper-cased, at a pixel position. Anything falling outside the target is clipped,
        ///     so a caller nudging text around never has to bounds-check first.
        /// </summary>
        /// <param name="target">Buffer to draw into.</param>
        /// <param name="text">The line; case is honoured where a glyph exists, unknown characters become blanks.</param>
        /// <param name="x">Left edge of the first glyph.</param>
        /// <param name="y">Top edge of the line.</param>
        /// <param name="color">Colour to set lit pixels to.</param>
        /// <param name="scale">Pixel size multiplier.</param>
        public static void Draw(PixelBuffer target, string text, int x, int y, Rgba32 color, int scale = 1)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var penX = x;
            foreach (var character in text)
            {
                if (TryGlyph(character, out var rows))
                    DrawGlyph(target, rows, penX, y, color, scale);

                penX += (GlyphWidth + Tracking) * scale;
            }
        }

        /// <summary>Draws a line horizontally centred on <paramref name="centerX" />.</summary>
        public static void DrawCentered(PixelBuffer target, string text, int centerX, int y, Rgba32 color,
            int scale = 1)
        {
            Draw(target, text, centerX - Measure(text, scale) / 2, y, color, scale);
        }

        /// <summary>
        ///     Draws a line into a <b>fixed character cell</b> rather than with this font's own spacing — every glyph
        ///     advances by <paramref name="cellWidth" /> whatever its shape.
        ///     <para>
        ///         This is how the original laid text out: it printed into a text window over the picture, and its
        ///         characters sat on a hardware grid (7x8 on the Apple II). Column <i>n</i> therefore lands on an exact
        ///         pixel, which is what makes a layout reproducible rather than eyeballed.
        ///     </para>
        /// </summary>
        /// <param name="target">Buffer to draw into.</param>
        /// <param name="text">The line; case is honoured where a glyph exists, unknown characters become blanks.</param>
        /// <param name="x">Left edge of column 0.</param>
        /// <param name="y">Top edge of the row.</param>
        /// <param name="color">Colour to set lit pixels to.</param>
        /// <param name="cellWidth">Cell width in pixels before scaling; the glyph is centred inside it.</param>
        /// <param name="scale">Pixel size multiplier.</param>
        public static void DrawFixed(PixelBuffer target, string text, int x, int y, Rgba32 color, int cellWidth,
            int scale = 1)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var inset = Math.Max(0, (cellWidth - GlyphWidth) / 2);
            for (var i = 0; i < text.Length; i++)
                if (TryGlyph(text[i], out var rows))
                    DrawGlyph(target, rows, x + (i * cellWidth + inset) * scale, y, color, scale);
        }

        /// <summary>
        ///     Finds a character's glyph, <b>falling back to upper case</b> when it has no lower-case shape of its own.
        ///     Case is honoured rather than folded away, because the travel screen's status panel is mixed case; the
        ///     fallback keeps anything the table only carries as a capital rendering as one instead of vanishing.
        /// </summary>
        private static bool TryGlyph(char character, out byte[] rows) =>
            Glyphs.TryGetValue(character, out rows!) ||
            Glyphs.TryGetValue(char.ToUpperInvariant(character), out rows!);

        private static void DrawGlyph(PixelBuffer target, byte[] rows, int x, int y, Rgba32 color, int scale)
        {
            for (var row = 0; row < GlyphHeight; row++)
            {
                var bits = rows[row];
                for (var column = 0; column < GlyphWidth; column++)
                {
                    // Bit 4 is the leftmost column of the 5-wide cell.
                    if ((bits & (1 << (GlyphWidth - 1 - column))) == 0)
                        continue;

                    for (var dy = 0; dy < scale; dy++)
                    for (var dx = 0; dx < scale; dx++)
                    {
                        var px = x + column * scale + dx;
                        var py = y + row * scale + dy;
                        if (px >= 0 && py >= 0 && px < target.Width && py < target.Height)
                            target.SetPixel(px, py, color);
                    }
                }
            }
        }
    }
}
