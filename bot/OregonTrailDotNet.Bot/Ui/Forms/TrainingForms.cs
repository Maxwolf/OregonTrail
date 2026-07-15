using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>Lets the player pick how long to train from a short menu, then hands control to <see cref="Program" /> to run
    ///     the batch. The last choice trains open-endedly until Esc is pressed (recorded as a negative generation count).</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class TrainingConfigForm : Form<BotAppData>
    {
        // Internal so the custom-count form can build an identical request.
        internal const int DefaultPopulation = 16;
        internal const int DefaultGamesPerCandidate = 8;

        // The training-length choices. A negative generation count means "train until the player presses Esc" (see
        // TrainingSession.Run and Program.RunTraining, which both treat < 0 as open-ended).
        private static readonly (string Label, int Generations)[] Choices =
        {
            ("10 generations", 10),
            ("50 generations", 50),
            ("100 generations", 100),
            ("Train until I press Esc", -1)
        };

        // ReSharper disable once UnusedMember.Global
        public TrainingConfigForm(IWindow window) : base(window)
        {
        }

        public override bool InputFillsBuffer => true;

        public override string OnRenderForm()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Environment.NewLine}Train profile '{BotContext.ActiveProfileName}'.");
            sb.AppendLine();
            sb.AppendLine($"Each generation evaluates {DefaultPopulation} strategies over {DefaultGamesPerCandidate} games " +
                          $"each ({DefaultPopulation * DefaultGamesPerCandidate} games/generation).");
            sb.AppendLine($"{Environment.NewLine}How long should it train?{Environment.NewLine}");

            for (var i = 0; i < Choices.Length; i++)
                sb.AppendLine($"  {i + 1}. {Choices[i].Label}");
            sb.AppendLine($"  {Choices.Length + 1}. Custom number of generations...");

            sb.Append($"{Environment.NewLine}Enter a number (ENTER = {Choices[0].Label}), or 0 to cancel.");
            return sb.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            var choice = 1;
            if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out var parsed))
                choice = parsed;

            if (choice == 0)
            {
                ClearForm();
                return;
            }

            // The extra option after the presets asks for an arbitrary count on its own form.
            if (choice == Choices.Length + 1)
            {
                SetForm(typeof(CustomGenerationsForm));
                return;
            }

            if (choice < 1 || choice > Choices.Length)
                choice = 1;

            StartTraining(Choices[choice - 1].Generations);
        }

        /// <summary>Records a training request for the given generation count (negative = until Esc) and tears down the control
        ///     panel; Program runs the batch and then rebuilds the panel.</summary>
        internal static void StartTraining(int generations)
        {
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

    /// <summary>Prompts for an arbitrary number of generations to train (the "Custom" choice on <see cref="TrainingConfigForm" />).</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class CustomGenerationsForm : Form<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public CustomGenerationsForm(IWindow window) : base(window)
        {
        }

        public override bool InputFillsBuffer => true;

        public override string OnRenderForm() =>
            $"{Environment.NewLine}Train profile '{BotContext.ActiveProfileName}'.{Environment.NewLine}{Environment.NewLine}" +
            $"Each generation is {TrainingConfigForm.DefaultPopulation * TrainingConfigForm.DefaultGamesPerCandidate} games." +
            $"{Environment.NewLine}{Environment.NewLine}How many generations? Type a number and press ENTER, or 0 to go back.";

        public override void OnInputBufferReturned(string input)
        {
            // Blank / non-numeric / <= 0 → go back to the length menu rather than starting a run.
            if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out var generations) || generations <= 0)
            {
                SetForm(typeof(TrainingConfigForm));
                return;
            }

            TrainingConfigForm.StartTraining(generations);
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
