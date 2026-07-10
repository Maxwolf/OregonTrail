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

    /// <summary>Confirms permanently deleting the active bot and all of its training data.</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class DeleteProfileConfirm : InputForm<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public DeleteProfileConfirm(IWindow window) : base(window)
        {
        }

        protected override DialogType DialogType => DialogType.YesNo;

        protected override string OnDialogPrompt() =>
            $"{Environment.NewLine}Delete the bot '{BotContext.ActiveProfileName}'?{Environment.NewLine}{Environment.NewLine}" +
            $"This permanently destroys ALL of its training data{Environment.NewLine}" +
            $"and high scores, and cannot be undone.{Environment.NewLine}{Environment.NewLine}" +
            "Are you sure? (Y/N)";

        protected override void OnDialogResponse(DialogResponse reponse)
        {
            if (reponse == DialogResponse.Yes && BotContext.Db != null && BotContext.ActiveProfileId >= 0)
            {
                BotContext.Db.Leaderboard.DeleteForProfile(BotContext.ActiveProfileId);
                BotContext.Db.Profiles.Delete(BotContext.ActiveProfileId);
                BotContext.ActiveProfileId = -1;
                BotContext.ActiveProfileName = string.Empty;
            }

            ClearForm();
        }
    }

    /// <summary>Confirms wiping the entire database — every bot, all training, and the leaderboard.</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class EraseAllConfirm : InputForm<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public EraseAllConfirm(IWindow window) : base(window)
        {
        }

        protected override DialogType DialogType => DialogType.YesNo;

        protected override string OnDialogPrompt() =>
            $"{Environment.NewLine}Erase ALL bot data?{Environment.NewLine}{Environment.NewLine}" +
            $"This permanently destroys EVERY bot, all training{Environment.NewLine}" +
            $"progress, and the entire leaderboard.{Environment.NewLine}" +
            $"It cannot be undone.{Environment.NewLine}{Environment.NewLine}" +
            "Are you sure? (Y/N)";

        protected override void OnDialogResponse(DialogResponse reponse)
        {
            if (reponse == DialogResponse.Yes && BotContext.Db != null)
            {
                BotContext.Db.Leaderboard.Clear();
                BotContext.Db.Profiles.DeleteAll();
                BotContext.ActiveProfileId = -1;
                BotContext.ActiveProfileName = string.Empty;
            }

            ClearForm();
        }
    }
}
