using OregonTrailDotNet.Presentation;

namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     Steers the Columbia run - the same <see cref="RaftGame" /> a player steers, read the same way: where the
    ///     raft is, where the rocks are drifting, and how long is left before the landing. Like
    ///     <see cref="HuntPilot" /> this is hands, not judgement; the Columbia offers no choices to learn, only a
    ///     river to get down.
    ///     <para>
    ///         The rudder is momentum, not position: a press nudges a drift that is then applied, so stopping in a
    ///         lane costs one press against the drift and holding it costs alternating presses. Everything below is
    ///         written in those terms rather than in "move left / move right".
    ///     </para>
    /// </summary>
    public static class RaftPilot
    {
        /// <summary>Lanes 0 and 17 are the banks; a raft in one is grinding against it.</summary>
        private const int SafeLow = 2;

        /// <summary>Room to spare on the landing side, so a late drift does not beach the raft early.</summary>
        private const int SafeHigh = 15;

        /// <summary>How far ahead to read the rocks. A rock crosses the raft in a handful of ticks.</summary>
        private const int LookAhead = 3;

        /// <summary>
        ///     The key to press this tick, or null to hold the current drift. Null is a normal answer: a raft already
        ///     drifting the right way wants no rudder at all.
        /// </summary>
        /// <param name="game">The run in progress.</param>
        public static ConsoleKey? NextKey(RaftGame game)
        {
            if (game == null || game.Outcome != RaftOutcomeEnum.Running)
                return null;

            var target = TargetLane(game);

            // Head for the target lane by setting the drift toward it; once there, kill the drift so the raft holds.
            var want = target > game.Lane ? 1 : target < game.Lane ? -1 : 0;
            if (game.Drift == want)
                return null;

            // Near raises the drift (toward lane 17 and the landing), Far lowers it.
            return want > game.Drift ? ConsoleKey.DownArrow : ConsoleKey.UpArrow;
        }

        /// <summary>
        ///     Where the raft wants to be. For most of the run that is clear water in midstream; from the moment the
        ///     landing is close enough to reach it is the far bank, because the window is only twenty ticks wide and
        ///     the raft crosses one lane a tick at best.
        /// </summary>
        private static int TargetLane(RaftGame game)
        {
            // The landing is open: take the last lane, which is now a landing rather than a bank.
            if (game.LandingWindowOpen)
                return RaftGame.LastLane;

            // Close to it: wait one lane short. Lane 17 before the window opens is not a landing at all - it is the
            // bank, and touching it costs a drowning roll, an oxen roll and half the supplies. Staging at 16 means
            // the landing is one tick away the moment it counts, at no risk while it does not.
            var ticksToCross = RaftGame.LastLane - game.Lane + 4;
            if (game.Tick >= RaftGame.LandingOpensAfter - ticksToCross)
                return RaftGame.LastLane - 1;

            // Otherwise sit in the widest bit of clear water, preferring to stay put over crossing traffic.
            var best = Math.Clamp(game.Lane, SafeLow, SafeHigh);
            var bestCost = int.MaxValue;

            for (var lane = SafeLow; lane <= SafeHigh; lane++)
            {
                if (game.LaneThreatened(lane, LookAhead))
                    continue;

                var cost = Math.Abs(lane - game.Lane);
                if (cost >= bestCost)
                    continue;

                best = lane;
                bestCost = cost;
            }

            return best;
        }
    }
}
