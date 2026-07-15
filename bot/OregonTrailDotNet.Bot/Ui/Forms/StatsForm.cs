using System.Text;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Learning;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Bot.Ui
{
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
                // Plot the SHAPED FITNESS the optimizer actually maximizes (not the raw game score) plus the win-rate, so the
                // learning curve reflects what training is climbing. The raw-score curve is kept but clearly labelled as such.
                sb.AppendLine($"Mean fitness / generation ({generations.Count} gens)  <- the objective training maximizes:");
                sb.AppendLine("  " + Sparkline.Render(generations.Select(g => g.MeanFitness)));
                sb.AppendLine("Best fitness / generation:");
                sb.AppendLine("  " + Sparkline.Render(generations.Select(g => g.BestFitness)));
                sb.AppendLine($"Win rate / generation (latest {generations[^1].WinRate:P0}):");
                sb.AppendLine("  " + Sparkline.Render(generations.Select(g => g.WinRate)));
                sb.AppendLine("Best RAW game score / generation:");
                sb.AppendLine("  " + Sparkline.Render(generations.Select(g => (double) g.BestScore)));
            }

            var recent = db.Runs.RecentForProfile(profile.Id, 5);
            if (recent.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Most recent games:");
                foreach (var r in recent)
                    sb.AppendLine($"  {r.Outcome,-8} fit:{r.Fitness,7:F0} score:{r.FinalScore,6} miles:{r.Miles,4} days:{r.Days,3}");
            }

            return sb.ToString();
        }

        protected override void OnDialogResponse(DialogResponseEnum reponse) => ClearForm();
    }
}
