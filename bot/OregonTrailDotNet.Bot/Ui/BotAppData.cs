using OregonTrailDotNet.Bot.Game;
using WolfCurses.Window;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>
    ///     Shared user-data for every form in the bot control-panel window stack. Because a
    ///     <see cref="WolfCurses.Window.WindowData" /> instance is discarded when the window is torn down, anything that must
    ///     survive the handoff into a training/watch game session lives on the process-wide
    ///     <see cref="OregonTrailDotNet.Bot.BotContext" /> instead — this object only carries in-flight form state.
    /// </summary>
    public sealed class BotAppData : WindowData
    {
        /// <summary>
        ///     Training-model key chosen on the model-selection screen, carried into the naming screen that finalizes creation.
        /// </summary>
        public string NewProfileModelKey { get; set; } = string.Empty;

        /// <summary>
        ///     Profile the user is currently configuring/selecting. -1 means "none selected yet".
        /// </summary>
        public long ActiveProfileId { get; set; } = -1;

        /// <summary>
        ///     Human readable name of <see cref="ActiveProfileId" /> shown in headers.
        /// </summary>
        public string ActiveProfileName { get; set; } = string.Empty;

        /// <summary>
        ///     Number of candidate genomes evaluated per CEM generation (population size).
        /// </summary>
        public int PopulationSize { get; set; } = 12;

        /// <summary>
        ///     Number of games averaged together to score a single candidate genome (noise smoothing).
        /// </summary>
        public int GamesPerCandidate { get; set; } = 5;

        /// <summary>
        ///     Number of CEM generations to run in a single training batch.
        /// </summary>
        public int Generations { get; set; } = 5;

        /// <summary>Playback speed selected on the watch-configuration screen.</summary>
        public WatchSpeed WatchSpeed { get; set; } = WatchSpeed.Medium;

        /// <summary>Whether the watch-configuration screen has looping (replay until Esc) turned on.</summary>
        public bool WatchLoop { get; set; }

        /// <summary>Automated-testing session length in minutes chosen on its config screen (0 = run until Esc).</summary>
        public int AutoTestMinutes { get; set; } = 5;

        /// <summary>Whether automated testing stops at the first problem (default) or keeps going and logs them all.</summary>
        public bool AutoTestStopOnProblem { get; set; } = true;

        /// <summary>Benchmark time limit in minutes chosen on its config screen (0 = until every model reaches the goal or Esc).</summary>
        public int BenchmarkMinutes { get; set; } = 5;

        /// <summary>Which goal the benchmark races each model to: a first win, or Stephen Meek's 7650.</summary>
        public Testing.BenchmarkGoal BenchmarkGoal { get; set; } = Testing.BenchmarkGoal.FirstWin;
    }
}
