// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Event.Vehicle
{
    using Module.Director;
    using Prefab;

    /// <summary>
    ///     Vehicle has lost the trail and ended up in the deep woods, now they need to find their way back onto the trail.
    ///     Hopefully there are some tracks you can follow!
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle)]
    public sealed class LostTrail : LoseTime
    {
        /// <summary>
        ///     Grabs the correct number of days that should be skipped by the lose time event. The event skip day form that
        ///     follows will count down the number of days to zero before letting the player continue.
        /// </summary>
        /// <returns>Number of days that should be skipped in the simulation.</returns>
        protected override int DaysToSkip()
        {
            return GameSimulationApp.Instance.Random.Next(1, 3);
        }

        /// <summary>
        ///     Defines the string that will be used to define the event and how it affects the user. It will automatically append
        ///     the number of days lost and count them down this only wants the text that days what the player lost the days
        ///     because of.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnLostTimeReason()
        {
            return "Lost trail.";
        }
    }
}