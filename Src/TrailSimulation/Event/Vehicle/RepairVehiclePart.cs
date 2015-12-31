// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@2:14 AM

namespace TrailSimulation.Event
{
    using System;
    using Game;

    /// <summary>
    ///     Manually triggered when random event system damages or makes equipment on the vehicle malfunction. This gives the
    ///     player a chance to magically fix the vehicle part when they attempt to fix it. If the player tries to repair it
    ///     this form is shown if the dice roll went in their favor, otherwise they better hope they have a spare part to fix
    ///     it or they are going to be stranded in a broken vehicle unable to continue their journey.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle, EventExecution.ManualOnly)]
    public sealed class RepairVehiclePart : VehicleRepair
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
                   $"{partName}.{Environment.NewLine}";
        }
    }
}