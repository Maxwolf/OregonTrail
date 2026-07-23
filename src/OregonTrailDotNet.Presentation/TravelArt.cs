using WolfCurses.Graphics;

namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     The travel screen's assembled artwork, shared by every host that shows it. Everything here is assembled
    ///     because the original assembled it — there is no travel picture on either disk: a flooded ground box, a
    ///     full-width horizon strip, and the white status panel are the whole backdrop.
    /// </summary>
    public static class TravelArt
    {
        // travelox.png, cut in reading order: the three walk frames first, then the two the loss events swap in.

        /// <summary>The first ox-team walk frame on the <c>travelox</c> sheet (frames 1-3 cycle).</summary>
        public const int FirstWalkFrame = 1;

        /// <summary>The broken-wagon frame the breakdown events swap in.</summary>
        public const int BrokenWagon = 4;

        /// <summary>The burning-wagon frame the fire event swaps in.</summary>
        public const int BurningWagon = 5;

        // scenery.png's two full-width horizon strips.

        /// <summary>The plains horizon strip on the <c>scenery</c> sheet.</summary>
        public const int PlainsStrip = 1;

        /// <summary>The mountains horizon strip on the <c>scenery</c> sheet.</summary>
        public const int MountainStrip = 2;

        /// <summary>
        ///     Sky, ground, horizon. The <b>sky is black</b> — this is a 1980s micro, not a painted backdrop, and the
        ///     only two things ever coloured in are the ground box and the status panel.
        ///     <para>
        ///         The ground colour is the whole of what weather does to this screen, and it is one line (`:310`):
        ///     </para>
        ///     <code>CC = 3*(AS&gt;=1) + (AS&lt;1)*(5*(AR&lt;=.2) + (AR&gt;.2))</code>
        ///     <para>
        ///         Colour 3 is white, 5 orange, 1 green — snow if there is any lying (which before April there always
        ///         is), otherwise arid if the rain accumulator has stayed low, otherwise green. A flood fill, not a
        ///         picture, which is why the whole country can change season for the cost of one <c>&amp; BOX</c>.
        ///     </para>
        /// </summary>
        public static PixelBuffer BuildBackdrop(TravelTerrainEnum terrain, TravelWeatherEnum weather)
        {
            var backdrop = new PixelBuffer(TravelGame.ScreenWidth, TravelGame.ScreenHeight);

            var black = Palette.Black;
            var white = Palette.White;

            // The original floods the ground with `& BOX ,CC` where CC is 3, 5 or 1 (:310). Those are Apple II hi-res
            // colour numbers, but the art on top of them here is the DOS port's, so each is taken as the nearest DOS
            // palette entry rather than as the 1985 colour -- otherwise the flood is a shade no sprite above it can
            // contain. Snow and grass land on real palette entries either way; the arid case is the one that moved,
            // from a mixed-by-hand orange that is in NEITHER palette to the sand the port's own dry ground uses.
            var ground = weather switch
            {
                TravelWeatherEnum.Snow => Palette.Snow,     // CC=3
                TravelWeatherEnum.Arid => Palette.Sand,     // CC=5
                _ => Palette.Grass                          // CC=1
            };

            for (var y = 0; y < backdrop.Height; y++)
            {
                var colour = y >= TravelGame.PanelY && y < TravelGame.PanelBottomY ? white
                    : y >= TravelGame.GroundY && y < TravelGame.GroundBottomY ? ground
                    : black;

                for (var x = 0; x < backdrop.Width; x++)
                    backdrop.SetPixel(x, y, colour);
            }

            // The strip is exactly as wide as the screen and hangs well above the ground, with black between the two.
            // That black band is not empty space — it is where the team walks.
            var strip = Art.Dos("scenery", terrain == TravelTerrainEnum.Plains ? PlainsStrip : MountainStrip);
            backdrop.DrawImage(strip, 0, TravelGame.StripY);

            return backdrop;
        }

        /// <summary>Draws the prompt line centred over the black band under the picture.</summary>
        public static void DrawPrompt(PixelBuffer frame, string prompt) =>
            PixelFont.DrawFixed(frame, prompt,
                (TravelGame.ScreenWidth - prompt.Length * 8) / 2, TravelGame.PromptY, Palette.White, 8);

        /// <summary>
        ///     Draws the status panel's readings over a composed frame. This cannot live in a cached backdrop,
        ///     because every reading on it changes as the team walks. The rows are passed in so the host decides
        ///     what is true — the workbench feeds stand-ins, the game feeds the simulation.
        ///     <para>
        ///         Labels right-aligned onto a shared colon column, values all starting on the next — which is what
        ///         gives the panel its ragged left edge and dead-straight middle.
        ///     </para>
        /// </summary>
        public static void DrawStatusPanel(PixelBuffer frame, IReadOnlyList<(string Label, string Value)> rows)
        {
            var black = Palette.Black;

            var y = TravelGame.PanelY + 2;
            foreach (var (label, value) in rows)
            {
                PixelFont.DrawFixed(frame, label, TravelGame.PanelLabelRight - label.Length * 8, y, black, 8);
                PixelFont.DrawFixed(frame, value, TravelGame.PanelValueLeft, y, black, 8);
                y += TravelGame.PanelLineHeight;
            }
        }
    }
}
