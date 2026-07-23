using WolfCurses.Graphics;

namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     The picture strips the 1990 DOS port draws above whole text screens — the title lettering over the main
    ///     menu, the family by their wagon over every party-naming prompt — extracted from the DOS art and embedded
    ///     under <c>art/</c>. Banners, not scenes: the text screens beneath stay ordinary menu and typed-input
    ///     forms (the headless bot drives them) and simply print one of these above their text when presentation
    ///     is on.
    /// </summary>
    public static class Banners
    {
        /// <summary>The title lettering and scroll flourish, drawn over the main menu.</summary>
        /// <param name="reservedRows">Rows of text the caller prints under the picture, prompt included.</param>
        public static string Title(int reservedRows) => Render("banner.png", reservedRows);

        /// <summary>The family standing by their wagon, drawn over the party-naming prompts.</summary>
        /// <param name="reservedRows">Rows of text the caller prints under the picture, prompt included.</param>
        public static string Family(int reservedRows) => Render("family.png", reservedRows);

        private static string Render(string key, int reservedRows)
        {
            int rows;
            try
            {
                rows = Console.WindowHeight - reservedRows;
            }
            catch (Exception)
            {
                // No real console, or one that will not answer. A guess is fine; failing to draw is not.
                rows = 24 - reservedRows;
            }

            // Not centred: centring indents each row and the console is not cleared between frames, so the
            // indented columns would keep whatever the previous screen left there.
            return AnsiImage.FromPixels(Art.Load(key)).ToAnsi(new AnsiImageOptions
            {
                Fit = AnsiImageFitEnum.Contain,
                MaxRows = Math.Max(4, rows)
            });
        }
    }
}
