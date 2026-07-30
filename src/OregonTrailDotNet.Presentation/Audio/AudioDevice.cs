namespace OregonTrailDotNet.Presentation.Audio
{
    /// <summary>
    ///     Picks the <see cref="IAudioDevice" /> for the machine the game is running on.
    ///     <para>
    ///         One decision, made once, in one place - which is the whole point of the seam. Above here nothing
    ///         knows which platform it is on: <see cref="Music" /> and <see cref="Sfx" /> each hold an
    ///         <see cref="IAudioDevice" /> and name their cues, and the three backends differ only in how a
    ///         finished buffer reaches the speakers.
    ///     </para>
    ///     <para>
    ///         Note what is <i>not</i> decided here. Whether sound is <b>audible</b> is
    ///         <see cref="Music.Muted" /> and <see cref="SceneHost.Graphical" />; whether a device could be
    ///         opened at all is the backend's own business, reported through <see cref="IAudioDevice.Ready" />.
    ///         This method only answers "which library does this operating system speak", and it never fails -
    ///         an unrecognised platform gets a <see cref="SilentDevice" /> and the game plays on in silence.
    ///     </para>
    /// </summary>
    public static class AudioDevice
    {
        /// <summary>Creates an output for this platform. Never returns null and never throws.</summary>
        /// <param name="stream">
        ///     How the caller intends to use it. Only <see cref="AlsaPlayer" /> reads this, and only because an
        ///     ALSA output without a software mixer can be held by one stream at a time - see
        ///     <see cref="AudioStreamEnum" />.
        /// </param>
        public static IAudioDevice Create(AudioStreamEnum stream = AudioStreamEnum.Sustained)
        {
            if (OperatingSystem.IsWindows())
                return new WaveOutPlayer();

            if (OperatingSystem.IsMacOS())
                return new AudioQueuePlayer();

            if (OperatingSystem.IsLinux())
                return new AlsaPlayer(stream);

            return new SilentDevice();
        }

        /// <summary>
        ///     Does the platform's one-time audio setup now, on a thread of its own, so that the first cue of the
        ///     game does not pay for it mid-frame.
        ///     <para>
        ///         Worth a whole method because the cost is wildly uneven across the three. Windows resolves
        ///         <c>winmm</c> and macOS resolves AudioToolbox out of the shared cache, both of which are quick.
        ///         Linux is the reason this exists: opening the first PCM parses the entire <c>/usr/share/alsa</c>
        ///         configuration tree and loads the plugin behind <c>default</c>, and against a sound server that
        ///         is configured but not running it can sit there for seconds. On the game's tick-and-draw thread
        ///         that is a visible freeze; here it is nothing at all, because the title screen is already up.
        ///     </para>
        ///     <para>
        ///         Strictly an optimisation - skipping it costs a stutter at the first cue, never the audio
        ///         itself. Hosts that draw nothing should not call it: a headless run has no business opening a
        ///         sound device, which is why the game's own <c>Program.Main</c> calls this alongside setting
        ///         <c>PresentationEnabled</c>, and the training bot never does.
        ///     </para>
        /// </summary>
        public static void Warm()
        {
            try
            {
                new Thread(WarmCore)
                {
                    IsBackground = true,
                    Name = "audio-warm"
                }.Start();
            }
            catch
            {
                // ignored - the first cue will do this work instead
            }
        }

        private static void WarmCore()
        {
            try
            {
                if (OperatingSystem.IsLinux())
                {
                    AlsaPlayer.Warm();
                    return;
                }

                if (OperatingSystem.IsMacOS())
                    AudioQueuePlayer.Warm();
            }
            catch
            {
                // ignored - nothing here is required for sound to work, only for it to start smoothly
            }
        }
    }
}
