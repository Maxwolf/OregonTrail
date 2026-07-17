namespace OregonTrailDotNet.Bot
{
    /// <summary>
    ///     A transition request handed from the WolfCurses control-panel UI to the top-level <see cref="Program" /> state
    ///     machine. Because only one WolfCurses <c>SimulationApp</c> can own the console at a time, a form cannot launch a game
    ///     session itself; instead it records the request here and destroys the control-panel app, and <see cref="Program" />
    ///     acts on it.
    /// </summary>
    public sealed class BotRequest
    {
        public BotRequestKindEnum Kind { get; init; }
        public long ProfileId { get; init; }
        public int PopulationSize { get; init; }
        public int GamesPerCandidate { get; init; }
        public int Generations { get; init; }

        /// <summary>Watch mode: playback speed the viewer chose on the watch-config screen.</summary>
        public Game.WatchSpeedEnum WatchSpeed { get; init; } = Game.WatchSpeedEnum.Medium;

        /// <summary>Watch mode: keep replaying games until the viewer presses Esc.</summary>
        public bool LoopUntilEscape { get; init; }

        /// <summary>Automated testing: session length in minutes (0 = run until Esc).</summary>
        public int AutoTestMinutes { get; init; } = 5;

        /// <summary>Automated testing: stop the session as soon as a problem is found (vs. keep going and log them all).</summary>
        public bool AutoTestStopOnProblem { get; init; } = true;

        /// <summary>Benchmark: time limit in minutes (0 = run until every model reaches the goal or Esc).</summary>
        public int BenchmarkMinutes { get; init; } = 5;

        /// <summary>Benchmark: what each model races to reach (a first win, Stephen Meek's 7650, or the 13,860 ceiling).</summary>
        public Testing.BenchmarkGoalEnum BenchmarkGoal { get; init; } = Testing.BenchmarkGoalEnum.FirstWin;
    }
}
