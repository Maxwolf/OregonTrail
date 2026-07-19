using System.Text;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     Every river crossing the original can show, played one after another so each can be wired into the game
    ///     with something to check it against.
    ///     <para>
    ///         The layering is the original's and is only four deep: a framed black box, the river filling it, the
    ///         bank drawn as a corner triangle, and <b>one</b> vehicle sprite. See <see cref="RiverCrossingGame" /> for
    ///         where each number comes from; the short version is that <c>I</c> picks the sprite and the two animated
    ///         sweeps are the same primitive fired into opposite corners — water into the near bank as you leave, land
    ///         into the far corner as you arrive.
    ///     </para>
    ///     <para>
    ///         Two departures from the 1985 art, both because this draws with the DOS port's sprites. The Apple II
    ///         keeps a dedicated river picture (<c>RIVER.IMA</c> image 5) whose bank is a flat <b>black</b> triangle;
    ///         the DOS port has no river backdrop at all — its crossing sprites are keyed against the water colour
    ///         because the water was drawn, not blitted — so the river is synthesized here, and the bank is given an
    ///         earth colour instead of black so it reads as ground rather than as a hole in the frame.
    ///     </para>
    /// </summary>
    [ParentWindow(typeof(MinigamesWindow))]
    public sealed class RiverCrossingForm : SceneForm
    {
        // float.png ids. The DOS port draws all four states the Apple II's RIVER.IMA carries, so the mapping is
        // one-for-one: I=2 ford, I=3 float, I=1 ferry, I=4 wreck.
        private const int FordSprite = 1;
        private const int FloatSprite = 2;
        private const int FerrySprite = 3;
        private const int WreckSprite = 8;

        private static readonly Rgba32 Water = new(64, 176, 252, 255);
        private static readonly Rgba32 Land = new(96, 68, 40, 255);
        private static readonly Rgba32 Frame = new(255, 255, 255, 255);
        private static readonly Rgba32 Backdrop = new(0, 0, 0, 255);

        private RiverCrossingGame _game = null!;

        /// <summary>Initializes a new instance of the <see cref="RiverCrossingForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public RiverCrossingForm(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override int ReservedRows => 10;

        /// <inheritdoc />
        protected override void Build() => _game = new RiverCrossingGame();

        /// <inheritdoc />
        protected override void Advance() => _game.Advance();

        /// <inheritdoc />
        protected override void OnSectionKey(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.RightArrow:
                case ConsoleKey.N:
                    _game.Next();
                    break;
                case ConsoleKey.LeftArrow:
                    _game.Previous();
                    break;
                case ConsoleKey.Spacebar:
                    _game.TogglePause();
                    break;
                case ConsoleKey.UpArrow:
                    _game.AdjustRain(0.05);
                    break;
                case ConsoleKey.DownArrow:
                    _game.AdjustRain(-0.05);
                    break;
                case ConsoleKey.R:
                    _game.Restart();
                    break;
                default:
                    return;
            }

            Invalidate();
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var scenario = _game.Scenario;
            var river = scenario.River;

            var text = new StringBuilder();
            text.AppendLine();
            text.AppendLine(
                $"RIVER CROSSING {_game.Index + 1}/{RiverCrossingGame.Scenarios.Length} — {river.Name}");
            text.AppendLine(
                $"depth {_game.LiveDepth,4:0.0} ft   width {_game.LiveWidth,4} ft   swiftness {_game.LiveSpeed,4:0.0}   " +
                $"bottom {river.Bottom switch { 1 => "muddy", 2 => "rough", _ => "none " }}   rain AR {_game.Rain:0.00}");
            text.AppendLine(
                $"menu: ford / caulk and float / {RiverCrossingGame.OptionName(river.Option)}   " +
                $"(RC row {Array.IndexOf(RiverCrossingGame.Rivers, river)}, LM {river.Landmark})");
            text.AppendLine(
                $"chose {scenario.Method}  ->  sprite {_game.Vehicle}   costs {_game.Toll}   risk: {_game.RiskOf}");
            text.AppendLine($"{_game.Message}");
            text.AppendLine($"{scenario.Note}");
            text.AppendLine(Footer(
                $"{_game.Phase} {_game.Step + 1}/{_game.PhaseLength}   " +
                "SPACE pause   ARROWS scenario / rain   R replay"));

            text.Append(AnsiImage.FromPixels(Draw()).ToAnsi(PictureOptions()));
            return text.ToString();
        }

        /// <summary>Builds the frame exactly as <c>:50000</c> and <c>:50110</c> do, then plays the current beat.</summary>
        private PixelBuffer Draw()
        {
            var canvas = new PixelBuffer(RiverCrossingGame.ScreenWidth, RiverCrossingGame.ScreenHeight);
            Fill(canvas, 0, 0, canvas.Width, canvas.Height, Backdrop);

            var x1 = RiverCrossingGame.FrameX1;
            var y1 = RiverCrossingGame.FrameY1;
            var x2 = RiverCrossingGame.FrameX2;
            var y2 = RiverCrossingGame.FrameY2;

            // :50110 — `& BOX` fills the rect, then three rectangles inset a pixel at a time make a solid white band.
            // That band is drawn here as a fill rather than as three hairlines, because insetting in *this* space
            // does not survive the scale: 280 -> 320 stretches a 3-pixel border to about 3.4, so three converted
            // single-pixel rectangles cannot tile it and whatever they miss shows through as black.
            Fill(canvas, x1, y1, x2 - x1 + 1, y2 - y1 + 1, Frame);

            // The river, and the bank the party is leaving. On the Apple II both arrive together as image 5, blitted
            // by `& IMAGE,5,58,24` at 161x113 — so it covers exactly 58..218 by 24..136. Sizing the water from that
            // rectangle rather than from an inset off the frame is what keeps it flush with the sweeps: the fans run
            // out to 218 and 136 too, and an inset that stopped a pixel short left a black hairline down the right
            // edge and along the bottom wherever the fans did not happen to reach.
            var waterX1 = RiverCrossingGame.ToX(58);
            var waterY1 = RiverCrossingGame.ToY(24);
            var waterX2 = RiverCrossingGame.ToX(218);
            var waterY2 = RiverCrossingGame.ToY(136);
            Fill(canvas, waterX1, waterY1, waterX2 - waterX1 + 1, waterY2 - waterY1 + 1, Water);

            // The bank at rest is exactly the region :50010 sweeps, so it is drawn by running that same fan to
            // completion in land — no second derivation to get wrong. The crossing then repaints the leading part of
            // it in water, which is the animation.
            NearBank(canvas, 1.0, Land);
            if (!_game.Refused)
                NearBank(canvas, Progress(CrossingPhaseEnum.Crossing), Water);

            // :50030 — on a clean crossing, land fans into the opposite corner. That is the far shore arriving.
            if (!_game.Failed && !_game.Refused)
                FarBank(canvas, Progress(CrossingPhaseEnum.Outcome));

            DrawVehicle(canvas);
            return canvas;
        }

        /// <summary>Blits the one sprite, swapping in the wreck when the original would have.</summary>
        private void DrawVehicle(PixelBuffer canvas)
        {
            // A refused crossing draws no vehicle at all, because the original never gets this far: RIVER.LIB sets
            // F=1 and `ON F GOTO 50010` bounces straight back to the menu, so CROSS.LIB is never even loaded. The
            // river and its bank are all there is to see.
            if (_game.Refused)
                return;

            var showWreck = _game.ShowsWreck &&
                            _game.Phase is CrossingPhaseEnum.Outcome or CrossingPhaseEnum.Done;

            var sprite = Assets.Dos("float", showWreck
                ? WreckSprite
                : _game.Vehicle switch
                {
                    CrossingMethodEnum.Ferry => FerrySprite,
                    CrossingMethodEnum.Float => FloatSprite,
                    _ => FordSprite
                });

            canvas.DrawImage(sprite, RiverCrossingGame.VehicleX, RiverCrossingGame.VehicleY);

            // :50060 — the fording wagon is not replaced when it fails, it is swamped where it stands: a wedge of
            // water fanned over the top of it.
            if (!_game.ShowsSwamping || _game.Phase is not (CrossingPhaseEnum.Outcome or CrossingPhaseEnum.Done))
                return;

            // FOR L = 0 TO 21: HPLOT 80+2*L,71 TO 180,100-L — a wedge of water fanned across the wagon's middle. It
            // covers everything below y=71, so what is left showing is the top of the canvas: swamped, not removed.
            foreach (var l in Fan(RiverCrossingGame.SwampSteps, Progress(CrossingPhaseEnum.Outcome)))
                Line(canvas,
                    RiverCrossingGame.ToX(80 + 2 * l), RiverCrossingGame.ToY(71.0),
                    RiverCrossingGame.ToX(180.0), RiverCrossingGame.ToY(100 - l), Water);
        }

        /// <summary>
        ///     `:50010` — <c>FOR L = 0 TO 52: HPLOT 114+2*L,136 TO 218,77+L</c>. The fan pivots off the bottom-right
        ///     corner: its start slides along the foot of the picture while its end slides down the right-hand side,
        ///     so the region swept is the triangle <c>(114,136) (218,136) (218,77)</c> — which is exactly where the
        ///     Apple II's river picture puts the bank. Run to completion it paints that bank; run partway in water it
        ///     is the party pulling away from it.
        /// </summary>
        private static void NearBank(PixelBuffer canvas, double progress, Rgba32 colour)
        {
            foreach (var l in Fan(RiverCrossingGame.CrossingSteps, progress))
                Line(canvas,
                    RiverCrossingGame.ToX(114 + 2 * l), RiverCrossingGame.ToY(136.0),
                    RiverCrossingGame.ToX(218.0), RiverCrossingGame.ToY(77 + l), colour);
        }

        /// <summary>
        ///     `:50030` — <c>FOR L = 0 TO 60: HPLOT 58,24+L TO 58+2*L,24</c>, in <c>HCOLOR=4</c>, which is black, and
        ///     black is what this scene draws land in. The same primitive as the near bank, fired into the opposite
        ///     corner: the far shore coming into view, and the only thing that marks a clean crossing.
        /// </summary>
        private static void FarBank(PixelBuffer canvas, double progress)
        {
            foreach (var l in Fan(RiverCrossingGame.LandingSteps, progress))
                Line(canvas,
                    RiverCrossingGame.ToX(58.0), RiverCrossingGame.ToY(24 + l),
                    RiverCrossingGame.ToX(58 + 2 * l), RiverCrossingGame.ToY(24.0), Land);
        }

        /// <summary>The sub-step positions of a sweep that is <paramref name="progress" /> of the way through.</summary>
        private static IEnumerable<double> Fan(int steps, double progress)
        {
            var last = steps * progress * RiverCrossingGame.FanSubdivision;
            for (var s = 0; s < last; s++)
                yield return s / (double) RiverCrossingGame.FanSubdivision;
        }

        /// <summary>How far through the given beat, 0 to 1, or 1 once it is behind us.</summary>
        private double Progress(CrossingPhaseEnum phase) =>
            _game.Phase == phase
                ? Math.Clamp((_game.Step + 1) / (double) _game.PhaseLength, 0, 1)
                : _game.Phase > phase
                    ? 1.0
                    : 0.0;

        private static void Fill(PixelBuffer canvas, int x, int y, int width, int height, Rgba32 colour)
        {
            for (var dy = 0; dy < height; dy++)
            for (var dx = 0; dx < width; dx++)
                canvas.SetPixel(x + dx, y + dy, colour);
        }

        /// <summary>The original draws these sweeps with <c>HPLOT ... TO ...</c>; this is that, clipped.</summary>
        private static void Line(PixelBuffer canvas, int x1, int y1, int x2, int y2, Rgba32 colour)
        {
            var dx = Math.Abs(x2 - x1);
            var dy = -Math.Abs(y2 - y1);
            var stepX = x1 < x2 ? 1 : -1;
            var stepY = y1 < y2 ? 1 : -1;
            var error = dx + dy;

            while (true)
            {
                if (x1 >= 0 && x1 < canvas.Width && y1 >= 0 && y1 < canvas.Height)
                    canvas.SetPixel(x1, y1, colour);

                if (x1 == x2 && y1 == y2)
                    return;

                var doubled = 2 * error;
                if (doubled >= dy)
                {
                    error += dy;
                    x1 += stepX;
                }

                if (doubled > dx)
                    continue;

                error += dx;
                y1 += stepY;
            }
        }
    }
}
