using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>Confirms wiping the entire database — every bot, all training, and the leaderboard.</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class EraseAllConfirm : InputForm<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public EraseAllConfirm(IWindow window) : base(window)
        {
        }

        protected override DialogTypeEnum DialogType => DialogTypeEnum.YesNo;

        protected override string OnDialogPrompt() =>
            $"{Environment.NewLine}Erase ALL bot data?{Environment.NewLine}{Environment.NewLine}" +
            $"This permanently destroys EVERY bot, all training{Environment.NewLine}" +
            $"progress, and the entire leaderboard.{Environment.NewLine}" +
            $"It cannot be undone.{Environment.NewLine}{Environment.NewLine}" +
            "Are you sure? (Y/N)";

        protected override void OnDialogResponse(DialogResponseEnum reponse)
        {
            if (reponse == DialogResponseEnum.Yes && BotContext.Db != null)
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
