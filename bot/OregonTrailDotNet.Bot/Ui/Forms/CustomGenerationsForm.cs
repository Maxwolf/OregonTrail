using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Bot.Ui
{
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
}
