using WolfCurses.Graphics;

namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     The tombstone inscription, laid out the way the original laid it out.
    ///     <para>
    ///         <c>TOMB.LIB</c> does not position letters at all — it opens a <i>text window</i> on the picture and
    ///         prints into it (<c>&amp; DFW,6 AT 56,62,150,45</c> then <c>&amp; WIND,6</c>, in <c>INVERSE</c> so the
    ///         glyphs come out dark on the blank white panel the artist drew on the stone). The window is 150x45 over
    ///         a 7x8 character cell, which is exactly <b>21 columns by 5 rows</b>; the epitaph is capped at 29
    ///         characters (<c>&amp; INP,29,"-09-AZ-az ,.'-"</c>), so it wraps to two rows and the four-line
    ///         inscription — "Here lies", the name, a blank, then the epitaph — fills the window precisely.
    ///     </para>
    /// </summary>
    public static class TombstoneArt
    {
        /// <summary>The BASIC's own window origin (<c>&amp; DFW,6 AT 56,62,150,45</c>), kept for reference only.</summary>
        public const int Apple2WindowX = 56;

        /// <inheritdoc cref="Apple2WindowX" />
        public const int Apple2WindowY = 62;

        // The *size* is the original's and is kept exactly, because 150x45 over a 7x8 cell is what makes the 21x5
        // grid and the 29-character cap agree.
        //
        // The origin is not the original's, and was not chosen by eye either. The carved face is an irregular blob,
        // not a rectangle: it narrows toward the top, so the "Here lies" row wants the block pushed right while a
        // full 21-column epitaph row wants it pushed left. Eyeballing satisfies whichever row you happen to be
        // looking at. So it was solved instead — sweep every origin, render all four samples, and count glyph pixels
        // landing on dark stone. 63,69 is the minimum at 8 stray pixels of 4655 (0.2%), against 48 at 60,67, 96 at
        // 81,67 and 180 at the BASIC's own 56,62. Do not "correct" this back to the literal.

        /// <summary>Tuned window origin on the extracted 280x192 stone; see the comment above for why not 56,62.</summary>
        public const int WindowX = 63;

        /// <inheritdoc cref="WindowX" />
        public const int WindowY = 69;

        /// <summary>The text window's size, the original's exactly.</summary>
        public const int WindowW = 150;

        /// <inheritdoc cref="WindowW" />
        public const int WindowH = 45;

        /// <summary>Apple II hi-res character cell. 150/7 = 21 columns, 45/8 = 5 rows.</summary>
        public const int CellW = 7;

        /// <inheritdoc cref="CellW" />
        public const int CellH = 8;

        /// <summary><c>&amp; INP,29,"-09-AZ-az ,.'-",0,A$</c> — the original epitaph's length cap.</summary>
        public const int EpitaphLimit = 29;

        /// <summary>Columns the text window holds.</summary>
        public static int Columns => WindowW / CellW;

        /// <summary>Rows the text window holds.</summary>
        public static int Rows => WindowH / CellH;

        /// <summary>Clips an epitaph to the length the original's input routine allowed.</summary>
        public static string Trim(string epitaph) =>
            epitaph.Length <= EpitaphLimit ? epitaph : epitaph[..EpitaphLimit];

        /// <summary>Word-wraps into the window's column count, which is what the text window did for free.</summary>
        public static List<string> Wrap(string text, int columns)
        {
            var wrapped = new List<string>();
            if (string.IsNullOrWhiteSpace(text))
                return wrapped;

            var line = new System.Text.StringBuilder();
            foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Length > 0 && line.Length + 1 + word.Length > columns)
                {
                    wrapped.Add(line.ToString());
                    line.Clear();
                }

                if (line.Length > 0)
                    line.Append(' ');

                line.Append(word);
            }

            if (line.Length > 0)
                wrapped.Add(line.ToString());

            return wrapped;
        }

        /// <summary>
        ///     The inscription exactly as <c>TOMB.LIB</c> prints it: "Here lies", the name, a blank line, then the
        ///     wrapped epitaph. <paramref name="epitaphRow" /> is the row the epitaph starts on — the value the
        ///     original remembers with <c>&amp; GCP</c> so an edit can reprint just that row.
        /// </summary>
        public static List<string> Inscription(string name, string epitaph, out int epitaphRow)
        {
            var lines = new List<string> { "Here lies", name, string.Empty };
            epitaphRow = lines.Count;
            lines.AddRange(Wrap(Trim(epitaph), Columns));
            return lines;
        }

        /// <summary>
        ///     Draws inscription rows into the text window at the given origin, clipped to the window's five rows the
        ///     way the original's text window clipped for free.
        /// </summary>
        public static void Draw(PixelBuffer canvas, IReadOnlyList<string> lines, int x, int y, Rgba32 ink)
        {
            for (var row = 0; row < lines.Count && row < Rows; row++)
                PixelFont.DrawFixed(canvas, lines[row], x, y + row * CellH, ink, CellW);
        }

        /// <summary>
        ///     The one-call version: clones the stone and returns it carrying the inscription at the tuned origin.
        /// </summary>
        /// <param name="stone">The 280x192 Apple II tombstone card; never mutated.</param>
        /// <param name="name">The dead party leader's name.</param>
        /// <param name="epitaph">The epitaph, clipped and wrapped as the original would.</param>
        /// <param name="ink">The glyph colour — the authentic choice is <see cref="Palette.Apple2.Black" />.</param>
        public static PixelBuffer Inscribe(PixelBuffer stone, string name, string epitaph, Rgba32 ink)
        {
            var canvas = new PixelBuffer(stone.Width, stone.Height, (byte[]) stone.Data.Clone());
            Draw(canvas, Inscription(name, epitaph, out _), WindowX, WindowY, ink);
            return canvas;
        }
    }
}
