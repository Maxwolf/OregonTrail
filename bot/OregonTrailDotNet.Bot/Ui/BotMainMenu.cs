using System.Text;
using WolfCurses;
using WolfCurses.Window;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>
    ///     Root control-panel menu for the bot. Command handlers are filled in as the UI is built; for now the window exists so
    ///     the de-risk spike can prove the bot's own <see cref="SimulationApp" /> discovers and renders its forms.
    /// </summary>
    public sealed class BotMainMenu : Window<BotMainMenuCommandsEnum, BotAppData>
    {
        // ReSharper disable once UnusedMember.Global — constructed by the WolfCurses window factory via reflection.
        public BotMainMenu(SimulationApp simUnit) : base(simUnit)
        {
        }

        public override void OnWindowPostCreate()
        {
            var header = new StringBuilder();
            header.AppendLine($"{Environment.NewLine}Watch the bot learn to conquer the Oregon Trail.");
            header.AppendLine();
            header.Append("You may:");
            MenuHeader = header.ToString();

            AddCommand(CreateProfile, BotMainMenuCommandsEnum.CreateProfile);
            AddCommand(SelectProfile, BotMainMenuCommandsEnum.SelectProfile);
            AddCommand(StartTraining, BotMainMenuCommandsEnum.StartTraining);
            AddCommand(WatchGame, BotMainMenuCommandsEnum.WatchGame);
            AddCommand(ViewLeaderboard, BotMainMenuCommandsEnum.ViewLeaderboard);
            AddCommand(ViewStats, BotMainMenuCommandsEnum.ViewStats);
            AddCommand(AutomatedTesting, BotMainMenuCommandsEnum.AutomatedTesting);
            AddCommand(Benchmark, BotMainMenuCommandsEnum.Benchmark);
            AddCommand(ManageData, BotMainMenuCommandsEnum.ManageData);
            AddCommand(Quit, BotMainMenuCommandsEnum.Quit);
        }

        private void CreateProfile() => SetForm(typeof(SelectModelForm));

        private void SelectProfile() => SetForm(typeof(SelectProfileForm));

        private void StartTraining()
        {
            if (BotContext.ActiveProfileId < 0)
                SetForm(typeof(NoProfileForm));
            else
                SetForm(typeof(TrainingConfigForm));
        }

        private void WatchGame()
        {
            if (BotContext.ActiveProfileId < 0)
            {
                SetForm(typeof(NoProfileForm));
                return;
            }

            // Let the viewer configure speed/looping first; that form records the request and tears the panel down.
            SetForm(typeof(WatchConfigForm));
        }

        private void ViewLeaderboard() => SetForm(typeof(LeaderboardForm));

        // Automated testing spins up its own transient bots (one of every model), so it needs no active profile.
        private void AutomatedTesting() => SetForm(typeof(AutoTestConfigForm));

        // Benchmarking likewise uses its own transient bots — no active profile required.
        private void Benchmark() => SetForm(typeof(BenchmarkConfigForm));

        private void ManageData() => SetForm(typeof(ManageDataForm));

        private void ViewStats()
        {
            if (BotContext.ActiveProfileId < 0)
                SetForm(typeof(NoProfileForm));
            else
                SetForm(typeof(StatsForm));
        }

        private static void Quit()
        {
            BotContext.Request = new BotRequest { Kind = BotRequestKindEnum.Quit };
            BotSimulationApp.Instance?.Destroy();
        }
    }
}
