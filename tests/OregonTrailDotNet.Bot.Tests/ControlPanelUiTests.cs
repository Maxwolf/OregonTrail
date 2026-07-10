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
        public void Create_Profile_Flow_Picks_Model_Then_Names_And_Activates()
        {
            Send("1"); // Create a new bot profile -> model picker comes first
            Assert.Equal("SelectModelForm", FormName);
            Assert.Contains("Neuro-Evolution", Screen); // all models are listed

            Send("2"); // pick the 2nd model (Genetic Algorithm) -> naming screen
            Assert.Equal("CreateProfileForm", FormName);
            Assert.Contains("Genetic Algorithm", Screen); // the chosen model is shown while naming

            Send("Pioneers"); // profile name (with room for spaces)
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

            // Create a profile (model pick -> name), then stats should render for it.
            Send("1");
            Send("1"); // choose the default model (CEM)
            Send("Blazers");
            Assert.Equal("", FormName);

            Send("6");
            Assert.Equal("StatsForm", FormName);
            Assert.Contains("Blazers", Screen);
        }

        [Fact]
        public void Model_Picker_Fits_A_Default_80x24_Terminal()
        {
            var screen = "";
            BotSimulationApp.Instance!.SceneGraph.ScreenBufferDirtyEvent += s => screen = s;

            Send("1"); // open the model picker (first step of creating a profile)
            Assert.Equal("SelectModelForm", FormName);

            var lines = screen.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Assert.All(lines, line =>
                Assert.True(line.Length <= 80, $"line exceeds 80 columns ({line.Length}): {line}"));
            Assert.True(lines.Length <= 23,
                $"screen is {lines.Length} lines; a default 24-row terminal only shows 23, so it would run off.");
        }

        [Fact]
        public void Delete_Active_Bot_Removes_It_Only_After_Confirming()
        {
            Send("1"); Send("1"); Send("Doomed"); // create a CEM bot named "Doomed" (model -> name)
            Assert.True(BotContext.ActiveProfileId > 0);

            // Decline first — nothing is deleted.
            Send("7"); // Manage data
            Assert.Equal("ManageDataForm", FormName);
            Send("1"); // delete the active bot
            Assert.Equal("DeleteProfileConfirm", FormName);
            Assert.Contains("cannot be undone", Screen);
            Send("N");
            Assert.True(BotContext.Db!.Profiles.NameExists("Doomed"));

            // Confirm — it's gone.
            Send("7");
            Send("1");
            Send("Y");
            Assert.Equal(-1, BotContext.ActiveProfileId);
            Assert.False(BotContext.Db!.Profiles.NameExists("Doomed"));
            Assert.Empty(BotContext.Db!.Profiles.All());
        }

        [Fact]
        public void Erase_All_Wipes_Every_Bot_And_The_Leaderboard()
        {
            BotContext.Db!.Profiles.Create("Alpha", "cem");
            var bravoId = BotContext.Db!.Profiles.Create("Bravo", "genetic");
            BotContext.Db!.Leaderboard.Insert(new LeaderboardEntry
            {
                ProfileId = bravoId, Name = "Bravo", Score = 500, Rating = "Greenhorn"
            });
            Assert.Equal(2, BotContext.Db!.Profiles.All().Count);

            Send("7"); // Manage data
            Send("2"); // Erase ALL data
            Assert.Equal("EraseAllConfirm", FormName);
            Assert.Contains("cannot be undone", Screen);
            Send("Y");

            Assert.Empty(BotContext.Db!.Profiles.All());
            Assert.Equal(0, BotContext.Db!.Leaderboard.Count());
            Assert.Equal(-1, BotContext.ActiveProfileId);
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
