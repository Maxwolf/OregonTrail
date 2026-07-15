using System.Text;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Learning;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>Shows the bot's persistent top-ten high scores.</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class LeaderboardForm : InputForm<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public LeaderboardForm(IWindow window) : base(window)
        {
        }

        protected override string OnDialogPrompt()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Environment.NewLine}=== BOT LEADERBOARD (top 10) ===");
            sb.AppendLine();

            var top = BotContext.Db?.Leaderboard.Top(10) ?? new List<LeaderboardEntry>();
            if (top.Count == 0)
            {
                sb.AppendLine("  No qualifying scores yet — train a profile and win some games!");
                sb.AppendLine("  (The original game's 10th place to beat is Elijah White at 250.)");
            }
            else
            {
                var rank = 1;
                foreach (var e in top)
                {
                    var model = e.ProfileId.HasValue
                        ? TrainingModels.ByKey(BotContext.Db?.Profiles.GetById(e.ProfileId.Value)?.PolicyKind).DisplayName
                        : "";
                    sb.AppendLine($"  {rank++,2}. {e.Score,6}  {e.Name}  [{model}]  [{e.Rating}]");
                }
            }

            return sb.ToString();
        }

        protected override void OnDialogResponse(DialogResponseEnum reponse) => ClearForm();
    }
}
