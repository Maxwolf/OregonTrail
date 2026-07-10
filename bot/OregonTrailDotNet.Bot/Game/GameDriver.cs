using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     Drives the game's <c>GameSimulationApp</c> headlessly and deterministically, reproducing the exact boot and
    ///     command pattern the game's own test harness uses (<c>SimulationTestBase</c>): create the singleton, tick twice to
    ///     build modules and menu mappings, then type-and-submit input followed by two ticks (dispatch, render). Screen text is
    ///     captured from the <c>ScreenBufferDirtyEvent</c> (the only way to read it — the buffer property is private); nothing
    ///     is written to the console unless <c>watch</c> is set.
    /// </summary>
    public sealed class GameDriver : IDisposable
    {
        private const int InputHistorySize = 20;

        private readonly bool _watch;
        private readonly int _watchDelayMs;
        private readonly LinkedList<string> _inputHistory = new();
        private bool _subscribed;

        public GameDriver(bool watch = false, int watchDelayMs = 35)
        {
            _watch = watch;
            _watchDelayMs = watchDelayMs;
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

        private void RawTick()
        {
            TickCount++;
            GameSimulationApp.Instance!.OnTick(false);

            if (_watch)
            {
                RenderToConsole(LastScreen);
                Thread.Sleep(_watchDelayMs);
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

        private static void RenderToConsole(string content)
        {
            try
            {
                var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                var height = Math.Max(1, Console.WindowHeight - 1);
                var width = Math.Max(1, Console.WindowWidth);
                for (var i = 0; i < height; i++)
                {
                    Console.SetCursorPosition(0, i);
                    var row = i < lines.Length ? lines[i] : string.Empty;
                    Console.Write(row.PadRight(width));
                }
            }
            catch (IOException)
            {
                // No real console (redirected) — skip rendering.
            }
        }
    }
}
