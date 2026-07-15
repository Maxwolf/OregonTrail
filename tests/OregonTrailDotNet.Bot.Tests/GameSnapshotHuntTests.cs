using System.Reflection;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.Hunt;
using Xunit;
using PersonEntity = OregonTrailDotNet.Entity.Person.Person;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Proves the load-bearing plumbing behind the bot's "stop when the wagon is full" behavior actually works end to end:
    ///     <see cref="GameSnapshot.Capture" /> must read the meat bagged so far straight off the live hunt on the focused
    ///     Travel window (Capture -> BaggedThisHunt -> Travel.ActiveHunt -> HuntManager.KillWeight). Without this, HuntBagged
    ///     could silently read 0 forever — the bot would never stop early and every other test would still pass, since the
    ///     HuntStrategy tests inject HuntBagged directly and the playthrough tests don't assert early stopping.
    /// </summary>
    public sealed class GameSnapshotHuntTests : IDisposable
    {
        static GameSnapshotHuntTests() => Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);

        public GameSnapshotHuntTests()
        {
            GameSimulationApp.Instance?.Destroy();
            GameSimulationApp.Create();
            // Two ticks run Restart (builds modules + windows) and render, mirroring the game/bot boot.
            GameSimulationApp.Instance.OnTick(false);
            GameSimulationApp.Instance.OnTick(false);
        }

        public void Dispose() => GameSimulationApp.Instance?.Destroy();

        // Make the Travel window the focused one — exactly the situation during a real hunt, where the Hunting form is a
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
        public void HuntBagged_ReflectsLiveKillWeight_OfTheFocusedTravelWindowsHunt()
        {
            var game = GameSimulationApp.Instance;
            var vehicle = game.Vehicle;
            vehicle.ResetVehicle();
            vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, "Hunter", true)); // leader for TryShoot's miss roll
            var ammo = vehicle.Inventory[EntitiesEnum.Ammo];
            ammo.AddQuantity(ammo.MaxQuantity);

            var travelInfo = FocusTravelWindow();
            travelInfo.GenerateHunt();
            var hunt = travelInfo.Hunt;

            // Force a deterministic kill so KillWeight > 0 — a freshly generated prey has TargetTime 0, so it clears both the
            // miss roll and the on-time gate. The target field is private; set it directly like the game's own hunt tests do.
            typeof(HuntManager).GetField("_target", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(hunt, new PreyItem());
            Assert.True(hunt.TryShoot());
            Assert.True(hunt.KillWeight > 0, "the forced kill should have registered weight");

            // The whole point: Capture reads that live weight off the focused Travel window rather than a hardcoded 0.
            var snapshot = GameSnapshot.Capture(game);
            Assert.Equal(hunt.KillWeight, snapshot.HuntBagged);
            Assert.True(snapshot.HuntBagged > 0);

            // And it falls back to 0 the moment the hunt is over.
            travelInfo.DestroyHunt();
            Assert.Equal(0, GameSnapshot.Capture(game).HuntBagged);
        }
    }
}
