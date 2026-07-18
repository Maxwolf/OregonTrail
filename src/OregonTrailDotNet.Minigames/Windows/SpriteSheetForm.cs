using System.Text;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     Contact sheets of the DOS sprites as <c>legacy/tools/dos_sprites.py</c> cut them, in sheet order with the
    ///     mirror toggle for comparison.
    ///     <para>
    ///         This exists to settle assumptions rather than to look pretty. The sheets are packed by hand, not on a
    ///         grid, so the cutter finds sprites by flood-filling content — and the frame <i>order</i> that falls out of
    ///         that is what the hunt's aim and walk mapping is built on. Far easier to confirm by eye than by argument.
    ///     </para>
    /// </summary>
    [ParentWindow(typeof(MinigamesWindow))]
    public sealed class SpriteSheetForm : SceneForm
    {
        private static readonly Sheet[] Sheets =
        [
            new("float — raft, ferry, rocks, shore block (key: water blue)", "float", 9),
            new("hunter — 8 aim directions x 3 walk, in N NE NW E W SE SW S order", "hunter", 24),
            new("animals — 6 species x [dead, walk x3, mirrored walk x3, mirrored dead]", "animals", 48),
            new("terrain — hunting-field scenery (the keypad tiles are filtered out)", "terrain", 14)
        ];

        private int _sheet;
        private bool _mirror;

        /// <summary>Initializes a new instance of the <see cref="SpriteSheetForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public SpriteSheetForm(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override bool Animated => false;

        /// <inheritdoc />
        protected override int ReservedRows => 8;

        /// <inheritdoc />
        protected override void Build()
        {
        }

        /// <inheritdoc />
        protected override void OnSectionKey(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    _sheet = (_sheet - 1 + Sheets.Length) % Sheets.Length;
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.Tab:
                    _sheet = (_sheet + 1) % Sheets.Length;
                    break;
                case ConsoleKey.M:
                    _mirror = !_mirror;
                    break;
                default:
                    return;
            }

            Invalidate();
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var sheet = Sheets[_sheet];
            var frames = sheet.Load();
            if (_mirror)
                frames = frames.Select(Assets.Mirror).ToArray();

            var canvas = Layout(frames, out var columns);

            var text = new StringBuilder();
            text.AppendLine();
            text.AppendLine($"SPRITE SHEET {_sheet + 1}/{Sheets.Length} — {sheet.Title}");
            text.AppendLine(
                $"{frames.Length} frames   {columns} per row, reading order   " +
                $"{(_mirror ? "MIRRORED" : "as cut")}");
            text.AppendLine(Footer("LEFT/RIGHT or TAB sheet   M mirror"));
            text.Append(AnsiImage.FromPixels(canvas).ToAnsi(PictureOptions()));
            return text.ToString();
        }

        /// <summary>Lays frames out in a grid on a mid-grey field, so transparent edges stay visible.</summary>
        private static PixelBuffer Layout(PixelBuffer[] frames, out int columns)
        {
            var widest = frames.Max(f => f.Width);
            var tallest = frames.Max(f => f.Height);
            var cellWidth = widest + 6;
            var cellHeight = tallest + PixelFont.LineHeight() + 8;

            columns = Math.Max(1, (Assets.DosWidth - 4) / cellWidth);
            var rows = (frames.Length + columns - 1) / columns;

            var canvas = new PixelBuffer(columns * cellWidth + 4, rows * cellHeight + 4);
            var field = new Rgba32(48, 48, 56, 255);
            var label = new Rgba32(200, 200, 200, 255);
            for (var y = 0; y < canvas.Height; y++)
            for (var x = 0; x < canvas.Width; x++)
                canvas.SetPixel(x, y, field);

            for (var i = 0; i < frames.Length; i++)
            {
                var column = i % columns;
                var row = i / columns;
                var cellX = 2 + column * cellWidth;
                var cellY = 2 + row * cellHeight;

                // Bottom-aligned in the cell so a walk cycle's feet line up and the animation reads.
                var frame = frames[i];
                canvas.DrawImage(frame, cellX + (cellWidth - frame.Width) / 2, cellY + (tallest - frame.Height));
                PixelFont.DrawCentered(canvas, (i + 1).ToString(), cellX + cellWidth / 2,
                    cellY + tallest + 3, label);
            }

            return canvas;
        }

        /// <summary>One contact sheet: a DOS sheet and how many sprites were cut from it.</summary>
        /// <param name="Title">What the sheet shows.</param>
        /// <param name="Name">Sheet name as the cutter wrote it.</param>
        /// <param name="Count">Sprites cut.</param>
        private sealed record Sheet(string Title, string Name, int Count)
        {
            /// <summary>Loads every frame, in the order the cutter numbered them.</summary>
            public PixelBuffer[] Load() =>
                Enumerable.Range(1, Count).Select(id => Assets.Dos(Name, id)).ToArray();
        }
    }
}
