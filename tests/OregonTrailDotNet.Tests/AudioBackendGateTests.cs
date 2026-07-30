using System;
using System.Diagnostics;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the platform seam the audio stack gained when music and sound effects stopped being
    ///     Windows-only: <see cref="AudioDevice.Create" /> chooses one <see cref="IAudioDevice" /> per operating
    ///     system, and above it nothing knows or cares which.
    ///     <para>
    ///         The load-bearing tests here are the silence ones. Until there were Linux and macOS backends, a test
    ///         run made no sound off Windows because the platform refused - every <c>waveOut</c> call was behind an
    ///         <c>OperatingSystem.IsWindows()</c> check. That accident is now gone, so what keeps the suite and the
    ///         training bot quiet is <see cref="SceneHost.Graphical" /> and the mute, and both of those are
    ///         deliberate code that can be deleted by someone who does not know why it is there. Hence these.
    ///     </para>
    ///     <para>
    ///         Nothing in this fixture ever plays audibly: every path exercised is one that must decline to open a
    ///         device. A test that actually opened one would make noise on the machine running it.
    ///     </para>
    /// </summary>
    public class AudioBackendGateTests : IDisposable
    {
        private const string SomeCue = "landmarks/04-chimney-rock";
        private const string AnotherCue = "tombstone";

        private readonly bool _wasGraphical;
        private readonly bool _wasMuted;

        public AudioBackendGateTests()
        {
            _wasGraphical = SceneHost.Graphical;
            _wasMuted = Music.Muted;

            // Known-quiet starting point: no cue, no device, whatever an earlier fixture in this serial run left
            // behind. Music is process-wide, so asserting against inherited state would make these tests lie.
            SceneHost.Graphical = false;
            Music.Shutdown();
        }

        public void Dispose()
        {
            Music.Shutdown();

            if (Music.Muted != _wasMuted)
                Music.ToggleMute();

            SceneHost.Graphical = _wasGraphical;
        }

        [Fact]
        public void Create_PicksTheBackendForThisPlatform()
        {
            var device = AudioDevice.Create();

            var expected = OperatingSystem.IsWindows() ? "WaveOutPlayer"
                : OperatingSystem.IsMacOS() ? "AudioQueuePlayer"
                : OperatingSystem.IsLinux() ? "AlsaPlayer"
                : "SilentDevice";

            Assert.Equal(expected, device.GetType().Name);
        }

        [Theory]
        [InlineData(AudioStreamEnum.Sustained)]
        [InlineData(AudioStreamEnum.OneShot)]
        public void Create_TakesTheStreamKind_WithoutChangingWhichBackendItPicks(AudioStreamEnum stream)
        {
            // The kind is a hint about how the caller uses the device, not a different device: it decides only
            // whether an ALSA output is handed back when the sound ends. Music is sustained and stops by hand;
            // Sfx fires one-shots and never stops anything, so on an ALSA card with no software mixer a held
            // one-shot would take the card off the music for the rest of the run.
            var device = AudioDevice.Create(stream);

            Assert.Equal(AudioDevice.Create().GetType(), device.GetType());
            Assert.False(device.Ready);

            device.Dispose();
        }

        [Fact]
        public void Create_HandsBackADeviceThatIsNotYetOpen()
        {
            // Ready means "a device is open", from a successful Play until Stop. Choosing a backend opens nothing,
            // which is what lets the scene footer say "(no audio device)" on a machine that has none.
            Assert.False(AudioDevice.Create().Ready);
        }

        [Fact]
        public void TheContract_NeverThrows_WhateverOrderItIsCalledIn()
        {
            var device = AudioDevice.Create();

            // Every one of these is something a real host does: a volume set before anything plays (which is the
            // order Music.Play uses), a stop with nothing playing (every silent scene, every frame), and a play
            // of nothing at all. None may throw, and none may claim to have played silence.
            device.SetVolume(0.4);
            device.SetVolume(-5);
            device.SetVolume(12);
            device.Stop();

            Assert.False(device.Play([], 22050));
            Assert.False(device.Play([0x00, 0x01], 0));

            device.Stop();
            device.Dispose();
            device.Dispose();

            Assert.False(device.Ready);
        }

        [Fact]
        public void Warm_ReturnsAtOnce_AndNeverThrows()
        {
            // The whole point of Warm is that the platform's one-time setup does not land on the game's thread.
            // On Linux the work it moves off-thread can take seconds, so a Warm that blocked would be worse than
            // no Warm at all.
            var clock = Stopwatch.StartNew();
            AudioDevice.Warm();
            clock.Stop();

            Assert.True(clock.ElapsedMilliseconds < 500, $"Warm blocked for {clock.ElapsedMilliseconds} ms.");
        }

        [Fact]
        public void Headless_MusicPlay_RunsTheCueButOpensNoDevice()
        {
            Assert.False(SceneHost.Graphical);

            Music.Play(SomeCue);

            // The cue and its clock belong to the simulation: the slideshow holds each card for the length of its
            // tune, and it must hold for the same time on a host that cannot hear it. So the cue is still
            // "playing" - only the device was declined.
            Assert.NotNull(Music.Playing);
            Assert.Equal(SomeCue, Music.Playing.Key);
            Assert.False(Music.Finished);
            Assert.False(Music.Audible);
        }

        [Fact]
        public void Headless_ToggleMute_OpensNoDevice()
        {
            Music.Play(SomeCue);

            // Unmuting normally rejoins the running tune partway through, which means handing the remainder to a
            // device. Headless, that has to be declined too, or F8 would be a way around the gate.
            Music.ToggleMute();
            Assert.False(Music.Audible);

            Music.ToggleMute();
            Assert.False(Music.Audible);
        }

        [Fact]
        public void Muted_MusicPlay_OpensNoDevice_EvenWithTheDrawingOn()
        {
            if (!Music.Muted)
                Music.ToggleMute();

            SceneHost.Graphical = true;

            Music.Play(AnotherCue);

            Assert.NotNull(Music.Playing);
            Assert.Equal(AnotherCue, Music.Playing.Key);
            Assert.False(Music.Audible);
        }

        [Fact]
        public void AnUnknownCue_StopsTheMusicRatherThanThrowing()
        {
            Music.Play(SomeCue);
            Music.Play("landmarks/99-nowhere-at-all");

            Assert.Null(Music.Playing);
            Assert.True(Music.Finished);
            Assert.False(Music.Audible);
        }

        [Fact]
        public void VolumeKeys_ClampAndSurviveWithNoDeviceOpen()
        {
            for (var i = 0; i < 20; i++)
                Music.Adjust(-0.1);

            Assert.Equal(0, Music.Volume);

            for (var i = 0; i < 20; i++)
                Music.Adjust(0.1);

            Assert.Equal(1, Music.Volume);

            Music.Adjust(-0.6);
            Assert.Equal(0.4, Music.Volume, 3);
        }
    }
}
