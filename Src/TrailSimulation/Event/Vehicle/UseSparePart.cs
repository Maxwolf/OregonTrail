// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@3:02 AM

namespace TrailSimulation.Event
{
    using System;
    using Game;

    /// <summary>
    ///     Called by the simulation when the player uses a spare part to repair their vehicle. This event will not actually
    ///     remove the item it should be done before this is attached, the event serves as more of a dialog box to inform the
    ///     user that they repair has taken place and the item removed from vehicle inventory.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle, EventExecution.ManualOnly)]
    public sealed class UseSparePart : VehicleRepair
    {
        /// <summary>
        ///     Called by the vehicle repair prefab so implementations can return the reason why the vehicle was repaired for the
        ///     player so they know why it happened.
        /// </summary>
        /// <param name="partName">Name of the part that was repaired on the vehicle.</param>
        /// <returns>Formatted string that contains the reason why the vehicle was repaired.</returns>
        protected override string OnVehicleRepairReason(string partName)
        {
            return $"{Environment.NewLine}You were able to repair the vehicle" +
                   $"{partName} using your spare.{Environment.NewLine}";
        }
    }
}