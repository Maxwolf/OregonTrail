using System.Text;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Learning;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>First step of creating a profile: choose which training model it learns with, then move on to naming it.</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class SelectModelForm : Form<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public SelectModelForm(IWindow window) : base(window)
        {
        }

        public override bool InputFillsBuffer => true;

        public override string OnRenderForm()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Environment.NewLine}Choose a training model for your new bot:{Environment.NewLine}");

            var models = TrainingModels.All;
            for (var i = 0; i < models.Count; i++)
            {
                sb.AppendLine($"  {i + 1}. {models[i].DisplayName}");
                sb.AppendLine($"       {models[i].Description}");
            }

            sb.Append($"{Environment.NewLine}Enter a number (ENTER = 1, Cross-Entropy Method), or 0 to cancel.");
            return sb.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            var models = TrainingModels.All;

            var choice = 1;
            if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out var parsed))
                choice = parsed;

            if (choice == 0)
            {
                ClearForm();
                return;
            }

            if (choice < 1 || choice > models.Count)
                choice = 1;

            UserData.NewProfileModelKey = models[choice - 1].Key;
            SetForm(typeof(CreateProfileForm));
        }
    }
}
