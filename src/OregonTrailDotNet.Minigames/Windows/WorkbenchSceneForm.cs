using OregonTrailDotNet.Presentation;
using WolfCurses.Window;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     The workbench's binding of the shared <see cref="SceneForm{TData}" />: adds back the two behaviors that
    ///     are workbench affordances rather than game features — the -/+ speed keys, and the per-section tick-rate
    ///     store in <see cref="MinigameInfo" /> so each section remembers its own tuning between visits.
    /// </summary>
    public abstract class WorkbenchSceneForm : SceneForm<MinigameInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="WorkbenchSceneForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        protected WorkbenchSceneForm(IWindow window) : base(window)
        {
        }

        /// <summary>Ticks per second, kept per section in the window's user data so each keeps its own tuning.</summary>
        protected override int TicksPerSecond => UserData.TicksPerSecond(GetType().Name, DefaultTicksPerSecond);

        /// <inheritdoc />
        protected override string FooterControlHints => $"ESC menu   -/+ speed ({TicksPerSecond}/sec)";

        /// <inheritdoc />
        public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.OemMinus:
                case ConsoleKey.Subtract:
                    AdjustRate(-1);
                    return;
                case ConsoleKey.OemPlus:
                case ConsoleKey.Add:
                    AdjustRate(1);
                    return;
                default:
                    base.OnKeyPressed(keyInfo);
                    return;
            }
        }

        private void AdjustRate(int delta)
        {
            UserData.SetTicksPerSecond(GetType().Name, Math.Clamp(TicksPerSecond + delta, 1, 60));
            Invalidate();
        }
    }
}
