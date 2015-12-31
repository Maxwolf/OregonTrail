// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@2:37 AM

namespace TrailSimulation.Event
{
    using System.Text;
    using Game;

    /// <summary>
    ///     Manually triggered event for when the player decides to not and repair their vehicle and instead opts to use spare
    ///     parts and or willingly want to be stranded unable to continue their journey.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle, EventExecution.ManualOnly)]
    public sealed class NoRepairVehicle : EventProduct
    {
        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="userData">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(RandomEventInfo userData)
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            var repairPrompt = new StringBuilder();
            repairPrompt.AppendLine("You did not repair the broken vehicle");
            repairPrompt.AppendLine($"{userData.BrokenPart.Name.ToLowerInvariant()}. You must replace it with a");
            repairPrompt.Append("space part.");
            return repairPrompt.ToString();
        }
    }
}