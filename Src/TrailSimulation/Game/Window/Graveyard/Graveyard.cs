// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tombstone.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Base tombstone interaction window, this is used to edit and confirm epitaphs for tombstones. Spawns forms that deal
//   with showing and editing tombstone object data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Game
{
    using Core;

    /// <summary>
    ///     Base tombstone interaction window, this is used to edit and confirm epitaphs for tombstones. Spawns forms that deal
    ///     with showing and editing tombstone object data.
    /// </summary>
    public sealed class Graveyard : Window<TombstoneCommands, TombstoneInfo>
    {
        /// <summary>
        ///     Defines the current game Windows the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameWindow WindowCategory
        {
            get { return GameWindow.Graveyard; }
        }

        /// <summary>
        ///     Called after the Windows has been added to list of modes and made active.
        /// </summary>
        public override void OnWindowPostCreate()
        {
            base.OnWindowPostCreate();

            // Depending on the living status of passengers in current player vehicle we will attach a different form.
            SetForm(GameSimulationApp.Instance.Vehicle.PassengerLivingCount <= 0
                ? typeof (EpitaphQuestion)
                : typeof (TombstoneView));
        }
    }
}