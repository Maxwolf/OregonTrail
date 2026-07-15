// Created by Maxwolf (bigmaxwolf.com)
// Timestamp 01/03/2016@1:50 AM

using System;
using OregonTrailDotNet.Event.Prefab;
using OregonTrailDotNet.Module.Director;

namespace OregonTrailDotNet.Event.Weather
{
    /// <summary>
    ///     A blizzard sweeps across the high country, pinning the party down for several days until the storm passes. Fired
    ///     manually when severe weather strikes a high-ground location, which is where blizzards overwhelmingly occur.
    /// </summary>
    [DirectorEvent(EventCategory.Weather, EventExecution.ManualOnly)]
    public sealed class Blizzard : LoseTime
    {
        /// <summary>
        ///     Grabs the correct number of days that should be skipped by the lose time event.
        /// </summary>
        /// <returns>Number of days that should be skipped in the simulation.</returns>
        protected override int DaysToSkip()
        {
            return GameSimulationApp.Instance.Random.Next(1, 4);
        }

        /// <summary>
        ///     Defines the string that will be used to define the event and how it affects the user.
        /// </summary>
        /// <returns>The reason days were skipped.</returns>
        protected override string OnLostTimeReason()
        {
            return $"Blizzard conditions at high{Environment.NewLine}elevation halt your progress.";
        }
    }
}
