using System.Reflection;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Window.Travel;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Proves the load-bearing plumbing behind the bot's "stop when the wagon is full" behavior actually works end to end:
    ///     <see cref="GameSnapshot.Capture" /> must read the meat bagged so far straight off the live hunt on the focused
    ///     Travel window (Capture -> BaggedThisHunt -> Travel.ActiveHunt -> HuntGame.Pounds, dressed). Without this, HuntBagged
    ///     could silently read 0 forever — the bot would never stop early and every other test would still pass, since the
    ///     HuntStrategy tests inject HuntBagged directly and the playthrough tests don't assert early stopping.
    /// </summary>
    public sealed class GameSnapshotHuntTests : IDisposable
    {

        public GameSnapshotHuntTests()
        {
            GameSimulationApp.Instance?.Destroy();

            // A bot host draws nothing. Stated rather than assumed, because the flag is a process-wide static.
            SceneHost.Graphical = false;

            GameSimulationApp.Create();
            // Two ticks run Restart (builds modules + windows) and render, mirroring the game/bot boot.
            GameSimulationApp.Instance!.OnTick(false);
            GameSimulationApp.Instance!.OnTick(false);
        }

        public void Dispose() => GameSimulationApp.Instance?.Destroy();

        // Make the Travel window the focused one — exactly the situation during a real hunt, where the hunt scene is a
        // child of Travel. Boot leaves the main menu focused on top of Travel, so drop windows until Travel surfaces, then
        // return its shared UserData (a protected WolfCurses member) so a hunt can be seeded on the very instance
        // GameSnapshot.Capture will read back.
        private static TravelInfo FocusTravelWindow()
        {
            var game = GameSimulationApp.Instance;

            for (var i = 0; i < 5 && game.WindowManager.FocusedWindow is { } focused && focused is not Travel; i++)
            {
                focused.RemoveWindowNextTick();
                game.OnTick(false);
            }

            var travel = Assert.IsType<Travel>(game.WindowManager.FocusedWindow);
            return (TravelInfo) travel.GetType().BaseType!
                .GetProperty("UserData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                .GetValue(travel)!;
        }

        [Fact]
        public void HuntBagged_IsZero_WhenNotHunting()
        {
            // Fresh boot: the main menu is focused and no hunt exists, so the snapshot reports nothing bagged.
            Assert.Equal(0, GameSnapshot.Capture(GameSimulationApp.Instance).HuntBagged);
        }

        [Fact]
        public void HuntBagged_IsZero_WhenTheTravelWindowIsFocusedButNoHuntIsRunning()
        {
            // The discriminating case for the null guard: Travel is on top, which is the ordinary state of the game,
            // and only the absence of a hunt keeps the reading at zero.
            var travelInfo = FocusTravelWindow();
            Assert.Null(travelInfo.Hunt);

            Assert.Equal(0, GameSnapshot.Capture(GameSimulationApp.Instance).HuntBagged);
        }

        [Fact]
        public void HuntBagged_IsTheDressedWeight_OfTheFocusedTravelWindowsLiveHunt()
        {
            var game = GameSimulationApp.Instance;
            var travelInfo = FocusTravelWindow();

            // The same object HuntScene publishes when it builds the field; seeded so nothing here depends on a roll.
            var hunt = new HuntGame(seed: 1, bullets: 20);
            travelInfo.Hunt = hunt;

            // Raw pounds only move when a bullet lands on an animal, and where the animals spawn is the hunt's own
            // seeded roll — there is no bounded way to walk one under the muzzle from out here without writing a
            // second hunter. So the shot game is planted straight on the live object through Pounds' backing field:
            // what is under test is the wiring from that number to the snapshot, not how the number got there.
            typeof(HuntGame)
                .GetField("<Pounds>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(hunt, 350);

            // The whole point: Capture reads that live hunt off the focused Travel window rather than a hardcoded 0 —
            // and reports DRESSED pounds, what actually reaches the wagon, because that is what the bot's carry cap is
            // stated against. 350 lb on the ground is 175 lb on the walk back.
            var snapshot = GameSnapshot.Capture(game);
            Assert.Equal(HuntGame.Bag(350), snapshot.HuntBagged);
            Assert.Equal(175, snapshot.HuntBagged);

            // And it falls back to 0 the moment the hunt is over — which is what HuntSceneResult does with it.
            travelInfo.Hunt = null;
            Assert.Equal(0, GameSnapshot.Capture(game).HuntBagged);
        }
    }
}
