using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace OregonTrailDotNet.Presentation.Audio
{
    /// <summary>
    ///     The macOS <see cref="IAudioDevice" />: pushes a block of PCM at the default output through
    ///     AudioToolbox's AudioQueue API.
    ///     <para>
    ///         The closest of the three to <c>waveOut</c>. The whole tune is handed over as enqueued buffers, Core
    ///         Audio plays it on its own threads, and nothing here streams or services a callback - the callback
    ///         exists only because <c>AudioQueueNewOutput</c> insists on a non-null one.
    ///     </para>
    ///     <para>
    ///         The one hard difference from <c>waveOut</c>, and the reason this class is shaped the way it is:
    ///         giving the device back is <i>documented-synchronous</i>. <c>AudioQueueStop</c> "returns when the
    ///         audio queue has stopped", and stops the audio hardware with it; both it and
    ///         <c>AudioQueueDispose</c> have been seen in the field to take seconds. <see cref="Music.Play" />
    ///         calls into a device while holding <c>Music</c>'s lock on the game's one tick-and-draw thread, so
    ///         running that teardown there would freeze drawing, input and the mute key along with it. So stopping
    ///         "now" is done the cheap way - the per-queue volume parameter, which takes effect immediately - and
    ///         the orphaned queue is handed to the thread pool to be torn down at its leisure.
    ///     </para>
    ///     <para>
    ///         Everything is best-effort, exactly as on the other two platforms.
    ///     </para>
    /// </summary>
    internal sealed class AudioQueuePlayer : IAudioDevice
    {
        /// <summary>
        ///     The full framework path, and it has to be.
        ///     <para>
        ///         .NET treats a name containing a directory separator as an absolute path and hands it to
        ///         <c>dlopen</c> verbatim with no <c>lib</c> prefixing and no suffix variations, so a bare
        ///         <c>AudioToolbox</c> cannot resolve a framework. And no such file exists on disk on any
        ///         currently supported macOS - the framework lives in the dyld shared cache, which
        ///         <c>dlopen</c> knows how to consult and <see cref="File.Exists" /> does not. Never guard this
        ///         with a file check.
        ///     </para>
        /// </summary>
        private const string AudioToolbox = "/System/Library/Frameworks/AudioToolbox.framework/AudioToolbox";

        /// <summary><c>kAudioFormatLinearPCM</c> - the four-character code <c>'lpcm'</c>.</summary>
        private const uint FormatLinearPcm = 0x6C70636D;

        /// <summary>
        ///     <c>kAudioFormatFlagIsSignedInteger | kAudioFormatFlagIsPacked</c>. No big-endian flag, because
        ///     <c>kAudioFormatFlagsNativeEndian</c> is zero on both architectures this ships for.
        /// </summary>
        private const uint FlagsSigned16 = (1u << 2) | (1u << 3);

        /// <summary><c>kAudioQueueParam_Volume</c> - this queue's own gain, never the system mixer's.</summary>
        private const uint ParamVolume = 1;

        /// <summary>
        ///     How much audio goes into one queue buffer. Nothing documents a ceiling on a single allocation, so
        ///     rather than find out at a player's expense the tune is split and every status checked. Must stay a
        ///     multiple of the two-byte frame.
        /// </summary>
        private const int ChunkBytes = 512 * 1024;

        /// <summary>How long <see cref="Dispose" /> gives the pending teardowns before it stops caring.</summary>
        private static readonly TimeSpan DisposeGrace = TimeSpan.FromSeconds(1);

        /// <summary>Roots the native thunk for <see cref="OnBufferDone" />. Collected, it would be a crash.</summary>
        private static readonly AudioQueueOutputCallback Callback = OnBufferDone;

        private readonly Lock _gate = new();

        /// <summary>Set once all handed-off teardowns have finished, so <see cref="Dispose" /> can wait on them.</summary>
        private readonly ManualResetEventSlim _idle = new(true);

        private IntPtr _queue;
        private int _pending;
        private float _volume = 1;
        private bool _missing;

        /// <inheritdoc />
        /// <remarks>
        ///     True from a successful <see cref="Play" /> until <see cref="Stop" /> - a queue is open, which is
        ///     what <c>waveOut</c> means by it too, and deliberately not a claim that sound is still coming out.
        /// </remarks>
        public bool Ready => _queue != IntPtr.Zero;

        /// <inheritdoc />
        public bool Play(byte[] pcm, int sampleRate)
        {
            // A partial frame is undefined; Music.Remainder slices at a computed offset, so mask rather than trust.
            var usable = pcm.Length & ~1;

            if (!OperatingSystem.IsMacOS() || !Available || sampleRate <= 0 || usable == 0)
                return false;

            lock (_gate)
            {
                if (_missing)
                    return false;

                var queue = IntPtr.Zero;

                try
                {
                    Abandon(Exchange());

                    var format = new AudioStreamBasicDescription
                    {
                        SampleRate = sampleRate,
                        FormatId = FormatLinearPcm,
                        FormatFlags = FlagsSigned16,
                        BytesPerPacket = 2,
                        FramesPerPacket = 1,
                        BytesPerFrame = 2,
                        ChannelsPerFrame = 1,
                        BitsPerChannel = 16,
                        Reserved = 0
                    };

                    // A null run loop means Core Audio services the queue on one of its own threads, so there is
                    // no CFRunLoop for a console app to create or pump. inFlags is reserved and must be zero.
                    if (AudioQueueNewOutput(ref format, Marshal.GetFunctionPointerForDelegate(Callback),
                            IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, out queue) != 0 || queue == IntPtr.Zero)
                        return false;

                    for (var offset = 0; offset < usable;)
                    {
                        var take = Math.Min(ChunkBytes, usable - offset);

                        if (AudioQueueAllocateBuffer(queue, (uint) take, out var raw) != 0 || raw == IntPtr.Zero)
                            return FailBeforeStart(queue);

                        Marshal.Copy(pcm, offset, Marshal.ReadIntPtr(raw, BufferDataOffset), take);

                        // AudioQueueAllocateBuffer fills in the capacity and the data pointer but leaves the
                        // byte count at zero. Enqueue it as handed over and the queue plays a flawless silence
                        // that looks exactly like a machine with no sound card.
                        Marshal.WriteInt32(raw, BufferByteSizeOffset, take);

                        // Zero and null: packet descriptions are for variable-bit-rate formats only.
                        if (AudioQueueEnqueueBuffer(queue, raw, 0, IntPtr.Zero) != 0)
                            return FailBeforeStart(queue);

                        offset += take;
                    }

                    // Legal before Start, and the seam's rule is that playback begins at whatever SetVolume last
                    // asked for rather than at full scale.
                    AudioQueueSetParameter(queue, ParamVolume, _volume);

                    if (AudioQueueStart(queue, IntPtr.Zero) != 0)
                        return FailBeforeStart(queue);

                    _queue = queue;
                    return true;
                }
                catch (Exception e) when (IsMissing(e))
                {
                    _missing = true;
                    return FailBeforeStart(queue);
                }
                catch
                {
                    // The seam is absolute: whatever went wrong, this returns false rather than throwing into
                    // the middle of a frame.
                    return FailBeforeStart(queue);
                }
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            lock (_gate)
            {
                Abandon(Exchange());
            }
        }

        /// <inheritdoc />
        /// <remarks>
        ///     The queue's own gain, which is documented to take effect immediately and is not the user's system
        ///     volume. Remembered even with nothing open, so a level set before the first <see cref="Play" /> is
        ///     not dropped.
        /// </remarks>
        public void SetVolume(double level)
        {
            var scaled = (float) Math.Clamp(level, 0, 1);

            lock (_gate)
            {
                _volume = scaled;

                if (!OperatingSystem.IsMacOS() || !Available || _missing || _queue == IntPtr.Zero)
                    return;

                try
                {
                    AudioQueueSetParameter(_queue, ParamVolume, scaled);
                }
                catch (Exception e) when (IsMissing(e))
                {
                    _missing = true;
                }
                catch
                {
                    // ignored - best effort, exactly like the winmm sibling
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>
        ///     <see cref="Stop" /> plus a bounded wait, so a process that exits normally leaves nothing open.
        ///     Bounded because the synchronous teardown this is waiting on is the very call that has been seen to
        ///     take seconds, and a hung sound device must not stop the game from closing.
        /// </remarks>
        public void Dispose()
        {
            lock (_gate)
            {
                Abandon(Exchange());
            }

            // Outside the gate on purpose. The teardown takes no lock, and this must not invite one.
            _idle.Wait(DisposeGrace);
        }

        /// <summary>Takes the current queue, leaving nothing behind. The caller holds the gate.</summary>
        private IntPtr Exchange()
        {
            var queue = _queue;
            _queue = IntPtr.Zero;
            return queue;
        }

        /// <summary>
        ///     Silences a queue at once and hands it to the thread pool to be given back.
        ///     <para>
        ///         The volume parameter is the whole trick: it is one call, it is per-queue, and it takes effect
        ///         immediately, so the caller gets the "stops now" the seam promises without ever touching the
        ///         call that blocks.
        ///     </para>
        /// </summary>
        private void Abandon(IntPtr queue)
        {
            if (!OperatingSystem.IsMacOS() || queue == IntPtr.Zero)
                return;

            try
            {
                AudioQueueSetParameter(queue, ParamVolume, 0f);
            }
            catch
            {
                // ignored - the teardown below still has to happen
            }

            if (Interlocked.Increment(ref _pending) == 1)
                _idle.Reset();

            ThreadPool.UnsafeQueueUserWorkItem(
                static state => state.Player.Teardown(state.Queue), (Player: this, Queue: queue), false);
        }

        /// <summary>
        ///     Gives a queue back, off the game's thread. Nothing may escape: an unhandled exception on a pool
        ///     thread ends the process.
        /// </summary>
        private void Teardown(IntPtr queue)
        {
            // Re-asserted rather than declared: this runs from a thread-pool callback, and the compatibility
            // analyzer cannot see the caller's guard through it.
            if (!OperatingSystem.IsMacOS())
            {
                if (Interlocked.Decrement(ref _pending) == 0)
                    _idle.Set();

                return;
            }

            try
            {
                // Dispose frees every buffer the queue allocated, which is why none of them are freed by hand.
                AudioQueueStop(queue, 1);
                AudioQueueDispose(queue, 1);
            }
            catch
            {
                // ignored - the process exiting reclaims the client either way
            }
            finally
            {
                if (Interlocked.Decrement(ref _pending) == 0)
                    _idle.Set();
            }
        }

        /// <summary>
        ///     Disposes a queue that was built but never started, then reports failure.
        ///     <para>
        ///         No stop: there is nothing running to stop, and stopping a queue that never started is exactly
        ///         the call with the field reports of multi-second hangs. Disposing releases the queue and its
        ///         buffers on its own. Cleaning up the local handle rather than the field matters - zeroing the
        ///         field instead would leak a live queue and every buffer in it, permanently.
        ///     </para>
        /// </summary>
        private bool FailBeforeStart(IntPtr queue)
        {
            if (!OperatingSystem.IsMacOS() || queue == IntPtr.Zero)
                return false;

            try
            {
                AudioQueueDispose(queue, 1);
            }
            catch
            {
                // ignored
            }

            return false;
        }

        private static bool IsMissing(Exception e) =>
            e is DllNotFoundException or EntryPointNotFoundException or BadImageFormatException;

        /// <summary>
        ///     Does nothing, but <c>AudioQueueNewOutput</c> requires a real function pointer.
        ///     <para>
        ///         Two invariants, both load-bearing. It must never throw - it runs on a Core Audio thread, or on
        ///         whichever thread is inside <c>AudioQueueStop</c>, and an escaped exception there ends the
        ///         process. And it must never take <see cref="_gate" />: a synchronous stop invokes pending
        ///         callbacks on the calling thread, so a callback reaching for a lock that thread already holds
        ///         would deadlock the game against itself.
        ///     </para>
        /// </summary>
        private static void OnBufferDone(IntPtr userData, IntPtr queue, IntPtr buffer)
        {
        }

        /// <summary>
        ///     Whether there is a usable AudioToolbox to talk to, decided once.
        ///     <para>
        ///         A real load and a real symbol lookup, not a guess from
        ///         <see cref="OperatingSystem.IsMacOS" />. A P/Invoke binds lazily, at the first call to that
        ///         particular method, so without this the throw would land inside <see cref="Music.Play" />, from
        ///         a scene, mid-frame, and take the game down.
        ///     </para>
        /// </summary>
        private static readonly bool Available = OperatingSystem.IsMacOS() && Init();

        private static bool Init()
        {
            try
            {
                // The handle is deliberately never freed: the framework stays resident for the process, which is
                // what every one of these DllImports is about to assume.
                return NativeLibrary.TryLoad(AudioToolbox, out var lib) &&
                       NativeLibrary.TryGetExport(lib, "AudioQueueNewOutput", out _);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Resolves the framework now rather than at the first cue. Reading <see cref="Available" /> is what
        ///     does it - that is where the load lives. See <see cref="AudioDevice.Warm" />.
        /// </summary>
        internal static void Warm() => _ = Available;

        /// <summary>
        ///     The <c>AudioQueueBuffer</c> fields this code touches, by byte offset.
        ///     <para>
        ///         Read and written through <see cref="Marshal" /> rather than declared as a struct, because only
        ///         two of the seven fields matter here and the layout is the one thing that must not drift: each
        ///         <c>UInt32</c> before a pointer carries four bytes of tail padding on a 64-bit target, so a
        ///         field declared in the wrong place would turn playback into silence rather than into an error.
        ///     </para>
        /// </summary>
        private const int BufferDataOffset = 8;

        private const int BufferByteSizeOffset = 16;

        /// <summary>
        ///     A <c>Float64</c> followed by eight <c>UInt32</c>: 40 bytes, eight-byte aligned, no padding.
        ///     Natural packing already matches the C ABI on both architectures - do not add <c>Pack = 1</c>.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct AudioStreamBasicDescription
        {
            public double SampleRate;
            public uint FormatId;
            public uint FormatFlags;
            public uint BytesPerPacket;
            public uint FramesPerPacket;
            public uint BytesPerFrame;
            public uint ChannelsPerFrame;
            public uint BitsPerChannel;
            public uint Reserved;
        }

        /// <summary>The user-data pointer comes first, then the queue, then the buffer that finished.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void AudioQueueOutputCallback(IntPtr userData, IntPtr queue, IntPtr buffer);

        // OSStatus is a signed 32-bit code; the queue and buffer references are opaque pointers; Core Audio's
        // Boolean is an unsigned char, hence byte and never bool, whose default marshalling is a four-byte
        // Win32 BOOL. SupportedOSPlatform goes on the imports rather than the type, so that the factory and the
        // two facades can hold one of these without any platform ceremony of their own.

        [SupportedOSPlatform("macos")]
        [DllImport(AudioToolbox)]
        private static extern int AudioQueueNewOutput(ref AudioStreamBasicDescription format, IntPtr callback,
            IntPtr userData, IntPtr callbackRunLoop, IntPtr callbackRunLoopMode, uint flags, out IntPtr queue);

        [SupportedOSPlatform("macos")]
        [DllImport(AudioToolbox)]
        private static extern int AudioQueueAllocateBuffer(IntPtr queue, uint byteSize, out IntPtr buffer);

        [SupportedOSPlatform("macos")]
        [DllImport(AudioToolbox)]
        private static extern int AudioQueueEnqueueBuffer(IntPtr queue, IntPtr buffer, uint packetCount,
            IntPtr packets);

        [SupportedOSPlatform("macos")]
        [DllImport(AudioToolbox)]
        private static extern int AudioQueueStart(IntPtr queue, IntPtr startTime);

        [SupportedOSPlatform("macos")]
        [DllImport(AudioToolbox)]
        private static extern int AudioQueueStop(IntPtr queue, byte immediate);

        [SupportedOSPlatform("macos")]
        [DllImport(AudioToolbox)]
        private static extern int AudioQueueDispose(IntPtr queue, byte immediate);

        [SupportedOSPlatform("macos")]
        [DllImport(AudioToolbox)]
        private static extern int AudioQueueSetParameter(IntPtr queue, uint parameterId, float value);
    }
}
