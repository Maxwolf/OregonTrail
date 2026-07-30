namespace OregonTrailDotNet.Presentation.Audio
{
    /// <summary>
    ///     How a caller uses its <see cref="IAudioDevice" /> - which is the one thing about the caller a backend
    ///     sometimes has to know.
    ///     <para>
    ///         Two of the three platforms ignore this entirely, because <c>winmm</c> and Core Audio both mix as
    ///         many open output streams as you care to have. It exists for ALSA, where an output with no software
    ///         mixer in front of it can be held by exactly one stream, so whether a stream ever gives the card
    ///         back decides whether the other one is heard at all.
    ///     </para>
    /// </summary>
    public enum AudioStreamEnum
    {
        /// <summary>
        ///     A sound the caller keeps going and ends by hand, so the device is held until
        ///     <see cref="IAudioDevice.Stop" /> - the tune under a scene, which <see cref="Music" /> stops when
        ///     the scene changes. Holding it is also what keeps <see cref="IAudioDevice.Ready" /> steady for the
        ///     length of the scene.
        /// </summary>
        Sustained,

        /// <summary>
        ///     A sound that finishes on its own and is never stopped by anyone, so the device goes back the moment
        ///     it has played out - an <see cref="Sfx" /> effect, which has no "over" for its caller to notice.
        /// </summary>
        OneShot
    }
}
