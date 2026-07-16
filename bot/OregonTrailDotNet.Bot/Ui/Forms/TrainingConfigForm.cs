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
        // Internal so the custom-count form can build an identical request. Games-per-candidate matches the
        // TrainingConfig default (see its doc comment for why 64).
        internal const int DefaultPopulation = 16;
        internal const int DefaultGamesPerCandidate = 64;

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
                Kind = BotRequestKindEnum.Train,
                ProfileId = BotContext.ActiveProfileId,
                Generations = generations,
                PopulationSize = DefaultPopulation,
                GamesPerCandidate = DefaultGamesPerCandidate
            };
            BotSimulationApp.Instance?.Destroy();
        }
    }
}
