using System.Diagnostics;
using System.Text;
using OregonTrailDotNet.Minigames.Audio;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     Base for every workbench section: a picture that is recomposed on a clock and handed to the scene graph as
    ///     a cached string.
    ///     <para>
    ///         The split matters. <c>OnRenderForm</c> is called every system tick — hundreds of times a second — so it
    ///         only ever returns an already-built string; the expensive part (compose the scene, resample it to ANSI)
    ///         happens in <see cref="OnTick" />, gated to the section's own frame rate. Rendering in the render method
    ///         would resample the picture on every tick and throw almost all of it away.
    ///     </para>
    /// </summary>
    public abstract class SceneForm : Form<MinigameInfo>
    {
        private readonly Stopwatch _clock = new();
        private bool _dirty = true;
        private int _subFrame;

        /// <summary>Initializes a new instance of the <see cref="SceneForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        protected SceneForm(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Sections are steered, not typed at. Keeping the buffer closed also stops the scene graph drawing a
        ///     prompt line under the artwork.
        /// </summary>
        public override bool InputFillsBuffer => false;

        /// <summary>Whether this section advances on its own; a still screen only recomposes when invalidated.</summary>
        protected virtual bool Animated => true;

        /// <summary>
        ///     Frames drawn per step of the section's logic. One means the picture changes only when the logic does,
        ///     which is right for anything whose logic already steps at a natural animation rate.
        ///     <para>
        ///         Raise it where a single step moves things a long way. A section is then free to draw the in-between
        ///         positions using <see cref="FrameProgress" />, which keeps the simulation on the original's integer
        ///         grid — the thing collision boxes and lane maths depend on — while the motion on screen stays smooth.
        ///     </para>
        /// </summary>
        protected virtual int FramesPerStep => 1;

        /// <summary>
        ///     How far the picture is between the previous step and the current one, from 0 up to but not reaching 1.
        ///     Always 0 when <see cref="FramesPerStep" /> is 1.
        ///     <para>
        ///         Note this runs <i>behind</i> the logic rather than ahead of it: the frame drawn immediately after a
        ///         step shows the state <b>before</b> that step, and the following frames close the gap. Interpolating
        ///         forward would mean guessing at a step that has not happened, which shows up as things visibly
        ///         snapping back whenever the guess is wrong.
        ///     </para>
        /// </summary>
        protected double FrameProgress { get; private set; }

        /// <summary>
        ///     The tune that belongs on this screen, as a score under <c>legacy/music/</c> without its extension, or
        ///     null for a section the original played no music on — which is most of them.
        ///     <para>
        ///         Sections declare a cue and do nothing else about it: the base class starts it, swaps it when the
        ///         property's answer changes, stops it on the way out, and <see cref="Music" /> owns the one mute and
        ///         the one volume for the whole workbench. There is deliberately no way for a section to have its own.
        ///     </para>
        ///     <para>
        ///         Read every frame and safe to compute, so a section whose music depends on what it is showing — the
        ///         slideshow changes tune with the card, and with which port's card — just returns a different string.
        ///     </para>
        /// </summary>
        protected virtual string? MusicCue => null;

        /// <summary>Rows of text this section prints above its picture, reserved so the picture still fits.</summary>
        protected virtual int ReservedRows => 8;

        /// <summary>
        ///     The rate this section starts at, before the speed keys touch it. Overridden by any section whose
        ///     natural cadence is not a general-purpose animation speed.
        /// </summary>
        protected virtual int DefaultTicksPerSecond => 20;

        /// <summary>Ticks per second, kept per section in the window's user data so each keeps its own tuning.</summary>
        protected int TicksPerSecond
        {
            get => UserData.TicksPerSecond(GetType().Name, DefaultTicksPerSecond);
            private set => UserData.SetTicksPerSecond(GetType().Name, Math.Clamp(value, 1, 60));
        }

        /// <summary>The finished frame — heads-up text plus the rendered picture — rebuilt on the clock.</summary>
        private string Frame { get; set; } = string.Empty;

        /// <inheritdoc />
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            Build();
            _clock.Restart();
            Recompose();
        }

        /// <inheritdoc />
        public override void OnFormActivate()
        {
            base.OnFormActivate();

            // Back from a dialog, with the clock having run the whole time it was up; without this the next frame
            // thinks it has a huge amount of time to make up. Restart mid-step too, so the first frame back is a
            // whole one rather than a fragment of the step that was interrupted.
            _clock.Restart();
            _subFrame = 0;
            FrameProgress = 0;
            Invalidate();
        }

        /// <inheritdoc />
        public override void OnTick(bool systemTick, bool skipDay)
        {
            base.OnTick(systemTick, skipDay);

            if (Animated)
            {
                // The clock runs at the frame rate, which is the logic rate multiplied out by the sub-frames.
                var frames = Math.Max(1, FramesPerStep);
                if (_clock.Elapsed < TimeSpan.FromSeconds(1.0 / (TicksPerSecond * frames)))
                    return;

                _clock.Restart();
                if (_subFrame == 0)
                    Advance();

                FrameProgress = (double) _subFrame / frames;
                _subFrame = (_subFrame + 1) % frames;
                Recompose();
                return;
            }

            if (!_dirty)
                return;

            Recompose();
        }

        /// <summary>
        ///     Takes the whole <see cref="ConsoleKeyInfo" /> rather than the bare key, because a section may need to
        ///     tell a shifted key from its unshifted twin — <c>ConsoleKey.OemComma</c> is reported for both <c>,</c>
        ///     and <c>&lt;</c>, and only <c>KeyChar</c> separates them. This is the overload the framework dispatches;
        ///     its base implementation is what forwards to the <see cref="ConsoleKey" /> one.
        /// </summary>
        /// <param name="keyInfo">The key exactly as the console reported it.</param>
        public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Escape:
                    // Back to the menu. The window is still underneath with its choices already built.
                    Music.Stop();
                    ClearForm();
                    return;
                case ConsoleKey.F8:
                    Music.ToggleMute();
                    Invalidate();
                    return;
                case ConsoleKey.F9:
                    Music.Adjust(-0.1);
                    Invalidate();
                    return;
                case ConsoleKey.F10:
                    Music.Adjust(0.1);
                    Invalidate();
                    return;
                case ConsoleKey.OemMinus:
                case ConsoleKey.Subtract:
                    TicksPerSecond--;
                    Invalidate();
                    return;
                case ConsoleKey.OemPlus:
                case ConsoleKey.Add:
                    TicksPerSecond++;
                    Invalidate();
                    return;
                default:
                    OnSectionKey(keyInfo);
                    return;
            }
        }

        /// <inheritdoc />
        public override string OnRenderForm() => Frame;

        /// <inheritdoc />
        public override void OnInputBufferReturned(string input)
        {
            // Sections take keys, not lines; an ENTER that reaches here means nothing.
        }

        /// <summary>Loads artwork and builds the scene. Called once, when the section is attached.</summary>
        protected abstract void Build();

        /// <summary>Advances one tick of the section's own logic. Only called while <see cref="Animated" />.</summary>
        protected virtual void Advance()
        {
        }

        /// <summary>Renders the section: heads-up text first, then the picture.</summary>
        protected abstract string Compose();

        /// <summary>Handles a key the base class did not claim.</summary>
        protected virtual void OnSectionKey(ConsoleKey key)
        {
        }

        /// <summary>
        ///     The same, with the shift state and character still attached. Sections that do not care about either
        ///     override <see cref="OnSectionKey(ConsoleKey)" /> and are unaffected; a section that needs to tell
        ///     <c>&lt;</c> from <c>,</c> overrides this one instead and falls back to <c>base</c> for the rest.
        /// </summary>
        /// <param name="keyInfo">The key exactly as the console reported it.</param>
        protected virtual void OnSectionKey(ConsoleKeyInfo keyInfo) => OnSectionKey(keyInfo.Key);

        /// <summary>Marks a still section as needing one more compose.</summary>
        protected void Invalidate() => _dirty = true;

        /// <summary>
        ///     Options that fit a picture into what is left of the console after this section's text. Recomputed each
        ///     frame so the picture follows a resized window.
        /// </summary>
        protected AnsiImageOptions PictureOptions()
        {
            int rows;
            try
            {
                rows = Console.WindowHeight - ReservedRows;
            }
            catch (Exception)
            {
                // No real console, or one that will not answer. A guess is fine; failing to draw is not.
                rows = 24 - ReservedRows;
            }

            return new AnsiImageOptions
            {
                Fit = AnsiImageFitEnum.Contain,
                MaxRows = Math.Max(4, rows),
                CenterHorizontally = CenterPicture
            };
        }

        /// <summary>
        ///     Whether to centre the picture in the console.
        ///     <para>
        ///         Turn it off for a section whose picture is the first thing it prints. Centring indents each row, and
        ///         the console is not cleared between frames, so those indented columns keep whatever the previous
        ///         screen left there — the menu shows through down the side of the artwork. A section that prints text
        ///         above its picture never notices, because the text has already overwritten that area.
        ///     </para>
        /// </summary>
        protected virtual bool CenterPicture => true;

        /// <summary>Builds the standard footer every section shows under its picture.</summary>
        protected string Footer(string keys)
        {
            var text = new StringBuilder();
            text.AppendLine($"{keys}");
            text.Append($"ESC menu   -/+ speed ({TicksPerSecond}/sec)");

            // Only where there is music to control. On the silent sections — which is most of them, because the
            // original was silent there too — offering a mute key would be inviting the reader to go looking for
            // sound that was never meant to be playing.
            if (MusicCue == null)
                return text.ToString();

            text.Append(Music.Audible
                ? $"   F8 {(Music.Muted ? "unmute" : "mute")}   F9/F10 vol {Music.Volume * 100:0}%"
                : "   (no audio device)");

            return text.ToString();
        }

        private void Recompose()
        {
            // Follows the section's answer wherever it goes, which is what lets a cue track what is on screen
            // without any section having to start, stop or swap it for itself.
            var cue = MusicCue;
            if (cue == null)
                Music.Stop();
            else
                Music.Play(cue);

            Frame = Compose();
            _dirty = false;
        }
    }
}
