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
        /// <summary>
        ///     Ticks each frame is held, matching <c>HuntGame</c>'s own walk timer so the cycles run at the speed they
        ///     do in play. <c>-</c>/<c>+</c> still scale the whole section if a closer look is wanted.
        /// </summary>
        private const int TicksPerFrame = 4;

        private static readonly Sheet[] Sheets =
        [
            new("float — raft, ferry, rocks, shore block (key: water blue)", "float", 9),
            new("hunter — 8 aim directions x 3 walk, in N NE NW E W SE SW S order", "hunter", 24),
            new("animals — 6 species x [dead, walk x3, mirrored walk x3, mirrored dead]", "animals", 48),
            new("terrain — hunting-field scenery (the keypad tiles are filtered out)", "terrain", 14),
            new("animations — every walk cycle at once, running the game's own frame maps", "", 0, true)
        ];

        private int _sheet;
        private bool _mirror;
        private int _tick;

        /// <summary>Initializes a new instance of the <see cref="SpriteSheetForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public SpriteSheetForm(IWindow window) : base(window)
        {
        }

        /// <summary>Only the animation page moves; a contact sheet is a still and recomposes when poked.</summary>
        protected override bool Animated => Sheets[_sheet].Animation;

        /// <inheritdoc />
        protected override int ReservedRows => 8;

        /// <inheritdoc />
        protected override void Build()
        {
        }

        /// <inheritdoc />
        protected override void Advance() => _tick++;

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
            var text = new StringBuilder();
            text.AppendLine();
            text.AppendLine($"SPRITE SHEET {_sheet + 1}/{Sheets.Length} — {sheet.Title}");

            if (sheet.Animation)
            {
                var cycles = BuildCycles();
                var step = _tick / TicksPerFrame;
                text.AppendLine(
                    $"{cycles.Length} cycles   frame {step % 4 + 1} of each   " +
                    $"1 frame per {TicksPerFrame} ticks, the hunt's own walk cadence");
                text.AppendLine(Footer("LEFT/RIGHT or TAB sheet   -/+ speed"));
                text.Append(AnsiImage.FromPixels(LayoutCycles(cycles, step)).ToAnsi(PictureOptions()));
                return text.ToString();
            }

            var frames = sheet.Load();
            if (_mirror)
                frames = frames.Select(Assets.Mirror).ToArray();

            var canvas = Layout(frames, out var columns);
            text.AppendLine(
                $"{frames.Length} frames   {columns} per row, reading order   " +
                $"{(_mirror ? "MIRRORED" : "as cut")}");
            text.AppendLine(Footer("LEFT/RIGHT or TAB sheet   M mirror"));
            text.Append(AnsiImage.FromPixels(canvas).ToAnsi(PictureOptions()));
            return text.ToString();
        }

        /// <summary>
        ///     Every walking thing in the game, gathered so a wrong frame order or a flipped mirror shows up as one
        ///     cell limping while the rest stride. Built through <see cref="DosFrames" /> — the same lookup the hunt
        ///     and travel screens draw with — because a private copy of the mapping would only ever agree with itself.
        /// </summary>
        private static Cycle[] BuildCycles()
        {
            var cycles = new List<Cycle>
            {
                // travelox needs no mapping: frames 1-3 are the cycle in order.
                new("ox team", [DosFrames.Ox(0), DosFrames.Ox(1), DosFrames.Ox(2)])
            };

            // The hunter runs the game's own [0,1,0,2] cycle, so a mis-ordered strip reads as a stutter.
            for (var aim = 0; aim < DosFrames.Compass.Length; aim++)
            {
                var heading = aim;
                cycles.Add(new Cycle($"hunt {DosFrames.Compass[aim]}",
                    HuntGame.WalkCycle.Select(f => DosFrames.Hunter(heading, f)).ToArray()));
            }

            // Both facings of every species: the right-hand column should be the left one walking backwards.
            for (var species = 0; species < HuntGame.Species.Length; species++)
            foreach (var facing in new[] { 1, -1 })
            {
                var id = species;
                var direction = facing;
                var name = HuntGame.Species[species].Name.Split(' ')[0].ToLowerInvariant();
                cycles.Add(new Cycle($"{name}{(facing < 0 ? "<" : ">")}",
                    Enumerable.Range(0, 3).Select(f => DosFrames.Animal(id, f, direction)).ToArray()));
            }

            return cycles.ToArray();
        }

        /// <summary>Lays the cycles out in a grid, each showing whichever frame it is up to.</summary>
        private static PixelBuffer LayoutCycles(Cycle[] cycles, int step)
        {
            var widest = cycles.Max(c => c.Frames.Max(f => f.Width));
            var tallest = cycles.Max(c => c.Frames.Max(f => f.Height));
            var longestLabel = cycles.Max(c => PixelFont.Measure(c.Label));

            var cellWidth = Math.Max(widest, longestLabel) + 8;
            var cellHeight = tallest + PixelFont.LineHeight() + 8;
            var columns = Math.Max(1, (Assets.DosWidth - 4) / cellWidth);
            var rows = (cycles.Length + columns - 1) / columns;

            var canvas = new PixelBuffer(columns * cellWidth + 4, rows * cellHeight + 4);
            var field = Palette.Chrome.Field;
            var label = Palette.Chrome.Label;
            for (var y = 0; y < canvas.Height; y++)
            for (var x = 0; x < canvas.Width; x++)
                canvas.SetPixel(x, y, field);

            for (var i = 0; i < cycles.Length; i++)
            {
                var cellX = 2 + i % columns * cellWidth;
                var cellY = 2 + i / columns * cellHeight;
                var frame = cycles[i].Frames[step % cycles[i].Frames.Length];

                // Bottom-aligned, so feet that do not line up between frames show as a bounce.
                canvas.DrawImage(frame, cellX + (cellWidth - frame.Width) / 2, cellY + (tallest - frame.Height));
                PixelFont.DrawCentered(canvas, cycles[i].Label, cellX + cellWidth / 2, cellY + tallest + 3, label);
            }

            return canvas;
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
            var field = Palette.Chrome.Field;
            var label = Palette.Chrome.Label;
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

        /// <summary>One walking thing, and the frames its cycle runs through.</summary>
        /// <param name="Label">Short caption under the cell.</param>
        /// <param name="Frames">The cycle, in order.</param>
        private readonly record struct Cycle(string Label, PixelBuffer[] Frames);

        /// <summary>One contact sheet: a DOS sheet and how many sprites were cut from it.</summary>
        /// <param name="Title">What the sheet shows.</param>
        /// <param name="Name">Sheet name as the cutter wrote it.</param>
        /// <param name="Count">Sprites cut.</param>
        /// <param name="Animation">True for the page that plays cycles rather than laying out one sheet.</param>
        private sealed record Sheet(string Title, string Name, int Count, bool Animation = false)
        {
            /// <summary>Loads every frame, in the order the cutter numbered them.</summary>
            public PixelBuffer[] Load() =>
                Enumerable.Range(1, Count).Select(id => Assets.Dos(Name, id)).ToArray();
        }
    }
}
