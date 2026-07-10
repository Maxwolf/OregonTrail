using System.Reflection;
using WolfCurses;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>
    ///     The bot's own WolfCurses application, hosting the control-panel window stack. It is a completely separate
    ///     <see cref="SimulationApp" /> from the game's <c>GameSimulationApp</c>; the two never run at the same time (see the
    ///     <see cref="Program" /> mode state machine). Because WolfCurses discovers a form's parent window by scanning the
    ///     hosting app's own assembly (plus the entry assembly and <see cref="AdditionalFormAssemblies" />), this app finds its
    ///     forms in the bot assembly even though the process entry assembly is pinned to the game.
    /// </summary>
    public sealed class BotSimulationApp : SimulationApp
    {
        /// <summary>
        ///     Process-wide singleton, mirroring the game's <c>GameSimulationApp.Instance</c> pattern so <see cref="Program" />
        ///     can drive it the same way.
        /// </summary>
        public static BotSimulationApp? Instance { get; private set; }

        /// <summary>
        ///     Windows the control panel is allowed to create. Fleshed out as the UI is built.
        /// </summary>
        public override IEnumerable<Type> AllowedWindows => new List<Type>
        {
            typeof(BotMainMenu)
        };

        /// <summary>
        ///     Belt-and-suspenders: explicitly include the bot assembly for form discovery so it works regardless of what the
        ///     process entry assembly is set to.
        /// </summary>
        public override IEnumerable<Assembly> AdditionalFormAssemblies => new[] { typeof(BotSimulationApp).Assembly };

        /// <summary>
        ///     Creates the singleton instance. Complains if one already exists (matches the game's contract).
        /// </summary>
        public static void Create()
        {
            if (Instance != null)
                throw new InvalidOperationException("BotSimulationApp already exists; destroy the previous one first.");

            Instance = new BotSimulationApp();
        }

        /// <inheritdoc />
        protected override void OnFirstTick()
        {
            Restart();
        }

        /// <inheritdoc />
        public override void Restart()
        {
            base.Restart();
            WindowManager.Add(typeof(BotMainMenu));
        }

        /// <inheritdoc />
        protected override void OnPreDestroy()
        {
            Instance = null;
        }

        /// <inheritdoc />
        public override string OnPreRender()
        {
            var profile = BotContext.ActiveProfileId >= 0
                ? $"Active profile: {BotContext.ActiveProfileName}"
                : "Active profile: (none — create or select one)";

            return $"THE OREGON TRAIL BOT{Environment.NewLine}{profile}{Environment.NewLine}";
        }
    }
}
