using System.Text;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Learning;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Bot.Ui
{
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
}
