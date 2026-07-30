namespace OregonTrailDotNet.Presentation.Audio
{
    /// <summary>
    ///     One output stream the presentation layer can push a finished tune or effect at.
    ///     <para>
    ///         Every platform gets its own implementation of exactly this - <see cref="WaveOutPlayer" /> over
    ///         <c>winmm</c> on Windows, <see cref="AudioQueuePlayer" /> over AudioToolbox on macOS,
    ///         <see cref="AlsaPlayer" /> over <c>libasound</c> on Linux, and <see cref="SilentDevice" /> where
    ///         there is nothing to play through. <see cref="AudioDevice.Create" /> picks between them once, and
    ///         nothing above this interface knows or cares which one it got.
    ///     </para>
    ///     <para>
    ///         The contract every implementation owes its callers:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <see cref="Play" /> <b>returns immediately</b>. The whole sound is handed over in one buffer and
    ///             the platform plays it out on its own time; a caller drawing a frame must never wait on audio.
    ///         </item>
    ///         <item>
    ///             <see cref="Stop" /> stops <b>now</b>, not at the end of the buffer, and hands the device back.
    ///         </item>
    ///         <item>
    ///             <see cref="SetVolume" /> moves <b>this stream</b> only. Reaching for the system mixer would let
    ///             the game turn down the machine, which it has no business doing.
    ///         </item>
    ///         <item>
    ///             Nothing here ever throws. No device, no driver, no audio library installed at all, and every
    ///             method quietly does nothing - a missing sound card must not stop the game from starting.
    ///         </item>
    ///     </list>
    /// </summary>
    public interface IAudioDevice : IDisposable
    {
        /// <summary>True while a device is open and audio is actually going out.</summary>
        bool Ready { get; }

        /// <summary>
        ///     Plays a block of signed 16-bit mono PCM, replacing whatever was playing, and returns without
        ///     waiting for it to finish.
        /// </summary>
        /// <param name="pcm">
        ///     The samples, little-endian, as produced by <see cref="SquareWaveSynth.Render" /> or the
        ///     <see cref="Sfx" /> effect builder.
        /// </param>
        /// <param name="sampleRate">Sample rate of that data.</param>
        /// <returns>True if the platform took it.</returns>
        bool Play(byte[] pcm, int sampleRate);

        /// <summary>Stops immediately and gives the device back.</summary>
        void Stop();

        /// <summary>
        ///     Sets output level on this stream alone, rather than on the system mixer.
        /// </summary>
        /// <param name="level">0 is silent, 1 is full; out-of-range values are clamped.</param>
        void SetVolume(double level);
    }
}
