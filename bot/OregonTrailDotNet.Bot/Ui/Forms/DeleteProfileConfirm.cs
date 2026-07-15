using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>Confirms permanently deleting the active bot and all of its training data.</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class DeleteProfileConfirm : InputForm<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public DeleteProfileConfirm(IWindow window) : base(window)
        {
        }

        protected override DialogTypeEnum DialogType => DialogTypeEnum.YesNo;

        protected override string OnDialogPrompt() =>
            $"{Environment.NewLine}Delete the bot '{BotContext.ActiveProfileName}'?{Environment.NewLine}{Environment.NewLine}" +
            $"This permanently destroys ALL of its training data{Environment.NewLine}" +
            $"and high scores, and cannot be undone.{Environment.NewLine}{Environment.NewLine}" +
            "Are you sure? (Y/N)";

        protected override void OnDialogResponse(DialogResponseEnum reponse)
        {
            if (reponse == DialogResponseEnum.Yes && BotContext.Db != null && BotContext.ActiveProfileId >= 0)
            {
                BotContext.Db.Leaderboard.DeleteForProfile(BotContext.ActiveProfileId);
                BotContext.Db.Profiles.Delete(BotContext.ActiveProfileId);
                BotContext.ActiveProfileId = -1;
                BotContext.ActiveProfileName = string.Empty;
            }

            ClearForm();
        }
    }
}
