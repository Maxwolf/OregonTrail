using System.Text;
using WolfCurses;
using WolfCurses.Window;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>
    ///     Root control-panel menu for the bot. Command handlers are filled in as the UI is built; for now the window exists so
    ///     the de-risk spike can prove the bot's own <see cref="SimulationApp" /> discovers and renders its forms.
    /// </summary>
    public sealed class BotMainMenu : Window<BotMainMenuCommands, BotAppData>
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

            AddCommand(CreateProfile, BotMainMenuCommands.CreateProfile);
            AddCommand(SelectProfile, BotMainMenuCommands.SelectProfile);
            AddCommand(StartTraining, BotMainMenuCommands.StartTraining);
            AddCommand(WatchGame, BotMainMenuCommands.WatchGame);
            AddCommand(ViewLeaderboard, BotMainMenuCommands.ViewLeaderboard);
            AddCommand(ViewStats, BotMainMenuCommands.ViewStats);
            AddCommand(ManageData, BotMainMenuCommands.ManageData);
            AddCommand(Quit, BotMainMenuCommands.Quit);
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

            // Hand off to Program to play (and render) one game with the active profile's best strategy.
            BotContext.Request = new BotRequest { Kind = BotRequestKind.Watch, ProfileId = BotContext.ActiveProfileId };
            BotSimulationApp.Instance?.Destroy();
        }

        private void ViewLeaderboard() => SetForm(typeof(LeaderboardForm));

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
            BotContext.Request = new BotRequest { Kind = BotRequestKind.Quit };
            BotSimulationApp.Instance?.Destroy();
        }
    }
}
