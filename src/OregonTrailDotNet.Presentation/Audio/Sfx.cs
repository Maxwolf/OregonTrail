namespace OregonTrailDotNet.Presentation.Audio
{
    /// <summary>
    ///     The presentation layer's one-shot sound effects — the 1990 DOS port's complete effect set, recovered
    ///     from <c>OREGON.EXE</c> and documented in <c>docs/legacy-sounds.md</c>: the severe-thunderstorm rumble,
    ///     the wagon-part breakdown whoops, the hunt's muzzle pop, and the crash a sinking wagon and a colliding
    ///     raft shared. Nothing here is sampled; every effect is synthesized from its original recipe.
    ///     <para>
    ///         A separate facade with its own <see cref="WaveOutPlayer" /> on purpose: <c>winmm</c> mixes open
    ///         devices in the OS, so a short effect plays over the running tune with no duck-and-resume plumbing,
    ///         and none of <see cref="Music" />'s stop guards can cut an effect off (nor an effect restart a tune —
    ///         <see cref="Music.Play" />'s idempotence would swallow the re-assert). Mute and volume are still
    ///         <see cref="Music" />'s alone; this class only ever reads them, so the two facades cannot disagree.
    ///     </para>
    ///     <para>
    ///         Everything is fire-and-forget: a new effect replaces whichever one is still sounding, exactly as a
    ///         new <c>Sound()</c> retuned the one PC speaker.
    ///     </para>
    /// </summary>
    public static class Sfx
    {
        /// <summary>
        ///     Sample rate for effects. Double the music's, because the crash effect's last noise burst reaches
        ///     ~20 kHz — hash on the PC speaker, foldover distortion at 22050.
        /// </summary>
        public const int SampleRate = 44100;

        /// <summary>
        ///     How long each of the original's back-to-back noise <c>Sound()</c> calls is held. The DOS loop had no
        ///     delay at all — its duration was CPU-bound — so this is the port's one tuning knob: 0.1 ms sits in
        ///     the 0.5–2 s the effect took on period hardware.
        /// </summary>
        private const double NoiseTickMilliseconds = 0.1;

        private static readonly WaveOutPlayer Player = new();
        private static readonly Lock Gate = new();

        /// <summary>
        ///     The name of the last effect requested, muted or not — the observable for hosts and tests, in the
        ///     way <see cref="Music.Playing" /> names the current tune.
        /// </summary>
        public static string? LastCue { get; private set; }

        /// <summary>
        ///     The severe thunderstorm: four lightning flashes, each two 20 ms tones between 100 and 239 Hz, then
        ///     ~400 ms of low rumble — eighty 5 ms tones rolled from 0–129 Hz where a roll of 18 or under holds the
        ///     previous pitch, because the original's <c>Sound()</c> left the timer untouched below 19 Hz. That
        ///     quirk is what makes it growl instead of chirp, so it is reproduced deliberately.
        /// </summary>
        public static void Thunderstorm() => Play("thunderstorm", effect =>
        {
            var pitch = 0;
            for (var flash = 0; flash < 8; flash++)
            {
                pitch = 100 + Random.Shared.Next(140);
                effect.Tone(pitch, 20);
            }

            // The last flash tone is still sounding when the rumble loop starts, so it seeds the held pitch.
            double held = pitch;
            for (var i = 0; i < 80; i++)
            {
                var roll = Random.Shared.Next(130);
                if (roll > 18)
                    held = roll;

                effect.Tone(held, 5);
            }
        });

        /// <summary>
        ///     A wagon part giving way: three falling 800→50 Hz whoops of 30, 40 and 50 ms, back to back, played
        ///     before the original announced which of wheel, axle or tongue had broken.
        /// </summary>
        public static void Breakdown() => Play("breakdown", effect =>
        {
            effect.Sweep(800, 50, 30);
            effect.Sweep(800, 50, 40);
            effect.Sweep(800, 50, 50);
        });

        /// <summary>
        ///     The hunt's muzzle pop: one 10 ms tone. The original pitched it at the muzzle's screen row plus
        ///     50 Hz — and screen rows grow downward, so a shot fired high in the field thuds and one fired low
        ///     cracks.
        /// </summary>
        /// <param name="hertz">The pitch, normally muzzle Y + 50; clamped to what the speaker could hold.</param>
        public static void Gunshot(int hertz) => Play("gunshot", effect =>
            effect.Tone(Math.Clamp(hertz, 19, 1000), 10));

        /// <summary>
        ///     The crash a swamped wagon and a colliding raft shared — the original's <c>CrashEffect</c>: three
        ///     quick chirps (one rising, two falling, each separated by 30 ms of air) and then three successively
        ///     higher-register bursts of random static.
        /// </summary>
        public static void Crash() => Play("crash", effect =>
        {
            effect.Sweep(20, 220, 30);
            effect.Silence(30);
            effect.Sweep(300, 20, 50);
            effect.Silence(30);
            effect.Sweep(150, 20, 70);
            effect.Noise(200, 2200, 700);
            effect.Noise(2000, 10000, 2000);
            effect.Noise(3000, 20000, 1000);
        });

        /// <summary>Shuts the effect device down. <see cref="Music.Shutdown" /> calls this for every host.</summary>
        public static void Shutdown()
        {
            lock (Gate)
            {
                Player.Dispose();
            }
        }

        /// <summary>
        ///     Records the cue, and unless the process-wide mute is on, renders and plays it. The record happens
        ///     either way so a muted host still shows what would have sounded.
        /// </summary>
        private static void Play(string cue, Action<EffectBuilder> recipe)
        {
            lock (Gate)
            {
                LastCue = cue;

                if (Music.Muted)
                    return;

                var effect = new EffectBuilder();
                recipe(effect);
                Player.Play(effect.ToPcm(), SampleRate);
                Player.SetVolume(Music.Volume);
            }
        }

        /// <summary>
        ///     Builds an effect as one continuous square wave whose frequency is re-tuned segment by segment —
        ///     which is the PC speaker itself: one oscillator, retuned by every <c>Sound()</c>, gated by
        ///     <c>NoSound</c>. The phase carries across segments so a re-tune never resets the wave, and the only
        ///     softening applied is a ~1.5 ms fade at the effect's outer edges (the same liberty
        ///     <see cref="SquareWaveSynth" /> takes, for the same reason: a raw step edge is the sound card's
        ///     artefact, not the speaker's).
        /// </summary>
        private sealed class EffectBuilder
        {
            private const short Amplitude = 6000;

            private readonly List<short> _samples = new(SampleRate * 2);
            private double _phase;
            private double _carry;

            /// <summary>Appends one tone, phase-continuous with whatever came before it.</summary>
            /// <param name="hertz">Pitch, clamped below Nyquist.</param>
            /// <param name="milliseconds">How long it holds.</param>
            public void Tone(double hertz, double milliseconds)
            {
                var count = SamplesFor(milliseconds);
                var step = Math.Min(hertz, SampleRate / 2.0 - 1) / SampleRate;

                for (var i = 0; i < count; i++)
                {
                    _phase += step;
                    if (_phase >= 1)
                        _phase -= 1;

                    _samples.Add(_phase < 0.5 ? Amplitude : (short) -Amplitude);
                }
            }

            /// <summary>Appends silence — the original's <c>NoSound</c> then <c>Delay</c>.</summary>
            /// <param name="milliseconds">How long the gap is.</param>
            public void Silence(double milliseconds)
            {
                var count = SamplesFor(milliseconds);
                for (var i = 0; i < count; i++)
                    _samples.Add(0);
            }

            /// <summary>
            ///     The original's linear glissando, stepping exactly as it did: a wide sweep moves
            ///     <c>Round(range/ms)</c> Hz every millisecond; a narrow one moves 1 Hz every
            ///     <c>ceil(ms/range)</c> milliseconds. The run ends when the pitch passes its target, so a wide
            ///     sweep's true length is <c>ceil(range/step)</c> ms — faithfully a shade off the nominal figure.
            /// </summary>
            /// <param name="fromHertz">Starting pitch.</param>
            /// <param name="toHertz">Target pitch.</param>
            /// <param name="milliseconds">Nominal length.</param>
            public void Sweep(int fromHertz, int toHertz, int milliseconds)
            {
                var range = Math.Abs(toHertz - fromHertz);
                if (range == 0)
                {
                    Tone(fromHertz, milliseconds);
                    return;
                }

                var direction = Math.Sign(toHertz - fromHertz);
                if (range >= milliseconds)
                {
                    var step = (int) Math.Round((double) range / milliseconds, MidpointRounding.AwayFromZero);
                    for (var hz = fromHertz; direction > 0 ? hz < toHertz : hz > toHertz; hz += direction * step)
                        Tone(hz, 1);
                    return;
                }

                var hold = (int) Math.Ceiling((double) milliseconds / range);
                for (var hz = fromHertz; direction > 0 ? hz < toHertz : hz > toHertz; hz += direction)
                    Tone(hz, hold);
            }

            /// <summary>
            ///     The original's noise generator: <c>4 × (count + 1)</c> back-to-back re-tunes to a random pitch
            ///     in the band, each held one <see cref="NoiseTickMilliseconds" /> tick.
            /// </summary>
            /// <param name="nearHertz">Bottom of the band.</param>
            /// <param name="farHertz">Top of the band.</param>
            /// <param name="count">The original's outer loop count.</param>
            public void Noise(int nearHertz, int farHertz, int count)
            {
                var range = Math.Abs(farHertz - nearHertz);
                for (var burst = 0; burst <= count; burst++)
                    for (var i = 0; i < 4; i++)
                        Tone(nearHertz + Random.Shared.Next(range), NoiseTickMilliseconds);
            }

            /// <summary>Fades the outer edges and packs the effect as signed 16-bit mono PCM.</summary>
            public byte[] ToPcm()
            {
                var ramp = Math.Min(64, _samples.Count / 2);
                for (var i = 0; i < ramp; i++)
                {
                    _samples[i] = (short) (_samples[i] * i / ramp);
                    var tail = _samples.Count - 1 - i;
                    _samples[tail] = (short) (_samples[tail] * i / ramp);
                }

                var pcm = new byte[_samples.Count * 2];
                for (var i = 0; i < _samples.Count; i++)
                {
                    pcm[i * 2] = (byte) (_samples[i] & 0xFF);
                    pcm[i * 2 + 1] = (byte) ((_samples[i] >> 8) & 0xFF);
                }

                return pcm;
            }

            /// <summary>
            ///     Milliseconds to samples with the remainder carried, so runs of sub-sample segments — the noise
            ///     ticks are four and a half samples each — keep the effect's total length exact.
            /// </summary>
            private int SamplesFor(double milliseconds)
            {
                _carry += milliseconds * SampleRate / 1000.0;
                var count = (int) _carry;
                _carry -= count;
                return count;
            }
        }
    }
}
