using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace OregonTrailDotNet.Presentation.Audio
{
    /// <summary>
    ///     Pushes a block of PCM at the sound card through <c>winmm</c>'s <c>waveOut</c> API.
    ///     <para>
    ///         The whole tune is rendered up front and handed over as a single buffer, which is what makes this small
    ///         enough to be worth hand-rolling: <c>waveOutWrite</c> returns immediately and the driver plays the buffer
    ///         on its own, so there is no streaming thread, no ring buffer and no callback to service. The longest
    ///         score on either disk is well under a minute, or about two megabytes at this rate.
    ///     </para>
    ///     <para>
    ///         Everything is best-effort. No device, no driver, or not Windows at all, and every method here quietly
    ///         does nothing — neither the sprite workbench nor the game must fail to start because a machine has no
    ///         sound card. <see cref="Ready" /> reports which way it went.
    ///     </para>
    /// </summary>
    public sealed class WaveOutPlayer : IDisposable
    {
        private const int WaveMapper = -1;
        private const int WaveFormatPcm = 1;

        private readonly Lock _gate = new();
        private IntPtr _device;
        private IntPtr _buffer;
        private IntPtr _header;

        /// <summary>True when a device was opened and audio is actually going out.</summary>
        public bool Ready => _device != IntPtr.Zero;

        /// <inheritdoc />
        public void Dispose() => Stop();

        /// <summary>
        ///     Plays a block of signed 16-bit mono PCM, replacing whatever was playing.
        /// </summary>
        /// <param name="pcm">The samples, as produced by <see cref="SquareWaveSynth.Render" />.</param>
        /// <param name="sampleRate">Sample rate of that data.</param>
        /// <returns>True if the sound card took it.</returns>
        public bool Play(byte[] pcm, int sampleRate)
        {
            if (!OperatingSystem.IsWindows() || pcm.Length == 0)
                return false;

            lock (_gate)
            {
                Release();

                var format = new WaveFormatEx
                {
                    FormatTag = WaveFormatPcm,
                    Channels = 1,
                    SamplesPerSecond = sampleRate,
                    BitsPerSample = 16,
                    BlockAlign = 2,
                    AverageBytesPerSecond = sampleRate * 2,
                    Size = 0
                };

                if (waveOutOpen(out _device, WaveMapper, ref format, IntPtr.Zero, IntPtr.Zero, 0) != 0)
                {
                    _device = IntPtr.Zero;
                    return false;
                }

                // Unmanaged, and deliberately not a pinned managed array: the driver keeps reading this buffer after
                // the call returns, so its lifetime is ours to manage until waveOutReset says the device is done.
                _buffer = Marshal.AllocHGlobal(pcm.Length);
                Marshal.Copy(pcm, 0, _buffer, pcm.Length);

                var header = new WaveHdr { Data = _buffer, BufferLength = pcm.Length };
                _header = Marshal.AllocHGlobal(Marshal.SizeOf<WaveHdr>());
                Marshal.StructureToPtr(header, _header, false);

                if (waveOutPrepareHeader(_device, _header, (uint) Marshal.SizeOf<WaveHdr>()) == 0 &&
                    waveOutWrite(_device, _header, (uint) Marshal.SizeOf<WaveHdr>()) == 0)
                    return true;

                Release();
                return false;
            }
        }

        /// <summary>Stops immediately and gives the device back.</summary>
        public void Stop()
        {
            lock (_gate)
            {
                Release();
            }
        }

        /// <summary>
        ///     Sets output level, 0 to 1, on this device alone rather than the system mixer.
        /// </summary>
        /// <param name="level">0 is silent, 1 is full.</param>
        public void SetVolume(double level)
        {
            if (!OperatingSystem.IsWindows())
                return;

            lock (_gate)
            {
                if (_device == IntPtr.Zero)
                    return;

                // One 16-bit level per channel, packed into the low and high halves of a 32-bit word.
                var scaled = (uint) (Math.Clamp(level, 0, 1) * 0xFFFF);
                waveOutSetVolume(_device, (scaled << 16) | scaled);
            }
        }

        /// <summary>Tears down whatever is currently open. The caller must already hold the lock.</summary>
        private void Release()
        {
            if (!OperatingSystem.IsWindows())
                return;

            if (_device != IntPtr.Zero)
            {
                // Reset before unpreparing: the header cannot be unprepared while it is still queued, and this is
                // also what makes the sound stop now rather than at the end of the tune.
                waveOutReset(_device);

                if (_header != IntPtr.Zero)
                    waveOutUnprepareHeader(_device, _header, (uint) Marshal.SizeOf<WaveHdr>());

                waveOutClose(_device);
                _device = IntPtr.Zero;
            }

            if (_header != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_header);
                _header = IntPtr.Zero;
            }

            if (_buffer == IntPtr.Zero)
                return;

            Marshal.FreeHGlobal(_buffer);
            _buffer = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WaveFormatEx
        {
            public ushort FormatTag;
            public ushort Channels;
            public int SamplesPerSecond;
            public int AverageBytesPerSecond;
            public ushort BlockAlign;
            public ushort BitsPerSample;
            public ushort Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WaveHdr
        {
            public IntPtr Data;
            public int BufferLength;
            public int BytesRecorded;
            public IntPtr User;
            public int Flags;
            public int Loops;
            public IntPtr Next;
            public IntPtr Reserved;
        }

        [SupportedOSPlatform("windows")]
        [DllImport("winmm.dll")]
        private static extern int waveOutOpen(out IntPtr device, int deviceId, ref WaveFormatEx format,
            IntPtr callback, IntPtr instance, int flags);

        [SupportedOSPlatform("windows")]
        [DllImport("winmm.dll")]
        private static extern int waveOutPrepareHeader(IntPtr device, IntPtr header, uint size);

        [SupportedOSPlatform("windows")]
        [DllImport("winmm.dll")]
        private static extern int waveOutWrite(IntPtr device, IntPtr header, uint size);

        [SupportedOSPlatform("windows")]
        [DllImport("winmm.dll")]
        private static extern int waveOutUnprepareHeader(IntPtr device, IntPtr header, uint size);

        [SupportedOSPlatform("windows")]
        [DllImport("winmm.dll")]
        private static extern int waveOutReset(IntPtr device);

        [SupportedOSPlatform("windows")]
        [DllImport("winmm.dll")]
        private static extern int waveOutClose(IntPtr device);

        [SupportedOSPlatform("windows")]
        [DllImport("winmm.dll")]
        private static extern int waveOutSetVolume(IntPtr device, uint volume);
    }
}
