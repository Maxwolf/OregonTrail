using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Bot.Ui
{
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
