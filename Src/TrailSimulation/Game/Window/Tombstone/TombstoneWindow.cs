using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Base tombstone interaction window, this is used to edit and confirm epitaphs for tombstones. Spawns forms that deal
    ///     with showing and editing tombstone object data.
    /// </summary>
    public sealed class TombstoneWindow : Window<TombstoneCommands, TombstoneInfo>
    {
        /// <summary>
        ///     Defines the current game Windows the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameWindow WindowCategory
        {
            get { return GameWindow.Tombstone; }
        }

        /// <summary>
        ///     Called after the Windows has been added to list of modes and made active.
        /// </summary>
        public override void OnWindowPostCreate()
        {
            base.OnWindowPostCreate();

            // Attach the tombstone viewer.
            SetForm(typeof (EpitaphQuestion));
        }
    }
}