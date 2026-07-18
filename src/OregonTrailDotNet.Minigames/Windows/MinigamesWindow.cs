using WolfCurses;
using WolfCurses.Window;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     The workbench's only window: a menu whose choices attach the section forms. One window with many forms
    ///     (rather than a window per minigame) is the same shape the game's own Travel window uses, and it means
    ///     leaving a section is just <c>ClearForm()</c> — the menu is still underneath, already built.
    /// </summary>
    public sealed class MinigamesWindow : Window<MinigameCommandsEnum, MinigameInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="MinigamesWindow" /> class.</summary>
        /// <param name="simUnit">The owning simulation.</param>
        // ReSharper disable once UnusedMember.Global — created by the window factory.
        public MinigamesWindow(SimulationApp simUnit) : base(simUnit)
        {
        }

        /// <inheritdoc />
        public override void OnWindowPostCreate()
        {
            base.OnWindowPostCreate();

            MenuHeader =
                "THE OREGON TRAIL — MINIGAME WORKBENCH" + Environment.NewLine + Environment.NewLine +
                "Each section runs on its own, with no game simulation behind it," + Environment.NewLine +
                "so logic and presentation can be felt and tuned in isolation." + Environment.NewLine +
                Environment.NewLine +
                "In a section:  ESC returns here   -/+ changes the tick rate";

            MenuFooter = Environment.NewLine + "Art: legacy/art/apple2   Spec: docs/minigames.md";

            AddCommand(Raft, MinigameCommandsEnum.ColumbiaRaft);
            AddCommand(Hunt, MinigameCommandsEnum.Hunt);
            AddCommand(Tombstone, MinigameCommandsEnum.Tombstone);
            AddCommand(Sprites, MinigameCommandsEnum.Sprites);
            AddCommand(Landmarks, MinigameCommandsEnum.Landmarks);
            AddCommand(Travel, MinigameCommandsEnum.Travel);
            AddCommand(Quit, MinigameCommandsEnum.Quit);
        }

        private void Raft() => SetForm(typeof(RaftForm));
        private void Hunt() => SetForm(typeof(HuntForm));
        private void Tombstone() => SetForm(typeof(TombstoneForm));
        private void Sprites() => SetForm(typeof(SpriteSheetForm));
        private void Landmarks() => SetForm(typeof(LandmarkSlideshowForm));
        private void Travel() => SetForm(typeof(TravelForm));
        private static void Quit() => MinigamesApp.Instance?.Destroy();
    }
}
