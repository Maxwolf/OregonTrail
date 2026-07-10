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

    /// <summary>Second step of creating a profile: name it, which creates it with the model chosen previously and activates it.</summary>
    [ParentWindow(typeof(BotMainMenu))]
    public sealed class CreateProfileForm : Form<BotAppData>
    {
        // ReSharper disable once UnusedMember.Global
        public CreateProfileForm(IWindow window) : base(window)
        {
        }

        public override bool InputFillsBuffer => true;

        public override string OnRenderForm()
        {
            var modelName = TrainingModels.ByKey(UserData.NewProfileModelKey).DisplayName;
            return $"{Environment.NewLine}New {modelName} profile.{Environment.NewLine}" +
                   $"Type a name (this is its save file) and press ENTER, or{Environment.NewLine}" +
                   $"leave it blank to use the default name \"{modelName}\":";
        }

        public override void OnInputBufferReturned(string input)
        {
            if (BotContext.Db == null)
            {
                ClearForm();
                return;
            }

            var key = string.IsNullOrEmpty(UserData.NewProfileModelKey)
                ? TrainingModels.Default.Key
                : UserData.NewProfileModelKey;

            // No name given → default to the strategy model's name (made unique so repeated blank creates don't collide).
            var name = input?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
                name = UniqueDefaultName(TrainingModels.ByKey(key).DisplayName);

            var existing = BotContext.Db.Profiles.GetByName(name);
            var id = existing?.Id ?? BotContext.Db.Profiles.Create(name, key);

            BotContext.ActiveProfileId = id;
            BotContext.ActiveProfileName = name;
            UserData.NewProfileModelKey = string.Empty;
            ClearForm();
        }

        /// <summary>Returns the model's display name, appending " 2", " 3", … if a profile by that name already exists.</summary>
        private static string UniqueDefaultName(string modelDisplayName)
        {
            var profiles = BotContext.Db!.Profiles;
            if (!profiles.NameExists(modelDisplayName))
                return modelDisplayName;

            for (var n = 2; ; n++)
            {
                var candidate = $"{modelDisplayName} {n}";
                if (!profiles.NameExists(candidate))
                    return candidate;
            }
        }
    }

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
