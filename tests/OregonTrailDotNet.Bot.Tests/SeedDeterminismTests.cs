using System.Reflection;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Proves the seed-injection seam: a fixed seed makes an entire playthrough reproducible (the foundation the trainer's
    ///     common-random-numbers evaluation relies on), and different seeds actually steer the game's randomness. A deterministic
    ///     policy (the heuristic) is used so any variation comes purely from the game RNG, not the policy.
    /// </summary>
    public sealed class SeedDeterminismTests : IDisposable
    {
        static SeedDeterminismTests()
        {
            Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);
        }

        public void Dispose()
        {
            GameSimulationApp.Instance?.Destroy();
        }

        private static (GameOutcomeEnum Outcome, int Score, int Miles, int Days, int Survivors) Play(int seed)
        {
            var result = GamePlayer.PlayOnce(new HeuristicPolicy(), seed: seed);
            return (result.Outcome, result.Score, result.Miles, result.Days, result.Survivors);
        }

        [Fact]
        public void SameSeed_Reproduces_The_Whole_Game()
        {
            var first = Play(20250715);
            var second = Play(20250715);

            Assert.Equal(first, second);
        }

        [Fact]
        public void DifferentSeeds_Steer_The_Game_Randomness()
        {
            // Across a spread of seeds the deterministic heuristic can't produce identical (outcome, score, miles, days,
            // survivors) tuples unless the seed is genuinely driving the game RNG — so more than one distinct result proves
            // seeding takes effect. (The chance of eight seeds coinciding on all five fields is effectively zero.)
            var outcomes = new HashSet<(GameOutcomeEnum, int, int, int, int)>();
            for (var seed = 1; seed <= 8; seed++)
                outcomes.Add(Play(seed));

            Assert.True(outcomes.Count > 1, "Every seed produced the identical game — seeding is not influencing the RNG.");
        }
    }
}
