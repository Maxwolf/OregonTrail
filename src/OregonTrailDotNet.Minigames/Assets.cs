using System.Collections.Concurrent;
using WolfCurses.Graphics;

namespace OregonTrailDotNet.Minigames
{
    /// <summary>
    ///     Finds and caches the artwork extracted from the original disks (see <c>docs/minigames.md</c>).
    ///     <para>
    ///         The minigames are drawn with the <b>1990 DOS port's</b> art: 320x200 in 256 colours against the Apple II's
    ///         280x192 in six, and — decisively for a terminal — its sprites are shaded illustrations where the Apple II's
    ///         are white silhouettes. Terminal rendering widens that gap rather than narrowing it, because area-averaging
    ///         a picture down to character cells preserves colour but destroys the Apple II's dithering.
    ///     </para>
    ///     <para>
    ///         Everything loads off disk rather than being embedded, so a sprite can be re-cut with
    ///         <c>legacy/tools/dos_sprites.py</c> and looked at again without rebuilding. A missing file is not an
    ///         exception: WolfCurses hands back the magenta-and-black "missing texture" checkerboard, which is exactly
    ///         what should appear on screen.
    ///     </para>
    /// </summary>
    public static class Assets
    {
        /// <summary>MCGA width. Every DOS-drawn scene is composed at this size and scaled by the renderer.</summary>
        public const int DosWidth = 320;

        /// <summary>MCGA height.</summary>
        public const int DosHeight = 200;

        /// <summary>Apple II hi-res width, still the space the ported <c>FLOAT</c> logic thinks in.</summary>
        public const int Apple2Width = 280;

        /// <summary>Apple II hi-res height.</summary>
        public const int Apple2Height = 192;

        private static readonly ConcurrentDictionary<string, PixelBuffer> Cache = new();

        /// <summary>The <c>legacy/art</c> folder, or null when it could not be found.</summary>
        public static string? ArtRoot { get; } = FindArtRoot();

        /// <summary>
        ///     The <c>legacy/music</c> folder — the decoded scores — or null when it is not there. Derived from
        ///     <see cref="ArtRoot" />'s parent rather than searched for separately, since both are folders of the one
        ///     <c>legacy/</c> tree and finding one locates the other.
        ///     <para>
        ///         Unlike the artwork this is <b>optional</b>: without it the workbench runs silently rather than
        ///         refusing to start. Music is the one thing here you can do without.
        ///     </para>
        /// </summary>
        public static string? MusicRoot { get; } = ArtRoot == null
            ? null
            : Path.Combine(Directory.GetParent(ArtRoot)!.FullName, "music");

        /// <summary>
        ///     True when the DOS artwork is present. That is the only hard requirement: every minigame is drawn with
        ///     the 1990 port's art.
        /// </summary>
        public static bool Ready =>
            ArtRoot != null &&
            Directory.Exists(Path.Combine(ArtRoot, "dos", "rgba")) &&
            Directory.Exists(Path.Combine(ArtRoot, "dos", "mcga"));

        /// <summary>
        ///     True when the 1985 cards are present too. They are <b>optional</b>, and needed for exactly two things:
        ///     the tombstone — the one screen neither DOS picture library has a bitmap for, since the DOS port draws
        ///     it with BGI primitives — and the slideshow's side-by-side comparison.
        /// </summary>
        public static bool HasApple2 => ArtRoot != null && Directory.Exists(Path.Combine(ArtRoot, "apple2"));

        /// <summary>Loads a picture relative to <see cref="ArtRoot" />, decoded once and kept.</summary>
        /// <param name="relativePath">For example <c>dos/rgba/dos-float-05.png</c>.</param>
        public static PixelBuffer Load(string relativePath)
        {
            return Cache.GetOrAdd(relativePath, key =>
            {
                var full = Path.Combine(ArtRoot ?? ".", key.Replace('/', Path.DirectorySeparatorChar));
                return AnsiImage.FromFile(full).Pixels;
            });
        }

        /// <summary>
        ///     A sprite cut out of one of the DOS sheets by <c>legacy/tools/dos_sprites.py</c>. Ids are 1-based and follow
        ///     the sheet's reading order.
        /// </summary>
        /// <param name="sheet">One of <c>hunter</c>, <c>animals</c>, <c>float</c>, <c>terrain</c>.</param>
        /// <param name="id">1-based sprite id within that sheet.</param>
        public static PixelBuffer Dos(string sheet, int id) => Load($"dos/rgba/dos-{sheet}-{id:00}.png");

        /// <summary>
        ///     An Apple II full-screen card, scaled to that machine's own grid. Used for the tombstone, which the DOS
        ///     port has no equivalent of.
        /// </summary>
        /// <param name="fileName">For example <c>ts-tombstone.png</c>.</param>
        public static PixelBuffer Apple2Backdrop(string fileName)
        {
            return Cache.GetOrAdd("a2backdrop:" + fileName, _ =>
            {
                var picture = Load($"apple2/{fileName}");
                return picture.Width == Apple2Width && picture.Height == Apple2Height
                    ? picture
                    : picture.Resize(Apple2Width, Apple2Height);
            });
        }

        /// <summary>
        ///     A horizontally flipped copy. Both ports drew only one facing and mirrored at blit time; the DOS sheets
        ///     happen to store their mirrors, so this is mostly here for the sprite viewer's mirror toggle.
        /// </summary>
        public static PixelBuffer Mirror(PixelBuffer source)
        {
            var flipped = new PixelBuffer(source.Width, source.Height);
            for (var y = 0; y < source.Height; y++)
            for (var x = 0; x < source.Width; x++)
                flipped.SetPixel(source.Width - 1 - x, y, source.GetPixel(x, y));

            return flipped;
        }

        /// <summary>
        ///     Walks up from the build output and the working directory looking for the extracted art, so the workbench
        ///     runs from an IDE, from <c>dotnet run</c>, or from its output folder without being told where anything is.
        /// </summary>
        private static string? FindArtRoot()
        {
            foreach (var start in new[] { AppContext.BaseDirectory, Environment.CurrentDirectory })
            {
                var directory = new DirectoryInfo(start);
                while (directory != null)
                {
                    var candidate = Path.Combine(directory.FullName, "legacy", "art");
                    if (Directory.Exists(candidate))
                        return candidate;

                    directory = directory.Parent;
                }
            }

            return null;
        }
    }
}
