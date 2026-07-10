using System.Reflection;
using OregonTrailDotNet;
using OregonTrailDotNet.Bot;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Ui;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>Drives the WolfCurses control-panel app through its menus to prove the forms navigate and touch the database.</summary>
    public sealed class ControlPanelUiTests : IDisposable
    {
        private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"botui_{Guid.NewGuid():N}.db");

        static ControlPanelUiTests() => Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);

        public ControlPanelUiTests()
        {
            BotContext.Db = new BotDatabase(_dbPath);
            BotContext.ActiveProfileId = -1;
            BotContext.ActiveProfileName = "";
            BotContext.Request = null;

            BotSimulationApp.Instance?.Destroy();
            BotSimulationApp.Create();
            BotSimulationApp.Instance!.OnTick(false);
            BotSimulationApp.Instance!.OnTick(false);
        }

        public void Dispose()
        {
            BotSimulationApp.Instance?.Destroy();
            BotContext.Db?.Dispose();
            BotContext.Db = null;
            BotContext.ActiveProfileId = -1;
            foreach (var s in new[] { "", "-wal", "-shm", "-journal" })
                if (File.Exists(_dbPath + s)) File.Delete(_dbPath + s);
        }

        private static void Send(string command)
        {
            foreach (var c in command)
                BotSimulationApp.Instance!.InputManager.AddCharToInputBuffer(c);
            BotSimulationApp.Instance!.InputManager.SendInputBufferAsCommand();
            BotSimulationApp.Instance!.OnTick(false);
            BotSimulationApp.Instance!.OnTick(false);
        }

        private static string FormName => BotSimulationApp.Instance!.WindowManager.FocusedWindow!.CurrentForm?.GetType().Name ?? "";
        private static string Screen => BotSimulationApp.Instance!.WindowManager.FocusedWindow!.OnRenderWindow();

        [Fact]
        public void Create_Profile_Flow_Picks_Model_Persists_And_Activates()
        {
            Send("1"); // Create a new bot profile
            Assert.Equal("CreateProfileForm", FormName);

            Send("Pioneers"); // profile name (with room for spaces)
            Assert.Equal("SelectModelForm", FormName); // now choose a training model
            Assert.Contains("Neuro-Evolution", Screen); // all models are listed

            Send("2"); // pick the 2nd model (Genetic Algorithm)
            Assert.Equal("", FormName); // back at the menu

            Assert.True(BotContext.ActiveProfileId > 0);
            Assert.Equal("Pioneers", BotContext.ActiveProfileName);
            var profile = BotContext.Db!.Profiles.GetByName("Pioneers")!;
            Assert.Equal("genetic", profile.PolicyKind); // the chosen model was stored
        }

        [Fact]
        public void Leaderboard_And_Stats_And_NoProfile_Guard_Render()
        {
            // Leaderboard is reachable with no profile.
            Send("5");
            Assert.Equal("LeaderboardForm", FormName);
            Assert.Contains("LEADERBOARD", Screen);
            Send(""); // ENTER dismisses -> back to menu
            Assert.Equal("", FormName);

            // Training/Stats without a profile shows the guard.
            Send("3");
            Assert.Equal("NoProfileForm", FormName);
            Send("");

            // Create a profile (name -> model pick), then stats should render for it.
            Send("1");
            Send("Blazers");
            Send("1"); // choose the default model (CEM)
            Assert.Equal("", FormName);

            Send("6");
            Assert.Equal("StatsForm", FormName);
            Assert.Contains("Blazers", Screen);
        }

        [Fact]
        public void Select_Profile_Lists_And_Activates()
        {
            BotContext.Db!.Profiles.Create("Alpha", "cem");
            BotContext.Db!.Profiles.Create("Bravo", "cem");

            Send("2"); // Select an existing profile
            Assert.Equal("SelectProfileForm", FormName);
            Assert.Contains("Alpha", Screen);
            Assert.Contains("Bravo", Screen);

            Send("1"); // pick the first listed profile
            Assert.True(BotContext.ActiveProfileId > 0);
        }
    }
}
