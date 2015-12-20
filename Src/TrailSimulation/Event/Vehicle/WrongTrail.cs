using System.Diagnostics.CodeAnalysis;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Player gets lost and heads in the wrong direction which forces time to be lost without any progression of the date
    ///     or trail location.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class WrongTrail : EventLoseTime
    {
        /// <summary>
        ///     Grabs the correct number of days that should be skipped by the lose time event. The event skip day form that
        ///     follows will count down the number of days to zero before letting the player continue.
        /// </summary>
        /// <returns>Number of days that should be skipped in the simulation.</returns>
        protected override int DaysToSkip()
        {
            return GameSimulationApp.Instance.Random.Next(3, 8);
        }

        /// <summary>
        ///     Defines the string that will be used to define the event and how it affects the user. It will automatically append
        ///     the number of days lost and count them down this only wants the text that days what the player lost the days
        ///     because of.
        /// </summary>
        /// <returns></returns>
        protected override string OnLostTimeReason()
        {
            return "Wrong trail.";
        }
    }
}