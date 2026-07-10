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

        protected override void OnDialogResponse(DialogResponse reponse) => ClearForm();
    }

    /// <summary>Shows the active profile's training progress, including text sparklines of the learning curve.</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class StatsForm : InputForm<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public StatsForm(IWindow window) : base(window)
        {
        }

        protected override string OnDialogPrompt()
        {
            var db = BotContext.Db;
            var profile = db?.Profiles.GetById(BotContext.ActiveProfileId);
            if (db == null || profile == null)
                return $"{Environment.NewLine}No profile selected.";

            var sb = new StringBuilder();
            sb.AppendLine($"{Environment.NewLine}=== {profile.Name}  [{TrainingModels.ByKey(profile.PolicyKind).DisplayName}] ===");
            sb.AppendLine($"Total games played : {profile.TotalIterations}");
            sb.AppendLine($"Generations trained: {profile.Generations}");
            sb.AppendLine($"Best score ever    : {profile.BestScore}");

            var generations = db.Runs.GenerationSummary(profile.Id);
            if (generations.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"Best score / generation ({generations.Count} gens):");
                sb.AppendLine("  " + Sparkline.Render(generations.Select(g => (double) g.BestScore)));
                sb.AppendLine("Mean score / generation:");
                sb.AppendLine("  " + Sparkline.Render(generations.Select(g => g.MeanScore)));
            }

            var recent = db.Runs.RecentForProfile(profile.Id, 5);
            if (recent.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Most recent games:");
                foreach (var r in recent)
                    sb.AppendLine($"  {r.Outcome,-8} score:{r.FinalScore,6} miles:{r.Miles,4} days:{r.Days,3}");
            }

            return sb.ToString();
        }

        protected override void OnDialogResponse(DialogResponse reponse) => ClearForm();
    }
}
