// Created by Maxwolf (bigmaxwolf.com)
// Timestamp 01/03/2016@1:50 AM

using System;
using OregonTrailDotNet.Event.Prefab;
using OregonTrailDotNet.Module.Director;

namespace OregonTrailDotNet.Event.Vehicle
{
    /// <summary>
    ///     A cave-in along a high mountain trail blocks the way forward, costing the party a few days to dig out and find a way
    ///     around. Fired manually while traveling through high-ground locations.
    /// </summary>
    [DirectorEvent(EventCategoryEnum.Vehicle, EventExecutionEnum.ManualOnly)]
    public sealed class CaveIn : LoseTime
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
            return $"A cave-in blocks the{Environment.NewLine}trail through the mountains.";
        }
    }
}
