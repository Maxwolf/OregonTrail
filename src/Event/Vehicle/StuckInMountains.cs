// Created by Maxwolf (bigmaxwolf.com)
// Timestamp 01/03/2016@1:50 AM

using System;
using OregonTrailDotNet.Event.Prefab;
using OregonTrailDotNet.Module.Director;

namespace OregonTrailDotNet.Event.Vehicle
{
    /// <summary>
    ///     The party becomes stuck while trying to make its way through a high mountain pass, losing anywhere from one to ten
    ///     days before they manage to get moving again. Fired manually when departing a high-ground location that rolls its
    ///     stuck chance (South Pass 80%, Blue Mountains 70%).
    /// </summary>
    [DirectorEvent(EventCategoryEnum.Vehicle, EventExecutionEnum.ManualOnly)]
    public sealed class StuckInMountains : LoseTime
    {
        /// <summary>
        ///     Grabs the correct number of days that should be skipped by the lose time event. Capped between one and ten days so
        ///     the party is never stuck for more than ten days.
        /// </summary>
        /// <returns>Number of days that should be skipped in the simulation.</returns>
        protected override int DaysToSkip()
        {
            return GameSimulationApp.Instance.Random.Next(1, 11);
        }

        /// <summary>
        ///     Defines the string that will be used to define the event and how it affects the user.
        /// </summary>
        /// <returns>The reason days were skipped.</returns>
        protected override string OnLostTimeReason()
        {
            return $"You become stuck in the{Environment.NewLine}mountains for a time.";
        }
    }
}
