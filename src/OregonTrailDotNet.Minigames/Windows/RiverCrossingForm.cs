using OregonTrailDotNet.Presentation;
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
    public sealed class RiverCrossingForm : WorkbenchSceneForm
    {
        // The sprite ids, fan sweeps and drawing primitives live in CrossingArt now — they are the pieces a
        // game-side crossing scene reuses verbatim.
        private const int FordSprite = CrossingArt.FordSprite;
        private const int FloatSprite = CrossingArt.FloatSprite;
        private const int FerrySprite = CrossingArt.FerrySprite;
        private const int WreckSprite = CrossingArt.WreckSprite;

        // The bank is the SAME arid sand as the Columbia raft screen's, not mud -- see Palette for why these are
        // shared rather than mixed per screen.
        private static readonly Rgba32 Water = Palette.Water;
        private static readonly Rgba32 Land = Palette.Sand;
        private static readonly Rgba32 Frame = Palette.White;
        private static readonly Rgba32 Backdrop = Palette.Black;

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
            CrossingArt.NearBank(canvas, 1.0, Land);
            if (!_game.Refused)
                CrossingArt.NearBank(canvas, Progress(CrossingPhaseEnum.Crossing), Water);

            // :50030 — on a clean crossing, land fans into the opposite corner. That is the far shore arriving.
            if (!_game.Failed && !_game.Refused)
                CrossingArt.FarBank(canvas, Progress(CrossingPhaseEnum.Outcome), Land);

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

            var sprite = Art.Dos("float", showWreck
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

            CrossingArt.SwampWedge(canvas, Progress(CrossingPhaseEnum.Outcome), Water);
        }

        /// <summary>How far through the given beat, 0 to 1, or 1 once it is behind us.</summary>
        private double Progress(CrossingPhaseEnum phase) =>
            _game.Phase == phase
                ? Math.Clamp((_game.Step + 1) / (double) _game.PhaseLength, 0, 1)
                : _game.Phase > phase
                    ? 1.0
                    : 0.0;

        private static void Fill(PixelBuffer canvas, int x, int y, int width, int height, Rgba32 colour) =>
            CrossingArt.Fill(canvas, x, y, width, height, colour);
    }
}
