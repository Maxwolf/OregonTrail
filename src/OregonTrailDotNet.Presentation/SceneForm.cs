using System.Diagnostics;
using System.Text;
using OregonTrailDotNet.Presentation.Audio;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     Base for every graphical scene, in the workbench and the game alike: a picture that is recomposed on a
    ///     clock and handed to the scene graph as a cached string.
    ///     <para>
    ///         The split matters. <c>OnRenderForm</c> is called every system tick — hundreds of times a second — so it
    ///         only ever returns an already-built string; the expensive part (compose the scene, resample it to ANSI)
    ///         happens in <see cref="OnTick" />, gated to the scene's own frame rate. Rendering in the render method
    ///         would resample the picture on every tick and throw almost all of it away.
    ///     </para>
    ///     <para>
    ///         Generic over the parent window's user data so the same machinery hosts forms under any window — the
    ///         workbench's menu window, the game's Travel, Graveyard and GameOver windows. Host-specific behavior
    ///         (what ESC means, whether the speed keys exist) is a handful of virtuals, not a fork of this class.
    ///     </para>
    /// </summary>
    /// <typeparam name="TData">The parent window's user-data type.</typeparam>
    public abstract class SceneForm<TData> : Form<TData> where TData : WindowData, new()
    {
        private readonly Stopwatch _clock = new();
        private bool _dirty = true;
        private int _subFrame;

        /// <summary>Initializes a new instance of the <see cref="SceneForm{TData}" /> class.</summary>
        /// <param name="window">The parent window.</param>
        protected SceneForm(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Scenes are steered, not typed at. Keeping the buffer closed also stops the scene graph drawing a
        ///     prompt line under the artwork.
        /// </summary>
        public override bool InputFillsBuffer => false;

        /// <summary>Whether this scene advances on its own; a still screen only recomposes when invalidated.</summary>
        protected virtual bool Animated => true;

        /// <summary>
        ///     Frames drawn per step of the scene's logic. One means the picture changes only when the logic does,
        ///     which is right for anything whose logic already steps at a natural animation rate.
        ///     <para>
        ///         Raise it where a single step moves things a long way. A scene is then free to draw the in-between
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
        ///     The tune that belongs on this screen, as an embedded score key under <c>music/</c> without its
        ///     extension (e.g. <c>landmarks/04-chimney-rock</c>, <c>tombstone</c>), or null for a scene the original
        ///     played no music on — which is most of them.
        ///     <para>
        ///         Scenes declare a cue and do nothing else about it: the base class starts it, swaps it when the
        ///         property's answer changes, stops it on the way out, and <see cref="Music" /> owns the one mute and
        ///         the one volume for the whole process. There is deliberately no way for a scene to have its own.
        ///     </para>
        ///     <para>
        ///         Read every frame and safe to compute, so a scene whose music depends on what it is showing — the
        ///         slideshow changes tune with the card — just returns a different string.
        ///     </para>
        /// </summary>
        protected virtual string? MusicCue => null;

        /// <summary>
        ///     Rows of text this scene prints above its picture, reserved so the picture still fits. Hosts whose
        ///     simulation prepends status lines to every frame (the game's OnPreRender adds two) must count those
        ///     rows here too.
        /// </summary>
        protected virtual int ReservedRows => 8;

        /// <summary>
        ///     The rate this scene starts at. Overridden by any scene whose natural cadence is not a
        ///     general-purpose animation speed.
        /// </summary>
        protected virtual int DefaultTicksPerSecond => 20;

        /// <summary>
        ///     Logic steps per second. The base rate is fixed per scene; the workbench's subclass overrides this to
        ///     read its per-section tuning store and binds the -/+ keys that adjust it. Game scenes stay fixed —
        ///     a player should not be able to fast-forward the Columbia.
        /// </summary>
        protected virtual int TicksPerSecond => DefaultTicksPerSecond;

        /// <summary>
        ///     When true, every unclaimed key — and an empty ENTER, which only ever arrives through
        ///     <see cref="OnInputBufferReturned" /> because the framework consumes it as buffer control — routes to
        ///     <see cref="OnDismiss" /> instead of <see cref="OnSectionKey(ConsoleKeyInfo)" />. This is the contract
        ///     for still screens (cards, the map, the tombstone): "press any key" must include the ENTER this game
        ///     has trained its players to press. ESC routes there too.
        /// </summary>
        protected virtual bool DismissOnAnyKey => false;

        /// <summary>
        ///     Where a dismissed still screen goes. Implementations decide the exit (usually stop the music and
        ///     change or clear the form); the base never assumes one.
        /// </summary>
        protected virtual void OnDismiss()
        {
        }

        /// <summary>
        ///     What ESC means here. Default is the workbench's: stop the music, drop back to the menu window
        ///     underneath. Game scenes override — the hunt ends early keeping the bag, the raft ignores it, still
        ///     screens treat it as a dismiss (which <see cref="OnKeyPressed" /> already routes when
        ///     <see cref="DismissOnAnyKey" /> is set).
        /// </summary>
        protected virtual void OnEscape()
        {
            Music.Stop();
            ClearForm();
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
            //
            // Do not rely on this firing: it only does when the parent window's OnWindowActivate chains base, and
            // the game's Travel window does not. The self-heal in OnTick covers that host; this hook covers the
            // workbench and costs nothing where it is dead.
            _clock.Restart();
            _subFrame = 0;
            FrameProgress = 0;
            Invalidate();
        }

        /// <inheritdoc />
        public override void OnTick(bool systemTick, bool skipDay)
        {
            base.OnTick(systemTick, skipDay);

            // The simulation's own once-per-second tick, forwarded so a scene that drives real game state (the
            // travel screen's day tick) has a place for it that is not tangled up with animation pacing.
            if (!systemTick)
                OnSimulationTick();

            if (Animated)
            {
                // The clock runs at the frame rate, which is the logic rate multiplied out by the sub-frames.
                var frames = Math.Max(1, FramesPerStep);
                var interval = TimeSpan.FromSeconds(1.0 / (TicksPerSecond * frames));
                if (_clock.Elapsed < interval)
                    return;

                // A long gap means something sat on top of this scene (an event window, a dialog) or the host
                // stalled. Resume cleanly — restart the clock, drop any half-drawn sub-step, recompose once (which
                // also re-evaluates the music cue) — rather than advancing into a catch-up frame. The threshold is
                // generous so a slow-but-normal cadence (1 tick/sec scenes) never trips it.
                if (_clock.Elapsed > TimeSpan.FromSeconds(Math.Max(1.0, 4 * interval.TotalSeconds)))
                {
                    _clock.Restart();
                    _subFrame = 0;
                    FrameProgress = 0;
                    Recompose();
                    return;
                }

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
        ///     Takes the whole <see cref="ConsoleKeyInfo" /> rather than the bare key, because a scene may need to
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
                    if (DismissOnAnyKey)
                        OnDismiss();
                    else
                        OnEscape();
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
                default:
                    if (DismissOnAnyKey)
                        OnDismiss();
                    else
                        OnSectionKey(keyInfo);
                    return;
            }
        }

        /// <inheritdoc />
        public override string OnRenderForm() => Frame;

        /// <inheritdoc />
        public override void OnInputBufferReturned(string input)
        {
            // Scenes take keys, not lines — but ENTER only ever arrives here (the framework consumes it as buffer
            // control before OnKeyPressed can see it), so a dismissable still screen honours it as the dismiss.
            if (DismissOnAnyKey && string.IsNullOrWhiteSpace(input))
                OnDismiss();
        }

        /// <summary>Loads artwork and builds the scene. Called once, when the scene is attached.</summary>
        protected abstract void Build();

        /// <summary>Advances one tick of the scene's own logic. Only called while <see cref="Animated" />.</summary>
        protected virtual void Advance()
        {
        }

        /// <summary>
        ///     The simulation's once-per-second tick (systemTick == false), independent of animation pacing. Most
        ///     scenes leave it empty; the game's drive scene runs its day logic here.
        /// </summary>
        protected virtual void OnSimulationTick()
        {
        }

        /// <summary>Renders the scene: heads-up text first, then the picture.</summary>
        protected abstract string Compose();

        /// <summary>Handles a key the base class did not claim.</summary>
        protected virtual void OnSectionKey(ConsoleKey key)
        {
        }

        /// <summary>
        ///     The same, with the shift state and character still attached. Scenes that do not care about either
        ///     override <see cref="OnSectionKey(ConsoleKey)" /> and are unaffected; a scene that needs to tell
        ///     <c>&lt;</c> from <c>,</c> overrides this one instead and falls back to <c>base</c> for the rest.
        /// </summary>
        /// <param name="keyInfo">The key exactly as the console reported it.</param>
        protected virtual void OnSectionKey(ConsoleKeyInfo keyInfo) => OnSectionKey(keyInfo.Key);

        /// <summary>Marks a still scene as needing one more compose.</summary>
        protected void Invalidate() => _dirty = true;

        /// <summary>
        ///     Options that fit a picture into what is left of the console after this scene's text. Recomputed each
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
        ///         Turn it off for a scene whose picture is the first thing it prints. Centring indents each row, and
        ///         the console is not cleared between frames, so those indented columns keep whatever the previous
        ///         screen left there — the menu shows through down the side of the artwork. A scene that prints text
        ///         above its picture never notices, because the text has already overwritten that area.
        ///     </para>
        /// </summary>
        protected virtual bool CenterPicture => true;

        /// <summary>
        ///     The control hints shown on the footer's second line before any music keys. The workbench appends its
        ///     -/+ speed hint here; game scenes usually keep the default or replace it with their own exit hint.
        /// </summary>
        protected virtual string FooterControlHints => "ESC menu";

        /// <summary>Builds the standard footer every scene shows under its picture.</summary>
        protected string Footer(string keys)
        {
            var text = new StringBuilder();
            text.AppendLine($"{keys}");
            text.Append(FooterControlHints);

            // Only where there is music to control. On the silent scenes — which is most of them, because the
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
            // Follows the scene's answer wherever it goes, which is what lets a cue track what is on screen
            // without any scene having to start, stop or swap it for itself.
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
