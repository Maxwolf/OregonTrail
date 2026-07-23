using System.Collections.Concurrent;
using WolfCurses.Graphics;
using AssetStore = OregonTrailDotNet.Assets.AssetStore;

namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     Decodes and caches the artwork the graphical scenes draw with. The pictures are compiled into the
    ///     <c>OregonTrailDotNet.Assets</c> library as embedded resources rather than read off disk, so both the
    ///     workbench and the game ship as single executables with no loose art files to find, ship alongside, or lose.
    ///     <para>
    ///         The scenes are drawn with the <b>1990 DOS port's</b> art: 320x200 in 256 colours against the Apple II's
    ///         280x192 in six, and — decisively for a terminal — its sprites are shaded illustrations where the Apple II's
    ///         are white silhouettes. Terminal rendering widens that gap rather than narrowing it, because area-averaging
    ///         a picture down to character cells preserves colour but destroys the Apple II's dithering.
    ///     </para>
    ///     <para>
    ///         Everything is addressed by its path within the art set — <c>sprites/hunter/01.png</c>,
    ///         <c>tombstone.png</c>. A key with no matching resource is not an exception: it comes back as the
    ///         magenta-and-black "missing texture" checkerboard, which is exactly what should appear on screen if a build
    ///         is ever cut without its art.
    ///     </para>
    /// </summary>
    public static class Art
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

        /// <summary>
        ///     True when the embedded artwork is present. Since the pictures are compiled into
        ///     <c>OregonTrailDotNet.Assets</c> they always should be, so this is a sanity check on the build rather than
        ///     on the file system — a guard against a broken package, not a missing folder.
        /// </summary>
        public static bool Ready => AssetStore.Has("art/map.png");

        /// <summary>Loads a picture from the assets library, decoded once and kept.</summary>
        /// <param name="relativePath">
        ///     The picture's path within the art set, for example <c>sprites/hunter/01.png</c>. The <c>art/</c>
        ///     prefix that namespaces it inside the assets library is added here.
        /// </param>
        public static PixelBuffer Load(string relativePath)
        {
            return Cache.GetOrAdd(relativePath, key =>
            {
                using var stream = AssetStore.Open($"art/{key}");
                return stream == null
                    ? ImageErrorTexture.Create(DosWidth, DosHeight, 8)
                    : AnsiImage.FromStream(stream).Pixels;
            });
        }

        /// <summary>
        ///     A sprite cut out of one of the DOS sheets by <c>legacy/tools/dos_sprites.py</c>. Ids are 1-based and follow
        ///     the sheet's reading order.
        /// </summary>
        /// <param name="sheet">
        ///     One of <c>hunter</c>, <c>animals</c>, <c>float</c>, <c>terrain</c>, <c>scenery</c>, <c>travelox</c>,
        ///     <c>events</c> — the sheet directories under <c>art/sprites/</c> in the assets library.
        /// </param>
        /// <param name="id">1-based sprite id within that sheet.</param>
        public static PixelBuffer Dos(string sheet, int id) => Load($"sprites/{sheet}/{id:00}.png");

        /// <summary>
        ///     An Apple II full-screen card, scaled to that machine's own grid. Used for the tombstone, which the DOS
        ///     port has no equivalent of.
        /// </summary>
        /// <param name="fileName">For example <c>tombstone.png</c>.</param>
        public static PixelBuffer Apple2Backdrop(string fileName)
        {
            return Cache.GetOrAdd("a2backdrop:" + fileName, _ =>
            {
                var picture = Load(fileName);
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
    }
}
