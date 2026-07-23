using System.Diagnostics;

namespace OregonTrailDotNet.Presentation.Audio
{
    /// <summary>
    ///     The workbench's music, as one shared player with one shared mute.
    ///     <para>
    ///         Static on purpose. There is one sound card and one pair of ears, so there is one cue playing at a time
    ///         and one place that decides whether it is audible — a section says <i>what</i> should be playing and
    ///         nothing else. No section owns a mute key, a volume level, or a player instance, and none of them can
    ///         disagree about the state of the audio.
    ///     </para>
    ///     <para>
    ///         <see cref="Play" /> is idempotent by key, so the caller does not have to track what it already started;
    ///         a form is free to name its cue every single frame and the tune plays once through regardless.
    ///     </para>
    /// </summary>
    public static class Music
    {
        private static readonly WaveOutPlayer Player = new();
        private static readonly Stopwatch Clock = new();
        private static readonly Lock Gate = new();

        private static Chiptune? _cue;
        private static bool _muted;
        private static double _volume = 0.4;

        /// <summary>Whether sound is currently silenced. Global, and the only mute in the workbench.</summary>
        public static bool Muted
        {
            get
            {
                lock (Gate)
                {
                    return _muted;
                }
            }
        }

        /// <summary>Output level from 0 to 1.</summary>
        public static double Volume
        {
            get
            {
                lock (Gate)
                {
                    return _volume;
                }
            }
        }

        /// <summary>The cue that is playing, or null when nothing is.</summary>
        public static Chiptune? Playing
        {
            get
            {
                lock (Gate)
                {
                    return _cue;
                }
            }
        }

        /// <summary>
        ///     Whether the current cue has run its full length.
        ///     <para>
        ///         Timed off a clock rather than off the sound card, deliberately, so that muting, turning the volume
        ///         down, or running on a machine with no audio at all does not change a section's pacing. The
        ///         slideshow holds each card for the length of its tune, and it must hold for the same time whether or
        ///         not anyone can hear it.
        ///     </para>
        /// </summary>
        public static bool Finished
        {
            get
            {
                lock (Gate)
                {
                    return _cue == null || Clock.Elapsed >= _cue.Duration;
                }
            }
        }

        /// <summary>True when a sound device was opened; false means the workbench is running silent.</summary>
        public static bool Audible => Player.Ready;

        /// <summary>
        ///     Starts a cue, or does nothing if that same cue is already playing.
        /// </summary>
        /// <param name="key">
        ///     A score in the embedded music set, without its extension — <c>landmarks/04-chimney-rock</c>,
        ///     <c>tombstone</c>. An unknown key stops the music rather than throwing.
        /// </param>
        public static void Play(string key)
        {
            lock (Gate)
            {
                if (_cue?.Key == key)
                    return;

                var tune = Chiptune.Load(key);
                if (tune == null)
                {
                    StopLocked();
                    return;
                }

                _cue = tune;
                Clock.Restart();

                // Muted still counts as playing: the cue and its clock run either way, and only the sound is
                // withheld. Unmuting therefore drops back in partway through rather than restarting the tune.
                if (_muted)
                    return;

                Player.Play(SquareWaveSynth.Render(tune), SquareWaveSynth.SampleRate);
                Player.SetVolume(_volume);
            }
        }

        /// <summary>
        ///     Stops whatever is playing and forgets it. A no-op when nothing is, which matters because the silent
        ///     sections call this on every frame they draw.
        /// </summary>
        public static void Stop()
        {
            lock (Gate)
            {
                if (_cue == null)
                    return;

                StopLocked();
            }
        }

        /// <summary>Silences or restores the sound without disturbing the cue that is running.</summary>
        public static void ToggleMute()
        {
            lock (Gate)
            {
                _muted = !_muted;

                if (_muted)
                {
                    Player.Stop();
                    return;
                }

                // Rejoin the cue where its clock has got to, rather than starting it again from the top — the tune
                // has been "playing" silently the whole time and restarting it would contradict that.
                if (_cue == null || Clock.Elapsed >= _cue.Duration)
                    return;

                Player.Play(Remainder(_cue, Clock.Elapsed), SquareWaveSynth.SampleRate);
                Player.SetVolume(_volume);
            }
        }

        /// <summary>Moves the volume by a step, clamped, and unmutes if it is being turned up from silence.</summary>
        /// <param name="delta">How much to move it, positive or negative.</param>
        public static void Adjust(double delta)
        {
            lock (Gate)
            {
                _volume = Math.Clamp(_volume + delta, 0, 1);
                Player.SetVolume(_volume);
            }
        }

        /// <summary>Shuts the device down. Called when the workbench exits.</summary>
        public static void Shutdown()
        {
            lock (Gate)
            {
                StopLocked();
                Player.Dispose();
            }
        }

        private static void StopLocked()
        {
            _cue = null;
            Clock.Reset();
            Player.Stop();
        }

        /// <summary>
        ///     Renders only the part of a tune that has not been heard yet, so unmuting resumes rather than restarts.
        /// </summary>
        private static byte[] Remainder(Chiptune tune, TimeSpan elapsed)
        {
            var pcm = SquareWaveSynth.Render(tune);

            // Two bytes a sample, and the offset must land on a sample boundary or the buffer plays as noise.
            var offset = (int) (elapsed.TotalSeconds * SquareWaveSynth.SampleRate) * 2;
            if (offset <= 0)
                return pcm;

            return offset >= pcm.Length ? [] : pcm[offset..];
        }
    }
}
