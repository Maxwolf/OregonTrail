using WolfCurses;

namespace OregonTrailDotNet.Minigames
{
    /// <summary>
    ///     The workbench simulation. Deliberately tiny: it owns no modules, no trail, no party — a minigame here is
    ///     driven by its own form and nothing else, which is the entire point of testing them outside the game.
    ///     <para>
    ///         <see cref="SimulationApp.AllowedWindows" /> is not overridden, so WolfCurses discovers every concrete
    ///         window in this assembly (and its own built-in dialogs) by reflection. Adding a new section is therefore
    ///         a new form plus one menu entry — nothing to register.
    ///     </para>
    /// </summary>
    public sealed class MinigamesApp : SimulationApp
    {
        /// <summary>The running workbench, or null once it has been destroyed.</summary>
        public static MinigamesApp? Instance { get; private set; }

        /// <summary>Creates the singleton. Mirrors the game's own create-then-tick lifecycle.</summary>
        public static void Create()
        {
            Instance?.Destroy();
            Instance = new MinigamesApp();
        }

        /// <inheritdoc />
        protected override void OnFirstTick()
        {
            Restart();
            WindowManager.Add(typeof(Windows.MinigamesWindow));
        }

        /// <summary>
        ///     The scene graph always draws a status header; keeping our contribution to it empty leaves the artwork as
        ///     unencumbered as the framework allows.
        /// </summary>
        public override string OnPreRender() => string.Empty;

        /// <inheritdoc />
        protected override void OnPreDestroy() => Instance = null;
    }
}
