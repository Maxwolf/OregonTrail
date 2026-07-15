using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>Destructive data management: delete the active bot, or wipe the whole database. Each choice is confirmed.</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class ManageDataForm : Form<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public ManageDataForm(IWindow window) : base(window)
        {
        }

        public override bool InputFillsBuffer => true;

        public override string OnRenderForm()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Environment.NewLine}Manage training data:{Environment.NewLine}");

            sb.AppendLine(BotContext.ActiveProfileId >= 0
                ? $"  1. Delete the active bot: {BotContext.ActiveProfileName}"
                : "  1. Delete the active bot  (none selected)");
            sb.AppendLine("  2. Erase ALL data (every bot and score)");

            sb.Append($"{Environment.NewLine}Enter a number, or 0 to go back.");
            return sb.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            switch (input?.Trim())
            {
                case "1":
                    if (BotContext.ActiveProfileId >= 0)
                        SetForm(typeof(DeleteProfileConfirm));
                    // no active profile: leave the menu up
                    break;
                case "2":
                    SetForm(typeof(EraseAllConfirm));
                    break;
                default:
                    ClearForm();
                    break;
            }
        }
    }
}
