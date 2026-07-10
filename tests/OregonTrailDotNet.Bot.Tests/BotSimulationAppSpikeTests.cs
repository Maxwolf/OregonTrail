using System.Reflection;
using OregonTrailDotNet;
using OregonTrailDotNet.Bot.Ui;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     De-risk spike for the plan's Section A: proves that the bot's own WolfCurses application
    ///     (<see cref="BotSimulationApp" />) can be created inside the same process as the game and discovers and renders its
    ///     own forms, even though the process entry assembly is pinned to the game (which the game's own EventFactory requires).
    /// </summary>
    public sealed class BotSimulationAppSpikeTests : IDisposable
    {
        static BotSimulationAppSpikeTests()
        {
            // Mirror the production/entry-assembly pin. Form discovery for the bot must still work because
            // FormFactory also scans the hosting app's own assembly + AdditionalFormAssemblies.
            Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);
        }

        public BotSimulationAppSpikeTests()
        {
            BotSimulationApp.Instance?.Destroy();
            BotSimulationApp.Create();

            // First tick runs OnFirstTick -> Restart (attaches BotMainMenu); second renders/builds mappings.
            BotSimulationApp.Instance!.OnTick(false);
            BotSimulationApp.Instance!.OnTick(false);
        }

        public void Dispose()
        {
            BotSimulationApp.Instance?.Destroy();
        }

        [Fact]
        public void ControlPanel_Boots_To_BotMainMenu()
        {
            var focused = BotSimulationApp.Instance!.WindowManager.FocusedWindow;
            Assert.NotNull(focused);
            Assert.IsType<BotMainMenu>(focused);
        }

        [Fact]
        public void ControlPanel_Renders_Its_Own_Forms_Discovered_From_Bot_Assembly()
        {
            var screen = BotSimulationApp.Instance!.WindowManager.FocusedWindow!.OnRenderWindow();

            Assert.False(string.IsNullOrWhiteSpace(screen));
            Assert.Contains("Create a new bot profile", screen);
            Assert.Contains("Quit", screen);
        }

        [Fact]
        public void ControlPanel_Can_Be_Destroyed_And_Recreated()
        {
            BotSimulationApp.Instance!.Destroy();
            Assert.Null(BotSimulationApp.Instance);

            BotSimulationApp.Create();
            BotSimulationApp.Instance!.OnTick(false);
            BotSimulationApp.Instance!.OnTick(false);

            Assert.IsType<BotMainMenu>(BotSimulationApp.Instance!.WindowManager.FocusedWindow);
        }
    }
}
