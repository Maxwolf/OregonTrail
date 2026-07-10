using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     Drives the game's <c>GameSimulationApp</c> headlessly and deterministically, reproducing the exact boot and
    ///     command pattern the game's own test harness uses (<c>SimulationTestBase</c>): create the singleton, tick twice to
    ///     build modules and menu mappings, then type-and-submit input followed by two ticks (dispatch, render). Screen text is
    ///     captured from the <c>ScreenBufferDirtyEvent</c> (the only way to read it — the buffer property is private).
    ///     When <see cref="WatchOptions" /> is supplied the frames are drawn to the console and paced for a human to watch;
    ///     otherwise nothing is rendered and there are no delays (training speed).
    /// </summary>
    public sealed class GameDriver : IDisposable
    {
        private const int InputHistorySize = 20;

        private readonly WatchOptions? _watch;
        private readonly LinkedList<string> _inputHistory = new();
        private bool _subscribed;

        public GameDriver(WatchOptions? watch = null)
        {
            _watch = watch;
        }

        /// <summary>Latest fully composited screen text (header + window body + prompt).</summary>
        public string LastScreen { get; private set; } = "";

        /// <summary>The last inputs sent, newest last, for crash reports.</summary>
        public IReadOnlyCollection<string> RecentInputs => _inputHistory;

        public int TickCount { get; private set; }
        public int CommandCount { get; private set; }

        public bool Alive => GameSimulationApp.Instance != null;
        public IWindow? Focused => GameSimulationApp.Instance?.WindowManager.FocusedWindow;
        public IForm? CurrentForm => Focused?.CurrentForm;
        public string WindowName => Focused?.GetType().Name ?? "";
        public string FormName => CurrentForm?.GetType().Name ?? "";
        public bool FormIsNull => CurrentForm == null;

        /// <summary>Whether the active form takes typed input (Y/N, quantity, text) vs. an ENTER-to-continue prompt.</summary>
        public bool InputFillsBuffer => CurrentForm?.InputFillsBuffer ?? false;

        /// <summary>True when a human is watching (rendered + paced).</summary>
        public bool Watching => _watch != null;

        public WatchOptions? Watch => _watch;

        /// <summary>A "thinking" line drawn under the game screen in watch mode.</summary>
        public string? StatusLine { get; set; }

        /// <summary>Boots a fresh game and leaves it focused on the main menu.</summary>
        public void Boot()
        {
            GameSimulationApp.Instance?.Destroy();
            GameSimulationApp.Create();
            GameSimulationApp.Instance!.SceneGraph.ScreenBufferDirtyEvent += OnScreen;
            _subscribed = true;

            // Tick 1 runs OnFirstTick -> Restart (build modules, attach Travel+MainMenu); tick 2 renders/builds mappings.
            RawTick();
            RawTick();
        }

        /// <summary>Types a command and submits it as if the player pressed ENTER, then dispatches and renders.</summary>
        public void Send(string command)
        {
            RecordInput(command);
            CommandCount++;

            var input = GameSimulationApp.Instance!.InputManager;
            foreach (var keyChar in command)
                input.AddCharToInputBuffer(keyChar);
            input.SendInputBufferAsCommand();

            RawTick(); // dispatch queued command
            RawTick(); // render resulting screen
        }

        /// <summary>Advances one deterministic logic tick without sending input (for self-advancing screens).</summary>
        public void Tick() => RawTick();

        /// <summary>Current window/form body text, computed on demand (side-effect free) for parsing menus and prompts.</summary>
        public string RenderWindowText() => Focused?.OnRenderWindow() ?? "";

        /// <summary>Redraws the current frame immediately (watch mode only) — used to show a "thinking" line before a pause.</summary>
        public void Repaint()
        {
            if (_watch != null)
                RenderFrame();
        }

        private void RawTick()
        {
            TickCount++;
            GameSimulationApp.Instance!.OnTick(false);

            if (_watch != null)
            {
                RenderFrame();
                if (_watch.TickDelayMs > 0)
                    Thread.Sleep(_watch.TickDelayMs);
            }
        }

        private void OnScreen(string content) => LastScreen = content;

        private void RecordInput(string command)
        {
            _inputHistory.AddLast(command.Length == 0 ? "<ENTER>" : command);
            while (_inputHistory.Count > InputHistorySize)
                _inputHistory.RemoveFirst();
        }

        public void Dispose()
        {
            if (_subscribed && GameSimulationApp.Instance != null)
                GameSimulationApp.Instance.SceneGraph.ScreenBufferDirtyEvent -= OnScreen;
            _subscribed = false;
            GameSimulationApp.Instance?.Destroy();
        }

        private void RenderFrame()
        {
            try
            {
                var lines = LastScreen.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                var width = Math.Max(1, Console.WindowWidth);
                var totalRows = Math.Max(2, Console.WindowHeight - 1);
                var contentRows = totalRows - 1; // reserve the bottom row for the "thinking" status bar

                for (var i = 0; i < contentRows; i++)
                {
                    Console.SetCursorPosition(0, i);
                    var row = i < lines.Length ? lines[i] : string.Empty;
                    Console.Write(row.PadRight(width));
                }

                Console.SetCursorPosition(0, contentRows);
                var status = string.IsNullOrEmpty(StatusLine) ? string.Empty : StatusLine;
                Console.Write(status.PadRight(width));
            }
            catch (IOException)
            {
                // No real console (redirected) — skip rendering.
            }
            catch (ArgumentOutOfRangeException)
            {
                // Console was resized very small — skip this frame.
            }
        }
    }
}
