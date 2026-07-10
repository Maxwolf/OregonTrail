using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>Collects how many generations to train, then hands control to <see cref="Program" /> to run the batch.</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class TrainingConfigForm : Form<BotAppData>
    {
        private const int DefaultPopulation = 16;
        private const int DefaultGamesPerCandidate = 5;
        private const int DefaultGenerations = 10;

        // ReSharper disable once UnusedMember.Global
        public TrainingConfigForm(IWindow window) : base(window)
        {
        }

        public override bool InputFillsBuffer => true;

        public override string OnRenderForm() =>
            $"{Environment.NewLine}Train profile '{BotContext.ActiveProfileName}'.{Environment.NewLine}{Environment.NewLine}" +
            $"Each generation evaluates {DefaultPopulation} strategies over {DefaultGamesPerCandidate} games each " +
            $"({DefaultPopulation * DefaultGamesPerCandidate} games/generation).{Environment.NewLine}{Environment.NewLine}" +
            $"How many generations? (ENTER for {DefaultGenerations}, or 0 to cancel)";

        public override void OnInputBufferReturned(string input)
        {
            var generations = DefaultGenerations;
            if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out var parsed))
                generations = parsed;

            if (generations <= 0)
            {
                ClearForm();
                return;
            }

            // Record the request and tear down the control panel; Program will run the training batch and then rebuild it.
            BotContext.Request = new BotRequest
            {
                Kind = BotRequestKind.Train,
                ProfileId = BotContext.ActiveProfileId,
                Generations = generations,
                PopulationSize = DefaultPopulation,
                GamesPerCandidate = DefaultGamesPerCandidate
            };
            BotSimulationApp.Instance?.Destroy();
        }
    }

    /// <summary>Shown when an action needs an active profile but none is selected.</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class NoProfileForm : InputForm<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public NoProfileForm(IWindow window) : base(window)
        {
        }

        protected override string OnDialogPrompt() =>
            $"{Environment.NewLine}No profile is selected.{Environment.NewLine}Create a new profile or select an existing one first.";

        protected override void OnDialogResponse(DialogResponse reponse) => ClearForm();
    }
}
