using System.Text;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     The tombstone, laid out the way the original laid it out.
    ///     <para>
    ///         <c>TOMB.LIB</c> does not position letters at all — it opens a <i>text window</i> on the picture and
    ///         prints into it (<c>&amp; DFW,6 AT 56,62,150,45</c> then <c>&amp; WIND,6</c>, in <c>INVERSE</c> so the
    ///         glyphs come out dark on the blank white panel the artist drew on the stone). The window is 150x45 over a
    ///         7x8 character cell, which is exactly <b>21 columns by 5 rows</b>; the epitaph is capped at 29 characters
    ///         (<c>&amp; INP,29,"-09-AZ-az ,.'-"</c>), so it wraps to two rows and the four-line inscription —
    ///         "Here lies", the name, a blank, then the epitaph — fills the window precisely.
    ///     </para>
    ///     <para>
    ///         The one clever bit: after printing the header the original calls <c>&amp; GCP,X,Y</c> to remember which
    ///         row the epitaph starts on, so an edit can reprint just that row (<c>&amp; CO,,Y</c> at <c>:50030</c>)
    ///         without redrawing the rest. That row is reported below as "epitaph row".
    ///     </para>
    ///     <para>
    ///         <b>The window's size is the original's; its origin is not.</b> The inscription is placed at
    ///         <c>60,67</c>, measured against the extracted art rather than taken from the BASIC's <c>56,62</c>.
    ///         An earlier claim that <c>56,62</c> had been "verified by overlay" was worthless: the whole stone reads
    ///         as white, so <i>every</i> placement lands on white and the test discriminated nothing. What actually
    ///         bounds the text is the <b>smooth</b> carved face — solid white as against the dithered stone texture —
    ///         which measures about <c>x=50..209</c> across the rows the inscription uses. The 150-wide window
    ///         therefore wants to start at 60, landing it at <c>60..210</c> and matching the face on both sides; a
    ///         full 21-column row is the case that catches a bad origin, since short epitaphs never reach the edge.
    ///     </para>
    /// </summary>
    [ParentWindow(typeof(MinigamesWindow))]
    public sealed class TombstoneForm : SceneForm
    {
        // & DFW,6 AT 56,62,150,45 — TOMB.LIB:50010 and :50105 use the identical rectangle. The *size* is the
        // original's and is kept exactly, because 150x45 over a 7x8 cell is what makes the 21x5 grid and the
        // 29-character cap agree.
        //
        // The origin is not the original's, and was not chosen by eye either. The carved face is an irregular blob,
        // not a rectangle: it narrows toward the top, so the "Here lies" row wants the block pushed right while a
        // full 21-column epitaph row wants it pushed left. Eyeballing satisfies whichever row you happen to be
        // looking at. So it was solved instead — sweep every origin, render all four samples, and count glyph pixels
        // landing on dark stone. 63,69 is the minimum at 8 stray pixels of 4655 (0.2%), against 48 at 60,67, 96 at
        // 81,67 and 180 at the BASIC's own 56,62. Do not "correct" this back to the literal.
        private const int Apple2WindowX = 56, Apple2WindowY = 62;
        private const int WindowX = 63, WindowY = 69, WindowW = 150, WindowH = 45;

        // Apple II hi-res character cell. 150/7 = 21 columns, 45/8 = 5 rows.
        private const int CellW = 7, CellH = 8;

        // & INP,29,"-09-AZ-az ,.'-",0,A$ — the epitaph's length cap and its allowed alphabet.
        private const int EpitaphLimit = 29;

        private static readonly (string Name, string Epitaph)[] Samples =
        [
            ("Andy", "Peace at last"),
            ("William", "Died of dysentery"),
            ("Maxwolf", "He forded one river too many"),
            ("Bartholomew", "Rest in peace, we ate the oxen"),
            ("Ann", "")
        ];

        // Apple II inks, not DOS ones: this screen is drawn over 1985 artwork, whose white is a true 255. The grey
        // is not a hi-res colour at all -- the Apple II cannot make one -- it is here to test legibility.
        private static readonly (string Name, Rgba32 Colour)[] Inks =
        [
            ("black (INVERSE on the white panel)", Palette.Apple2.Black),
            ("white", Palette.Apple2.White),
            ("chisel grey (not a hi-res colour)", new Rgba32(72, 72, 72, 255))
        ];

        private PixelBuffer _stone = null!;
        private int _x = WindowX;
        private int _y = WindowY;
        private int _sample;
        private int _ink;
        private bool _grid;

        /// <summary>Initializes a new instance of the <see cref="TombstoneForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public TombstoneForm(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override bool Animated => false;

        /// <inheritdoc />
        protected override int ReservedRows => 11;

        /// <summary>
        ///     <i>Taps</i>, which is what the original plays here — <c>TOMB.LIB:50010</c> loads the stone and the
        ///     score together (<c>&amp; DUN,"TS.PCK"</c> then <c>&amp; RFL,"TS.BIN"</c>) before printing the
        ///     inscription and waiting.
        ///     <para>
        ///         The Apple II's, necessarily. This is the one screen where the workbench cannot use the 1990 port's
        ///         assets, and it is the same reason in both media: <c>OTMCGA.PCL</c> has no tombstone picture because
        ///         the DOS port draws that screen with BGI primitives, and <c>SONGS.TXT</c> has no nineteenth line
        ///         because it carries the eighteen landmark tunes and nothing else. <b>The DOS port has no graveyard
        ///         music at all.</b>
        ///     </para>
        /// </summary>
        protected override string? MusicCue => "apple2/ts-tombstone";

        // The DOS port has no tombstone bitmap — neither picture library holds one, because it draws that screen with
        // BGI primitives — so this is the one screen that stays on the 1985 card.
        protected override void Build() => _stone = Assets.Apple2Backdrop("ts-tombstone.png");

        /// <inheritdoc />
        protected override void OnSectionKey(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.LeftArrow: _x--; break;
                case ConsoleKey.RightArrow: _x++; break;
                case ConsoleKey.UpArrow: _y--; break;
                case ConsoleKey.DownArrow: _y++; break;
                case ConsoleKey.Tab: _sample = (_sample + 1) % Samples.Length; break;
                case ConsoleKey.C: _ink = (_ink + 1) % Inks.Length; break;
                case ConsoleKey.G: _grid = !_grid; break;
                case ConsoleKey.Home:
                    _x = WindowX;
                    _y = WindowY;
                    break;
                default:
                    return;
            }

            Invalidate();
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var canvas = new PixelBuffer(_stone.Width, _stone.Height, (byte[]) _stone.Data.Clone());
            var columns = WindowW / CellW;
            var rows = WindowH / CellH;
            var (name, epitaph) = Samples[_sample];

            if (_grid)
                DrawGrid(canvas, columns, rows);

            // The inscription exactly as TOMB.LIB prints it: "Here lies", the name, a blank line, then the epitaph.
            var lines = new List<string> { "Here lies", name, string.Empty };
            var epitaphRow = lines.Count;
            lines.AddRange(Wrap(Trim(epitaph), columns));

            for (var row = 0; row < lines.Count && row < rows; row++)
                PixelFont.DrawFixed(canvas, lines[row], _x, _y + row * CellH, Inks[_ink].Colour, CellW);

            var overflowRows = Math.Max(0, lines.Count - rows);
            var text = new StringBuilder();
            text.AppendLine();
            text.AppendLine("TOMBSTONE — the original opens a text window on the picture and prints into it:");
            text.AppendLine("  & DFW,6 AT 56,62,150,45 : & WIND,6 : INVERSE     (TOMB.LIB:50010, :50105)");
            text.AppendLine(
                $"window {_x},{_y} {WindowW}x{WindowH}   cell {CellW}x{CellH}   {columns} cols x {rows} rows   " +
                $"epitaph row {epitaphRow}   " +
                $"{(_x == WindowX && _y == WindowY ? $"AT THE TUNED ORIGIN (BASIC says {Apple2WindowX},{Apple2WindowY})" : "moved")}");
            text.AppendLine(
                $"epitaph {Trim(epitaph).Length}/{EpitaphLimit} chars, wraps to {Wrap(Trim(epitaph), columns).Count} row(s)   " +
                $"ink {Inks[_ink].Name}" + (overflowRows > 0 ? $"   !! {overflowRows} row(s) OVERFLOW the window" : ""));
            text.Append(AnsiImage.FromPixels(canvas).ToAnsi(PictureOptions()));
            text.AppendLine();
            text.AppendLine($"        HERE LIES {name.ToUpperInvariant()}");
            text.Append(Footer("ARROWS move window   G cell grid   TAB sample   C ink   HOME original origin"));
            return text.ToString();
        }

        /// <summary>Clips an epitaph to the length the original's input routine allowed.</summary>
        private static string Trim(string epitaph) =>
            epitaph.Length <= EpitaphLimit ? epitaph : epitaph[..EpitaphLimit];

        /// <summary>Word-wraps into the window's column count, which is what the text window did for free.</summary>
        private static List<string> Wrap(string text, int columns)
        {
            var wrapped = new List<string>();
            if (string.IsNullOrWhiteSpace(text))
                return wrapped;

            var line = new StringBuilder();
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

        /// <summary>Draws the character cells, which is what makes a position checkable rather than eyeballed.</summary>
        private void DrawGrid(PixelBuffer canvas, int columns, int rows)
        {
            var line = Palette.Chrome.Rule;
            for (var column = 0; column <= columns; column++)
            for (var y = 0; y <= rows * CellH; y++)
                Plot(canvas, _x + column * CellW, _y + y, line);

            for (var row = 0; row <= rows; row++)
            for (var x = 0; x <= columns * CellW; x++)
                Plot(canvas, _x + x, _y + row * CellH, line);
        }

        private static void Plot(PixelBuffer canvas, int x, int y, Rgba32 colour)
        {
            if (x >= 0 && y >= 0 && x < canvas.Width && y < canvas.Height)
                canvas.SetPixel(x, y, colour);
        }
    }
}
