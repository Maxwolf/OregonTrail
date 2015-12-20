using System.Diagnostics.CodeAnalysis;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Does not destroy items or drown people but will make you lose time gathering your things and drying them out.
    /// </summary>
    [DirectorEvent(EventCategory.RiverCross)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class SuppliesWet : EventLoseTime
    {
        /// <summary>
        ///     Grabs the correct number of days that should be skipped by the lose time event. The event skip day form that
        ///     follows will count down the number of days to zero before letting the player continue.
        /// </summary>
        /// <returns>Number of days that should be skipped in the simulation.</returns>
        protected override int DaysToSkip()
        {
            return 1;
        }

        /// <summary>
        ///     Defines the string that will be used to define the event and how it affects the user. It will automatically append
        ///     the number of days lost and count them down this only wants the text that days what the player lost the days
        ///     because of.
        /// </summary>
        /// <returns></returns>
        protected override string OnLostTimeReason()
        {
            return "Your supplies got wet.";
        }
    }
}