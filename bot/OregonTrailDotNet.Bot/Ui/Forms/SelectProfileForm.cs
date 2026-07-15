using System.Text;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Learning;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>Lists existing profiles and lets the player pick one to make active.</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class SelectProfileForm : Form<BotAppData>
    {
        private List<ProfileRecord> _profiles = new();

        // ReSharper disable once UnusedMember.Global
        public SelectProfileForm(IWindow window) : base(window)
        {
        }

        public override bool InputFillsBuffer => true;

        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();
            _profiles = BotContext.Db?.Profiles.All().ToList() ?? new List<ProfileRecord>();
        }

        public override string OnRenderForm()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Environment.NewLine}Select a profile:{Environment.NewLine}");

            if (_profiles.Count == 0)
                sb.AppendLine("  (no profiles yet — create one first)");
            else
                for (var i = 0; i < _profiles.Count; i++)
                {
                    var p = _profiles[i];
                    var model = TrainingModels.ByKey(p.PolicyKind).DisplayName;
                    sb.AppendLine($"  {i + 1}. {p.Name}   [{model}]   games:{p.TotalIterations}  gens:{p.Generations}  best:{p.BestScore}");
                }

            sb.Append($"{Environment.NewLine}Enter a number, or 0 to go back.");
            return sb.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            if (int.TryParse(input, out var n) && n >= 1 && n <= _profiles.Count)
            {
                var p = _profiles[n - 1];
                BotContext.ActiveProfileId = p.Id;
                BotContext.ActiveProfileName = p.Name;
            }

            ClearForm();
        }
    }
}
