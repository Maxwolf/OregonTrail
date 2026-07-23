using System.Reflection;

namespace OregonTrailDotNet.Assets
{
    /// <summary>
    ///     The game's art and music, compiled into this assembly as embedded resources so it ships as a single
    ///     executable with no loose asset files sitting on disk.
    ///     <para>
    ///         Every asset is addressed by its path under the project's <c>Resources/</c> folder — for example
    ///         <c>art/dos/mcga/p3.png</c>, <c>music/dos/song-04-chimney-rock.json</c>, <c>data/PAL.256</c>. That path
    ///         <b>is</b> the embedded resource's name (set as its <c>LogicalName</c> in the project file), so a key maps
    ///         straight to its bytes with no manifest-name mangling to reason about.
    ///     </para>
    ///     <para>
    ///         This library deliberately references nothing — not even WolfCurses — and only hands back bytes and
    ///         streams. Turning a PNG into pixels or a score into notes is the caller's job. Keeping the decode out of
    ///         here is what lets the minigame workbench use it now and the main game use it later without either one
    ///         dictating the other's image or audio stack.
    ///     </para>
    /// </summary>
    public static class AssetStore
    {
        private static readonly Assembly Assembly = typeof(AssetStore).Assembly;

        /// <summary>
        ///     Logical name (normalised to forward slashes) to the manifest name it was embedded under. The build's
        ///     directory separator leaks into the manifest name — a backslash on Windows — so it is normalised once
        ///     here, and a caller's key (<c>art/dos/mcga/p3.png</c>) matches no matter where the assembly was built.
        /// </summary>
        private static readonly Dictionary<string, string> Names =
            Assembly.GetManifestResourceNames()
                .ToDictionary(name => name.Replace('\\', '/'), name => name);

        /// <summary>Opens a fresh read stream over an asset's bytes, or null when no asset has that key.</summary>
        /// <param name="key">The asset's path under <c>Resources/</c>, for example <c>art/dos/mcga/p3.png</c>.</param>
        public static Stream? Open(string key) =>
            Names.TryGetValue(key, out var name) ? Assembly.GetManifestResourceStream(name) : null;

        /// <summary>Reads an asset's bytes in full, or null when no asset has that key.</summary>
        /// <param name="key">The asset's path under <c>Resources/</c>.</param>
        public static byte[]? Bytes(string key)
        {
            using var stream = Open(key);
            if (stream == null)
                return null;

            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            return memory.ToArray();
        }

        /// <summary>Whether an asset with this key is embedded.</summary>
        /// <param name="key">The asset's path under <c>Resources/</c>.</param>
        public static bool Has(string key) => Names.ContainsKey(key);

        /// <summary>
        ///     Every embedded asset key that begins with the given prefix, in order — for example
        ///     <c>art/dos/rgba/</c> to walk one sheet's sprites, or the empty prefix for all of them.
        /// </summary>
        /// <param name="prefix">The key prefix to match; empty matches everything.</param>
        public static IEnumerable<string> Keys(string prefix = "") =>
            Names.Keys
                .Where(key => key.StartsWith(prefix, StringComparison.Ordinal))
                .OrderBy(key => key, StringComparer.Ordinal);
    }
}
