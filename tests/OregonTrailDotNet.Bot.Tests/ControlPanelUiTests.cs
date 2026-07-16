using System.Reflection;
using OregonTrailDotNet;
using OregonTrailDotNet.Bot;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Game;
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
        public void Blank_Name_Defaults_To_The_Model_Name_And_Stays_Unique()
        {
            // Create with no name -> defaults to the chosen model's display name.
            Send("1"); // create
            Send("2"); // Genetic Algorithm
            Assert.Contains("Genetic Algorithm", Screen); // the prompt offers the model name as the default
            Send(""); // blank name

            Assert.Equal("Genetic Algorithm", BotContext.ActiveProfileName);
            var first = BotContext.Db!.Profiles.GetByName("Genetic Algorithm")!;
            Assert.Equal("genetic", first.PolicyKind);

            // A second blank Genetic create gets a distinct, suffixed name rather than re-activating the first.
            Send("1");
            Send("2");
            Send("");
            Assert.Equal("Genetic Algorithm 2", BotContext.ActiveProfileName);
            Assert.NotEqual(first.Id, BotContext.ActiveProfileId);
            Assert.Equal(2, BotContext.Db!.Profiles.All().Count);
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
            Send("9"); // Manage data
            Assert.Equal("ManageDataForm", FormName);
            Send("1"); // delete the active bot
            Assert.Equal("DeleteProfileConfirm", FormName);
            Assert.Contains("cannot be undone", Screen);
            Send("N");
            Assert.True(BotContext.Db!.Profiles.NameExists("Doomed"));

            // Confirm — it's gone.
            Send("9");
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

            Send("9"); // Manage data
            Send("2"); // Erase ALL data
            Assert.Equal("EraseAllConfirm", FormName);
            Assert.Contains("cannot be undone", Screen);
            Send("Y");

            Assert.Empty(BotContext.Db!.Profiles.All());
            Assert.Equal(0, BotContext.Db!.Leaderboard.Count());
            Assert.Equal(-1, BotContext.ActiveProfileId);
        }

        [Fact]
        public void Watch_Opens_Config_Screen_And_Start_Records_Speed_And_Loop()
        {
            Send("1"); Send("1"); Send("Scout"); // create a CEM bot and activate it
            Assert.True(BotContext.ActiveProfileId > 0);

            // "Watch" now leads to the configuration screen rather than launching immediately.
            Send("4");
            Assert.Equal("WatchConfigForm", FormName);
            Assert.Contains("Medium", Screen); // default speed shown
            Assert.Contains("single game", Screen); // looping off by default

            // Adjust the settings in place: choose Slow speed and turn looping on.
            Send("3");
            Assert.Contains("Slow", Screen);
            Send("4");
            Assert.Contains("until you press Esc", Screen);
            Assert.Equal("WatchConfigForm", FormName); // still configuring, not launched yet

            // ENTER records the watch request with the chosen options and tears the panel down.
            BotSimulationApp.Instance!.InputManager.SendInputBufferAsCommand();
            for (var i = 0; i < 2 && BotSimulationApp.Instance != null; i++)
                BotSimulationApp.Instance.OnTick(false);

            Assert.Null(BotSimulationApp.Instance); // panel torn down to hand off to the watch session
            var request = BotContext.Request!;
            Assert.Equal(BotRequestKindEnum.Watch, request.Kind);
            Assert.Equal(BotContext.ActiveProfileId, request.ProfileId);
            Assert.Equal(WatchSpeedEnum.Slow, request.WatchSpeed);
            Assert.True(request.LoopUntilEscape);
        }

        [Fact]
        public void Automated_Testing_Config_Records_Duration_And_Stop_Toggle()
        {
            // Reachable with no profile — automated testing uses its own transient bots.
            Send("7");
            Assert.Equal("AutoTestConfigForm", FormName);
            Assert.Contains("5 minutes", Screen); // default duration
            Assert.Contains("Stop the session", Screen); // default: stop on first problem

            Send("15"); // set the duration
            Assert.Contains("15 minutes", Screen);
            Send("s"); // toggle "on problem" to keep going
            Assert.Contains("Keep going", Screen);

            // ENTER records the request and tears the panel down.
            BotSimulationApp.Instance!.InputManager.SendInputBufferAsCommand();
            for (var i = 0; i < 2 && BotSimulationApp.Instance != null; i++)
                BotSimulationApp.Instance.OnTick(false);

            Assert.Null(BotSimulationApp.Instance);
            var request = BotContext.Request!;
            Assert.Equal(BotRequestKindEnum.AutoTest, request.Kind);
            Assert.Equal(15, request.AutoTestMinutes);
            Assert.False(request.AutoTestStopOnProblem); // toggled off
        }

        [Fact]
        public void Benchmark_Config_Records_Time_Limit_And_Goal()
        {
            // Reachable with no profile — the benchmark uses its own transient bots.
            Send("8");
            Assert.Equal("BenchmarkConfigForm", FormName);
            Assert.Contains("5 minutes", Screen); // default time limit
            Assert.Contains("First win", Screen); // default goal

            Send("g"); // switch goal to the Meek test
            Assert.Contains("Meek", Screen);
            Assert.Contains("7650", Screen);

            Send("12"); // set an explicit limit
            Assert.Contains("12 minutes", Screen);

            BotSimulationApp.Instance!.InputManager.SendInputBufferAsCommand(); // ENTER starts
            for (var i = 0; i < 2 && BotSimulationApp.Instance != null; i++)
                BotSimulationApp.Instance.OnTick(false);

            Assert.Null(BotSimulationApp.Instance);
            var request = BotContext.Request!;
            Assert.Equal(BotRequestKindEnum.Benchmark, request.Kind);
            Assert.Equal(12, request.BenchmarkMinutes);
            Assert.Equal(OregonTrailDotNet.Bot.Testing.BenchmarkGoalEnum.MeekScore, request.BenchmarkGoal);
        }

        [Fact]
        public void Training_Menu_Custom_Option_Prompts_For_A_Number_And_Records_It()
        {
            Send("1"); Send("1"); Send("Trainee"); // create + activate a CEM bot (model -> name)
            Assert.True(BotContext.ActiveProfileId > 0);

            Send("3"); // Start training -> the length menu
            Assert.Equal("TrainingConfigForm", FormName);
            Assert.Contains("Train until I press Esc", Screen);
            Assert.Contains("Custom number of generations", Screen);

            Send("5"); // 4 presets + the custom option; picking custom opens its own prompt (no tear-down yet)
            Assert.Equal("CustomGenerationsForm", FormName);

            // Type a specific count and submit. StartTraining tears the panel down, so tick with a null guard.
            foreach (var c in "37")
                BotSimulationApp.Instance!.InputManager.AddCharToInputBuffer(c);
            BotSimulationApp.Instance!.InputManager.SendInputBufferAsCommand();
            for (var i = 0; i < 2 && BotSimulationApp.Instance != null; i++)
                BotSimulationApp.Instance.OnTick(false);

            Assert.Null(BotSimulationApp.Instance);
            var request = BotContext.Request!;
            Assert.Equal(BotRequestKindEnum.Train, request.Kind);
            Assert.Equal(BotContext.ActiveProfileId, request.ProfileId);
            Assert.Equal(37, request.Generations);
            Assert.Equal(16, request.PopulationSize);
            Assert.Equal(64, request.GamesPerCandidate);
        }

        [Fact]
        public void Training_Menu_Until_Esc_Option_Records_An_Open_Ended_Run()
        {
            Send("1"); Send("1"); Send("Marathoner"); // create + activate a CEM bot
            Send("3"); // length menu
            Assert.Equal("TrainingConfigForm", FormName);

            // Option 4 is "Train until I press Esc" — recorded as a negative generation sentinel. It tears the panel down,
            // so submit it with the null-guarded tick pattern rather than the Send helper.
            BotSimulationApp.Instance!.InputManager.AddCharToInputBuffer('4');
            BotSimulationApp.Instance!.InputManager.SendInputBufferAsCommand();
            for (var i = 0; i < 2 && BotSimulationApp.Instance != null; i++)
                BotSimulationApp.Instance.OnTick(false);

            Assert.Null(BotSimulationApp.Instance);
            var request = BotContext.Request!;
            Assert.Equal(BotRequestKindEnum.Train, request.Kind);
            Assert.Equal(-1, request.Generations);
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
