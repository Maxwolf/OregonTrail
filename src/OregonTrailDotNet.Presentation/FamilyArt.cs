using WolfCurses.Graphics;

namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     The family standing by their wagon — the picture the 1990 DOS port draws above every party-naming
    ///     prompt, extracted to <c>legacy/art/dos/mcga/family.png</c> and embedded as <c>art/family.png</c>.
    ///     A banner, not a scene: the naming forms stay ordinary typed-input forms (the headless bot types
    ///     through them) and simply print this above their text when presentation is on.
    /// </summary>
    public static class FamilyArt
    {
        /// <summary>
        ///     The picture as ANSI sized for the current console, leaving the given number of rows beneath it
        ///     for the form's own text and the input prompt.
        /// </summary>
        /// <param name="reservedRows">Rows of text the caller prints under the picture, prompt included.</param>
        public static string Banner(int reservedRows)
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
            return AnsiImage.FromPixels(Art.Load("family.png")).ToAnsi(new AnsiImageOptions
            {
                Fit = AnsiImageFitEnum.Contain,
                MaxRows = Math.Max(4, rows)
            });
        }
    }
}
