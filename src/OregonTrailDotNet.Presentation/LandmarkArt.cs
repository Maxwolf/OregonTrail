using WolfCurses.Graphics;

namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     The landmark card and its caption box — the screen the 1985 game shows when the player looks around a
    ///     stop, drawn at <c>OREGON TRAIL:305</c>: <c>&amp; BOX,X1,160,X2,180,3</c> then the name and arrival date
    ///     printed <c>INVERSE</c> (dark glyphs on the white box). The workbench's slideshow deliberately showed no
    ///     date — it has no journey to date a card from — so the box lives here, where the game wires in its clock.
    /// </summary>
    public static class LandmarkArt
    {
        /// <summary>The original's 7x8 text cell, which <see cref="PixelFont.DrawFixed" /> reproduces.</summary>
        private const int CellWidth = 7;

        private const int CellHeight = 8;

        /// <summary>The DOS card for a landmark index, <c>art/landmarks/p{n}.png</c>.</summary>
        public static PixelBuffer Card(int index) => Art.Load($"landmarks/p{index}.png");

        /// <summary>
        ///     <c>Z$</c> — the landmark name as the caption prints it: <c>LM$</c> with a leading <c>"the "</c>
        ///     stripped, mixed case kept (<c>OREGON TRAIL:260</c>). <c>the Kansas River crossing</c> captions as
        ///     <c>Kansas River crossing</c>.
        /// </summary>
        public static string CaptionName(string originalName) =>
            originalName.StartsWith("the ", StringComparison.Ordinal) ? originalName[4..] : originalName;

        /// <summary>
        ///     A copy of the card with the caption box drawn in: name over date, black on white, each line centred.
        ///     The box hugs the longer line the way the original sized <c>X1..X2</c> to its text.
        /// </summary>
        public static PixelBuffer WithCaption(PixelBuffer card, string nameLine, string dateLine)
        {
            var framed = card.Crop(0, 0, card.Width, card.Height);

            // The BASIC's box rows 160-180 sit at 5/6 of the way down the Apple II's 192-row screen; place it
            // proportionally on whatever height this card actually is (the DOS cards are cropped tighter than a
            // full 200-row screen), clamped so a short card never pushes the box off the picture.
            var boxHeight = 2 * CellHeight + 6;
            var top = Math.Clamp(card.Height * 160 / 192, 0, Math.Max(0, card.Height - boxHeight));
            var bottom = Math.Min(card.Height - 1, top + boxHeight - 1);

            var textWidth = Math.Max(nameLine.Length, dateLine.Length) * CellWidth;
            var boxWidth = Math.Min(card.Width - 4, textWidth + 12);
            var left = Math.Max(2, (card.Width - boxWidth) / 2);

            for (var y = top; y <= bottom; y++)
                for (var x = left; x < left + boxWidth; x++)
                    framed.SetPixel(x, y, Palette.White);

            var nameY = top + 3;
            var dateY = nameY + CellHeight + 1;
            PixelFont.DrawFixed(framed, nameLine, left + (boxWidth - nameLine.Length * CellWidth) / 2, nameY,
                Palette.Black, CellWidth);
            PixelFont.DrawFixed(framed, dateLine, left + (boxWidth - dateLine.Length * CellWidth) / 2, dateY,
                Palette.Black, CellWidth);

            return framed;
        }
    }
}
