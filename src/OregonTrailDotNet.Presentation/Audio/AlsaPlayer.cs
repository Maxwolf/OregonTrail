using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace OregonTrailDotNet.Presentation.Audio
{
    /// <summary>
    ///     The Linux <see cref="IAudioDevice" />: pushes a block of PCM at the sound card through
    ///     <c>libasound</c>'s PCM API.
    ///     <para>
    ///         ALSA is the awkward one of the three. <c>waveOut</c> and AudioQueue both take a whole buffer and
    ///         play it on the driver's own time; ALSA has no such call - <c>snd_pcm_writei</c> only ever moves as
    ///         much as fits in the ring buffer right now, so somebody has to feed it for the length of the tune.
    ///         That somebody is one thread per play request, and the ownership rule it exists to enforce is
    ///         absolute: <b>one request, one device handle, one thread, for that handle's whole life.</b> No other
    ///         thread ever calls into <c>libasound</c> for that handle. Closing a handle while another thread sits
    ///         inside a write frees the <c>snd_pcm_t</c> underneath it, which is a segmentation fault rather than
    ///         an exception, so the rule is not a style preference.
    ///     </para>
    ///     <para>
    ///         The device is opened non-blocking and the writer waits in <see cref="snd_pcm_wait" /> slices, which
    ///         is what makes <see cref="Stop" /> quick without any cross-thread ALSA call at all: the writer
    ///         notices the cancel between slices. A blocking write cannot be interrupted - <c>snd_pcm_drop</c> from
    ///         another thread does not reliably wake one - so it is never used.
    ///     </para>
    ///     <para>
    ///         Everything is best-effort, exactly as on the other two platforms. No <c>libasound</c>, no card, or
    ///         every PCM device already taken, and each method here quietly does nothing. A missing sound server
    ///         is deliberately <i>not</i> on that list: <see cref="Devices" /> falls back past dmix to the card
    ///         itself, so bare ALSA with no daemon running plays perfectly well.
    ///     </para>
    /// </summary>
    /// <param name="stream">
    ///     Whether the caller sustains a sound and stops it by hand, or fires one-shots and never stops anything.
    ///     The other two backends have no use for this - every platform with a software mixer lets both streams
    ///     stay open - but it decides who gets the card on an ALSA output that has no mixer in front of it. See
    ///     <see cref="Worker" />.
    /// </param>
    internal sealed class AlsaPlayer(AudioStreamEnum stream) : IAudioDevice
    {
        /// <summary>
        ///     The versioned soname, and it has to be. .NET probes a name containing <c>.so.</c> as
        ///     <c>libasound.so.2</c> then <c>liblibasound.so.2</c> and so on, all of which is fine - but a plain
        ///     <c>asound</c> would look for the unversioned <c>libasound.so</c> symlink, which ships only in the
        ///     development package. Players do not install those.
        /// </summary>
        private const string Lib = "libasound.so.2";

        // include/pcm.h. Appended-to but never renumbered since 1998, which is the only reason hardcoding an
        // ALSA enum is defensible: every compiled ALSA program on earth has these baked in too.
        private const int StreamPlayback = 0;
        private const int FormatS16Le = 2;
        private const int AccessRwInterleaved = 3;
        private const int ModeNonblock = 0x00000001;
        private const int StatePrepared = 2;
        private const int StateRunning = 3;

        // asm-generic/errno.h. Same numbers on every Linux architecture this ships for.
        private const int Eintr = 4;
        private const int Eagain = 11;

        /// <summary>Ring-buffer size to ask for, in microseconds. The period lands at a quarter of it.</summary>
        private const uint LatencyUs = 80_000;

        /// <summary>
        ///     How long the writer parks per wait. This is the granularity of <see cref="Stop" />: a cue stops
        ///     within one slice of being told to, and a volume move lands within one slice too.
        /// </summary>
        private const int SliceMs = 10;

        /// <summary>How long <see cref="Stop" /> waits for the writer to acknowledge. Bounded on purpose.</summary>
        private const int JoinMs = 60;

        /// <summary>Slices of no progress at all before the writer gives up and leaves the rest unplayed.</summary>
        private const int NoProgressSlices = 2000;

        /// <summary>
        ///     Where to look for an output, in order.
        ///     <para>
        ///         <c>default</c> first, which on any desktop is a <c>plug</c> wrapper over dmix, PulseAudio or
        ///         PipeWire and mixes several streams - the arrangement this game needs, because an effect has to
        ///         sound over a tune. <c>plug:dmix</c> covers a bare-ALSA card whose configuration defines no mixer,
        ///         and <c>plughw:0,0</c> is the last resort. Never bare <c>hw</c>: these scores are 22050 Hz mono,
        ///         which no card plays natively, and <c>snd_pcm_set_params</c> refuses a rate it would have to
        ///         approximate rather than quietly resampling. Never bare <c>dmix</c> either - its slave defaults
        ///         to stereo at 48000 with no conversion in front of it.
        ///     </para>
        /// </summary>
        private static readonly string[] Devices = ["default", "plug:dmix", "plughw:0,0"];

        /// <summary>Roots the native thunk for <see cref="SilentHandler" />. Collected, it would be a crash.</summary>
        private static SndErrorHandler? _muzzle;

        /// <summary>Whichever device name last opened, tried first next time.</summary>
        private static volatile string? _lastGood;

        private readonly Lock _gate = new();

        /// <summary>Amplitude in Q15 - 32768 is unity. Read once per chunk, so a volume move lands live.</summary>
        private volatile int _gainQ15 = 1 << 15;

        private Job? _job;

        /// <inheritdoc />
        /// <remarks>
        ///     True from a successful <see cref="Play" /> until the device goes back - which for a
        ///     <see cref="AudioStreamEnum.Sustained" /> stream means until <see cref="Stop" />, exactly what
        ///     <c>waveOut</c> means by it, and deliberately <i>not</i> "audio is still coming out".
        /// </remarks>
        public bool Ready => Volatile.Read(ref _job) != null;

        /// <inheritdoc />
        public void Dispose() => Stop();

        /// <inheritdoc />
        public bool Play(byte[] pcm, int sampleRate)
        {
            // A partial frame is undefined; Music.Remainder slices at a computed offset, so mask rather than trust.
            var usable = pcm.Length & ~1;

            if (!OperatingSystem.IsLinux() || !Available || sampleRate <= 0 || usable == 0)
                return false;

            lock (_gate)
            {
                try
                {
                    StopLocked();

                    var handle = OpenAny(sampleRate);
                    if (handle == IntPtr.Zero)
                        return false;

                    // Copied because the writer outlives this call by the length of the tune, and the seam does
                    // not get to assume the caller will leave its array alone for that long.
                    var job = new Job(handle, pcm.AsSpan(0, usable).ToArray());
                    _job = job;

                    new Thread(() => Worker(job))
                    {
                        // Never hold up process exit. Music.Shutdown asks politely first; if a writer is wedged
                        // in the driver, Ctrl-C must still end the game.
                        IsBackground = true,
                        Name = "alsa-pcm"
                    }.Start();

                    return true;
                }
                catch
                {
                    // The seam is absolute: a missing symbol or a wrong-architecture libasound degrades to
                    // silence rather than reaching the game loop. StopLocked already cleared _job, so there is
                    // nothing here to unwind.
                    return false;
                }
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            lock (_gate)
            {
                try
                {
                    StopLocked();
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>
        ///     ALSA's PCM API has no volume control of any kind, so the gain is applied to the samples on their
        ///     way out. The two alternatives were both rejected on purpose: <c>snd_mixer</c> is the machine's
        ///     volume and this game has no business touching it, and the <c>softvol</c> plugin needs a
        ///     <c>.asoundrc</c> and publishes a control into the user's mixer. The cost of doing it here is that
        ///     a move lands one <see cref="SliceMs" /> late instead of instantly.
        /// </remarks>
        public void SetVolume(double level) => _gainQ15 = (int) (Math.Clamp(level, 0, 1) * 32768);

        /// <summary>
        ///     Opens and closes an output once, off the caller's thread, so the expensive one-time work is done
        ///     before a scene ever asks for a cue.
        ///     <para>
        ///         Opening the first PCM parses the whole of <c>/usr/share/alsa</c> and <c>dlopen</c>s the plugin
        ///         behind <c>default</c>; against a dead PulseAudio socket it can sit there for seconds. On the
        ///         game's thread that is a freeze, so it happens here instead, and the device name that worked is
        ///         remembered for the real <see cref="Play" />.
        ///     </para>
        /// </summary>
        internal static void Warm()
        {
            if (!OperatingSystem.IsLinux() || !Available)
                return;

            try
            {
                // 22050 is the music rate. Effects resample from the same plugin chain, so priming with either
                // one warms what matters.
                var handle = OpenAny(SquareWaveSynth.SampleRate);
                if (handle != IntPtr.Zero)
                    snd_pcm_close(handle);
            }
            catch
            {
                // ignored - this is an optimisation, and a failure here just means the first cue pays the cost
            }
        }

        /// <summary>
        ///     Cancels the current request. The caller holds the gate; the writer never takes it, so this cannot
        ///     deadlock, and the wait is bounded because the writer owns and closes its own handle either way.
        /// </summary>
        private void StopLocked()
        {
            var job = _job;
            _job = null;

            if (job == null)
                return;

            // Per-request, never a shared field. A shared cancel flag that the next Play cleared would let an
            // abandoned writer decide it had not been cancelled after all and play its whole tune underneath
            // the new one.
            job.Cancel.Set();
            job.Done.Wait(JoinMs);
        }

        /// <summary>
        ///     One play request, start to finish, on its own thread. Nothing may escape: this is not the game's
        ///     thread and an unhandled exception here would end the process.
        /// </summary>
        private void Worker(Job job)
        {
            // Re-asserted rather than declared: this runs from a thread start, and the compatibility analyzer
            // cannot see Play's guard through the closure that got us here.
            if (!OperatingSystem.IsLinux())
                return;

            try
            {
                Pump(job);
            }
            catch
            {
                // ignored - a native hiccup is silence, not a crash
            }

            try
            {
                // Who keeps the card, and why it is not the same answer for both facades.
                //
                // A sustained stream holds the device after its last sample, exactly as waveOut does, so that
                // Ready stays true for the whole scene - Music.Audible feeds the scene footer every frame, and a
                // device that closed itself the moment a tune ran out would change the footer's width mid-scene.
                // Stop wakes this immediately.
                //
                // A one-shot must do the opposite, and on one configuration it matters a great deal. Sfx never
                // calls Stop - an effect has no "over" for the caller to notice - so holding would mean the very
                // first gunshot kept an ALSA handle until the process exited. On an output with a software mixer
                // (dmix, PulseAudio, PipeWire) that is merely untidy. On an output without one - the plughw
                // fallback below, or a hand-written `pcm.!default hw:x,y` - it is fatal to the music: nothing
                // holds the card while an effect fires (no scene that plays an effect declares a cue, and the
                // travel screen stopped the music on its way in), so the effect wins it and would never give it
                // back, and every tune from that point on would find the device busy.
                if (stream == AudioStreamEnum.Sustained)
                    job.Cancel.Wait();
                else
                    // Pump has already waited for the ring to empty, so nothing is truncated. Only clear the
                    // instance's job if it is still ours: a Play that has replaced us owns the state now, and
                    // Ready is derived from that field precisely so this cannot race with it.
                    Interlocked.CompareExchange(ref _job, null, job);

                snd_pcm_drop(job.Pcm);
                snd_pcm_close(job.Pcm);
            }
            catch
            {
                // ignored
            }
            finally
            {
                job.Done.Set();
            }
        }

        /// <summary>Feeds the ring buffer until the tune runs out or the request is cancelled.</summary>
        [SupportedOSPlatform("linux")]
        private void Pump(Job job)
        {
            var total = job.Data.Length / 2;

            if (snd_pcm_get_params(job.Pcm, out _, out var period) < 0 || period == 0)
                period = 1024;

            var chunk = (int) Math.Clamp((ulong) period, 256UL, 8192UL);
            var staging = new short[chunk];

            // Unmanaged rather than a pinned managed array: a `fixed` held for the length of a tune would pin a
            // block through tens of seconds of a game that allocates every frame.
            var scratch = Marshal.AllocHGlobal(chunk * 2);

            try
            {
                var frame = 0;
                var idle = 0;

                while (frame < total && !job.Cancel.IsSet)
                {
                    var n = Math.Min(chunk, total - frame);
                    ApplyGain(job.Data, frame, n, staging);
                    Marshal.Copy(staging, 0, scratch, n);

                    var done = 0;
                    while (done < n && !job.Cancel.IsSet)
                    {
                        // Non-blocking writes are partial writes: honour the count that came back.
                        var written = snd_pcm_writei(job.Pcm, IntPtr.Add(scratch, done * 2), (nuint) (n - done));
                        if (written > 0)
                        {
                            done += (int) written;
                            idle = 0;
                            continue;
                        }

                        // A write can legitimately move zero frames without it being an error. Counting that as
                        // progress turns this loop into a spin that pegs a core for the length of the tune.
                        var err = written == 0 ? -Eagain : (int) written;

                        if (err == -Eagain)
                        {
                            var ready = snd_pcm_wait(job.Pcm, SliceMs);
                            if (ready >= 0)
                            {
                                // 1 is writable, 0 is the slice expiring. Either way, look at the cancel again.
                                if (++idle > NoProgressSlices)
                                    return;

                                continue;
                            }

                            err = ready;
                        }

                        if (err == -Eintr)
                            continue;

                        // An underrun or a suspended device is recoverable. silent = 1 because alsa-lib's own
                        // diagnostics would print straight over the artwork.
                        if (snd_pcm_recover(job.Pcm, err, 1) < 0)
                            return;

                        idle = 0;
                    }

                    frame += done;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(scratch);
            }

            if (job.Cancel.IsSet)
                return;

            // snd_pcm_set_params starts the stream only once the ring buffer is nearly full, so anything shorter
            // than the ring has been accepted and is sitting there unplayed. This line is why a gunshot is
            // audible at all.
            if (snd_pcm_state(job.Pcm) == StatePrepared)
                snd_pcm_start(job.Pcm);

            // Deliberately not snd_pcm_drain, which blocks and cannot be interrupted. The stream leaves RUNNING
            // by itself once the ring empties, and that is the signal this waits for.
            while (!job.Cancel.IsSet && snd_pcm_state(job.Pcm) == StateRunning)
            {
                if (snd_pcm_delay(job.Pcm, out var pending) < 0 || pending <= 0)
                    break;

                job.Cancel.Wait(SliceMs);
            }
        }

        /// <summary>
        ///     Scales one chunk of signed 16-bit mono into the staging buffer, always from the pristine source so
        ///     that repeated volume moves cannot accumulate rounding error.
        /// </summary>
        private void ApplyGain(byte[] src, int firstFrame, int frames, short[] dst)
        {
            var gain = _gainQ15;
            var samples = MemoryMarshal.Cast<byte, short>(src.AsSpan(firstFrame * 2, frames * 2));

            if (gain >= 1 << 15)
            {
                samples.CopyTo(dst);
                return;
            }

            for (var i = 0; i < frames; i++)
                dst[i] = (short) (samples[i] * gain >> 15);
        }

        /// <summary>Opens the first device in <see cref="Devices" /> that takes this rate, or nothing.</summary>
        [SupportedOSPlatform("linux")]
        private static IntPtr OpenAny(int sampleRate)
        {
            var first = _lastGood;
            if (first != null && TryOpen(first, sampleRate, out var cached))
                return cached;

            foreach (var name in Devices)
            {
                if (name == first || !TryOpen(name, sampleRate, out var pcm))
                    continue;

                _lastGood = name;
                return pcm;
            }

            return IntPtr.Zero;
        }

        [SupportedOSPlatform("linux")]
        private static bool TryOpen(string name, int sampleRate, out IntPtr pcm)
        {
            pcm = IntPtr.Zero;

            // Non-blocking is not an optimisation here. A blocking open does not fail when something else holds
            // the device - the kernel parks the caller until it is free, which on the game's thread is a hang.
            if (snd_pcm_open(out var handle, name, StreamPlayback, ModeNonblock) < 0)
                return false;

            // soft_resample = 1: 22050 mono is native on no card made this century, and set_params returns an
            // error rather than approximating a rate it was not asked to convert.
            if (snd_pcm_set_params(handle, FormatS16Le, AccessRwInterleaved, 1, (uint) sampleRate, 1,
                    LatencyUs) == 0)
            {
                pcm = handle;
                return true;
            }

            snd_pcm_close(handle);
            return false;
        }

        /// <summary>
        ///     Whether there is a usable <c>libasound</c> to talk to, decided once.
        ///     <para>
        ///         This has to be a real load and a real symbol lookup, not a guess from
        ///         <see cref="OperatingSystem.IsLinux" />. A P/Invoke binds lazily, at the first call to that
        ///         particular method - so on a box without the library the throw would otherwise land inside
        ///         <see cref="Music.Play" />, from a scene, mid-frame, and take the game down.
        ///     </para>
        /// </summary>
        private static readonly bool Available = OperatingSystem.IsLinux() && Init();

        private static bool Init()
        {
            try
            {
                if (!NativeLibrary.TryLoad(Lib, out var lib))
                    return false;

                // The handle is deliberately never freed: libasound stays resident for the process, which is
                // what every one of these DllImports is about to assume.
                if (!NativeLibrary.TryGetExport(lib, "snd_pcm_open", out _))
                    return false;

                Muzzle(lib);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Stops alsa-lib printing to standard error.
        ///     <para>
        ///         Left alone it writes lines like <c>ALSA lib pcm.c:8545:(snd_pcm_recover) underrun occurred</c>
        ///         straight into the terminal this game is painting a picture into - a smeared frame at best, a
        ///         scrolled console at worst. Passing null does not silence it (that restores the default
        ///         handler); it takes a real no-op.
        ///     </para>
        ///     <para>
        ///         Strictly best-effort, and resolved by hand rather than imported, because the entry point is
        ///         deprecated. The day it disappears the logs get noisy again - which must never be the same day
        ///         Linux loses its audio, and gating <see cref="Available" /> on this is exactly how that would
        ///         happen.
        ///     </para>
        /// </summary>
        private static void Muzzle(IntPtr lib)
        {
            try
            {
                if (!NativeLibrary.TryGetExport(lib, "snd_lib_error_set_handler", out var setter))
                    return;

                _muzzle = SilentHandler;
                Marshal.GetDelegateForFunctionPointer<SetErrorHandler>(setter)(
                    Marshal.GetFunctionPointerForDelegate(_muzzle));
            }
            catch
            {
                // ignored - noise on standard error is survivable; no audio is not
            }
        }

        private static void SilentHandler(IntPtr file, int line, IntPtr function, int err, IntPtr format)
        {
        }

        /// <summary>
        ///     The native handler is variadic and really is called with six arguments - the fifth is a <c>"%s"</c>
        ///     and the sixth the already-formatted message. Declaring five fixed parameters is safe because a
        ///     non-variadic callee simply never reads the register the extra one arrived in, on both of the
        ///     64-bit ABIs this ships for.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SndErrorHandler(IntPtr file, int line, IntPtr function, int err, IntPtr format);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SetErrorHandler(IntPtr handler);

        /// <summary>One play request and everything belonging to it, including the device it owns.</summary>
        private sealed class Job(IntPtr pcm, byte[] data)
        {
            public readonly ManualResetEventSlim Cancel = new(false);
            public readonly byte[] Data = data;
            public readonly ManualResetEventSlim Done = new(false);
            public readonly IntPtr Pcm = pcm;
        }

        // snd_pcm_uframes_t and snd_pcm_sframes_t are C `long`, so nuint and nint. Spelling either as uint or
        // int would truncate the frame count on every 64-bit target this ships for, which is all of them.

        [SupportedOSPlatform("linux")]
        [DllImport(Lib, ExactSpelling = true)]
        private static extern int snd_pcm_open(out IntPtr pcm,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name, int stream, int mode);

        [SupportedOSPlatform("linux")]
        [DllImport(Lib, ExactSpelling = true)]
        private static extern int snd_pcm_set_params(IntPtr pcm, int format, int access, uint channels, uint rate,
            int softResample, uint latencyMicroseconds);

        [SupportedOSPlatform("linux")]
        [DllImport(Lib, ExactSpelling = true)]
        private static extern int snd_pcm_get_params(IntPtr pcm, out nuint bufferSize, out nuint periodSize);

        [SupportedOSPlatform("linux")]
        [DllImport(Lib, ExactSpelling = true)]
        private static extern nint snd_pcm_writei(IntPtr pcm, IntPtr buffer, nuint frames);

        [SupportedOSPlatform("linux")]
        [DllImport(Lib, ExactSpelling = true)]
        private static extern int snd_pcm_wait(IntPtr pcm, int timeoutMilliseconds);

        [SupportedOSPlatform("linux")]
        [DllImport(Lib, ExactSpelling = true)]
        private static extern int snd_pcm_recover(IntPtr pcm, int err, int silent);

        [SupportedOSPlatform("linux")]
        [DllImport(Lib, ExactSpelling = true)]
        private static extern int snd_pcm_start(IntPtr pcm);

        [SupportedOSPlatform("linux")]
        [DllImport(Lib, ExactSpelling = true)]
        private static extern int snd_pcm_state(IntPtr pcm);

        [SupportedOSPlatform("linux")]
        [DllImport(Lib, ExactSpelling = true)]
        private static extern int snd_pcm_delay(IntPtr pcm, out nint delayFrames);

        [SupportedOSPlatform("linux")]
        [DllImport(Lib, ExactSpelling = true)]
        private static extern int snd_pcm_drop(IntPtr pcm);

        [SupportedOSPlatform("linux")]
        [DllImport(Lib, ExactSpelling = true)]
        private static extern int snd_pcm_close(IntPtr pcm);
    }
}
