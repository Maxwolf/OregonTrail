using System.Diagnostics;
using System.Text;
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

        /// <summary>Rows of text this section prints above its picture, reserved so the picture still fits.</summary>
        protected virtual int ReservedRows => 8;

        /// <summary>Ticks per second, shared across sections through the window's user data.</summary>
        protected int TicksPerSecond
        {
            get => UserData.TicksPerSecond;
            private set => UserData.TicksPerSecond = Math.Clamp(value, 1, 60);
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
            // thinks it has a huge amount of time to make up.
            _clock.Restart();
            Invalidate();
        }

        /// <inheritdoc />
        public override void OnTick(bool systemTick, bool skipDay)
        {
            base.OnTick(systemTick, skipDay);

            if (Animated)
            {
                if (_clock.Elapsed < TimeSpan.FromSeconds(1.0 / TicksPerSecond))
                    return;

                _clock.Restart();
                Advance();
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
                    ClearForm();
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
                CenterHorizontally = true
            };
        }

        /// <summary>Builds the standard footer every section shows under its picture.</summary>
        protected string Footer(string keys)
        {
            var text = new StringBuilder();
            text.AppendLine($"{keys}");
            text.Append($"ESC menu   -/+ speed ({TicksPerSecond}/sec)");
            return text.ToString();
        }

        private void Recompose()
        {
            Frame = Compose();
            _dirty = false;
        }
    }
}
