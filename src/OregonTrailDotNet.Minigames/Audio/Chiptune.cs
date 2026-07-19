using System.Collections.Concurrent;
using System.Text.Json;

namespace OregonTrailDotNet.Minigames.Audio
{
    /// <summary>
    ///     One of the original's tunes, as a list of notes.
    ///     <para>
    ///         Both ports are single-voice square-wave beepers, so a score really is just this: a pitch and a duration,
    ///         over and over. The notes come from <c>legacy/music/</c>, decoded off the disks by
    ///         <c>legacy/tools/apple2_music.py</c> (the Apple II <c>MS&lt;n&gt;.BIN</c> scores) and
    ///         <c>legacy/tools/dos_music.py</c> (the DOS <c>SONGS.TXT</c> MML). Nothing here is sampled audio — the
    ///         <c>.wav</c> files beside the JSON are renders for listening to, not assets, and the workbench
    ///         synthesizes from the note lists instead.
    ///     </para>
    /// </summary>
    public sealed class Chiptune
    {
        private static readonly ConcurrentDictionary<string, Chiptune?> Cache = new();

        private Chiptune(string key, IReadOnlyList<Note> notes)
        {
            Key = key;
            Notes = notes;
            Duration = TimeSpan.FromMilliseconds(notes.Sum(n => n.Milliseconds));
        }

        /// <summary>The path this was loaded from, relative to <c>legacy/music/</c> and without the extension.</summary>
        public string Key { get; }

        /// <summary>The score, in order.</summary>
        public IReadOnlyList<Note> Notes { get; }

        /// <summary>How long the whole tune runs.</summary>
        public TimeSpan Duration { get; }

        /// <summary>
        ///     Loads a decoded score, or null when it is not on disk. Cached, including the misses — a section is free
        ///     to ask for its tune every frame.
        /// </summary>
        /// <param name="key">For example <c>dos/song-05-fort-laramie</c> or <c>apple2/ts-tombstone</c>.</param>
        public static Chiptune? Load(string key)
        {
            return Cache.GetOrAdd(key, path =>
            {
                var root = Assets.MusicRoot;
                if (root == null)
                    return null;

                var file = Path.Combine(root, path.Replace('/', Path.DirectorySeparatorChar) + ".json");
                if (!File.Exists(file))
                    return null;

                try
                {
                    return new Chiptune(path, Read(file));
                }
                catch (Exception)
                {
                    // A score that will not parse is a missing score. The workbench stays silent and keeps drawing;
                    // it is not worth taking the screen down over.
                    return null;
                }
            });
        }

        private static List<Note> Read(string file)
        {
            using var document = JsonDocument.Parse(File.ReadAllBytes(file));

            var notes = new List<Note>();
            foreach (var element in document.RootElement.GetProperty("events").EnumerateArray())
            {
                var milliseconds = element.GetProperty("ms").GetDouble();

                // A rest is stored as a null pitch rather than a zero one, so read it as nullable and let a missing
                // or null "hz" mean silence. Both decoders emit rests this way; MML "p" and the Apple II's own
                // rest byte both land here.
                var hertz = element.TryGetProperty("hz", out var pitch) && pitch.ValueKind == JsonValueKind.Number
                    ? pitch.GetDouble()
                    : (double?) null;

                notes.Add(new Note(hertz, milliseconds));
            }

            return notes;
        }

        /// <summary>One note, or a rest when <paramref name="Hertz" /> is null.</summary>
        /// <param name="Hertz">
        ///     The frequency actually synthesized by the original hardware. Used in preference to the JSON's
        ///     <c>midi</c> column, which for the Apple II is a nominal mapping of a note ladder that runs about 50
        ///     cents sharp of concert pitch — the <c>hz</c> value is what the machine really played.
        /// </param>
        /// <param name="Milliseconds">How long it sounds, including its share of the gap to the next note.</param>
        public readonly record struct Note(double? Hertz, double Milliseconds);
    }
}
