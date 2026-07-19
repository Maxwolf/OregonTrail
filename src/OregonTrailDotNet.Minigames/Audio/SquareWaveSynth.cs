namespace OregonTrailDotNet.Minigames.Audio
{
    /// <summary>
    ///     Turns a <see cref="Chiptune" /> into PCM the sound card can play.
    ///     <para>
    ///         A square wave and nothing else, because that is all either machine could make: the Apple II toggles the
    ///         speaker directly and the PC speaker is a one-bit output driven by timer channel 2. There is no envelope
    ///         to model and no timbre to choose — the only thing worth getting right is the articulation.
    ///     </para>
    /// </summary>
    public static class SquareWaveSynth
    {
        /// <summary>Sample rate. Matches the renders in <c>legacy/music/</c> so the two can be compared.</summary>
        public const int SampleRate = 22050;

        /// <summary>
        ///     The fraction of a note's length that actually sounds; the rest is silence before the next note.
        ///     <para>
        ///         This is the original's own convention, not a softening: GW-BASIC's <c>PLAY</c> defaults to
        ///         <c>MN</c> ("music normal"), which sounds 7/8 of each note's duration and rests the last eighth.
        ///         Without it a run of repeated notes — and these tunes are full of them — fuses into one long tone,
        ///         because nothing in a square wave separates two identical pitches laid end to end.
        ///     </para>
        /// </summary>
        private const double Articulation = 7.0 / 8.0;

        /// <summary>
        ///     Peak amplitude, well below full scale. A square wave at full scale on a modern output is startlingly
        ///     loud, and the point of comparing against the originals is to hear the tune, not to be shouted at.
        /// </summary>
        private const short Amplitude = 6000;

        /// <summary>Renders a whole tune to signed 16-bit mono PCM at <see cref="SampleRate" />.</summary>
        /// <param name="tune">The score to render.</param>
        public static byte[] Render(Chiptune tune)
        {
            var samples = new List<short>((int) (tune.Duration.TotalSeconds * SampleRate) + SampleRate);

            foreach (var note in tune.Notes)
            {
                var total = (int) Math.Round(note.Milliseconds / 1000.0 * SampleRate);
                if (total <= 0)
                    continue;

                var sounding = note.Hertz is > 0 ? (int) (total * Articulation) : 0;
                Tone(samples, note.Hertz ?? 0, sounding);

                for (var i = sounding; i < total; i++)
                    samples.Add(0);
            }

            var pcm = new byte[samples.Count * 2];
            for (var i = 0; i < samples.Count; i++)
            {
                pcm[i * 2] = (byte) (samples[i] & 0xFF);
                pcm[i * 2 + 1] = (byte) ((samples[i] >> 8) & 0xFF);
            }

            return pcm;
        }

        /// <summary>
        ///     Appends one square-wave tone.
        ///     <para>
        ///         The ramp at each end is the one liberty taken. A square wave that starts and stops at full
        ///         amplitude puts a step edge into the output, which a real speaker cone reproduces as an audible tick
        ///         on every single note — an artefact of the sound card, not of the original hardware, whose speaker
        ///         was already sitting at one rail or the other. A few hundred samples of ramp removes it and is far
        ///         too short to hear as an attack.
        ///     </para>
        /// </summary>
        private static void Tone(List<short> samples, double hertz, int count)
        {
            if (count <= 0)
                return;

            if (hertz <= 0)
            {
                for (var i = 0; i < count; i++)
                    samples.Add(0);

                return;
            }

            var period = SampleRate / hertz;
            var ramp = Math.Min(64, count / 2);

            for (var i = 0; i < count; i++)
            {
                var high = i % period < period / 2;
                var level = (double) (high ? Amplitude : -Amplitude);

                if (i < ramp)
                    level *= (double) i / ramp;
                else if (i >= count - ramp)
                    level *= (double) (count - 1 - i) / ramp;

                samples.Add((short) level);
            }
        }
    }
}
