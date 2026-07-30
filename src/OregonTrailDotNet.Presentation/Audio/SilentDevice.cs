namespace OregonTrailDotNet.Presentation.Audio
{
    /// <summary>
    ///     The device you get when there is no device: every call succeeds at doing nothing.
    ///     <para>
    ///         This is what keeps "best effort" from turning into a null check at every call site. A machine with
    ///         no sound card, a platform with no backend, a Linux box without <c>libasound</c> - all of them get one
    ///         of these, and <see cref="Music" /> and <see cref="Sfx" /> go on naming their cues exactly as they
    ///         would on a machine that can hear them. <see cref="Ready" /> is false, which is the one observable
    ///         difference, and it is how a scene footer knows to say so instead of offering volume keys.
    ///     </para>
    /// </summary>
    internal sealed class SilentDevice : IAudioDevice
    {
        /// <inheritdoc />
        public bool Ready => false;

        /// <inheritdoc />
        public bool Play(byte[] pcm, int sampleRate) => false;

        /// <inheritdoc />
        public void Stop()
        {
        }

        /// <inheritdoc />
        public void SetVolume(double level)
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
