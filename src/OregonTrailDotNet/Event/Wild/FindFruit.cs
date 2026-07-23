// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Diagnostics.CodeAnalysis;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Module.Director;
using OregonTrailDotNet.Window.RandomEvent;

namespace OregonTrailDotNet.Event.Wild
{
    /// <summary>
    ///     Similar to wild berries, but with fruit there will be more to go around. Found on about one day in twenty-five,
    ///     and only in the months anything is actually ripe.
    /// </summary>
    [DirectorEvent(EventCategoryEnum.Wild, EventExecutionEnum.RandomOrManual, 100, 4)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class FindFruit : EventProduct
    {
        /// <summary>
        ///     Only from May through September. Nothing is growing on the trail either side of that, and a party crossing in
        ///     winter finds nothing to pick - which is a good part of why leaving late is so much harder than leaving early.
        /// </summary>
        /// <param name="sourceEntity">Entity the event would be executed against.</param>
        /// <returns>TRUE only during the growing season.</returns>
        public override bool CanExecute(IEntity sourceEntity)
        {
            var month = (int) GameSimulationApp.Instance.Time.CurrentMonth;
            return (month > 4) && (month < 10);
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventExecutor">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(RandomEventInfo eventExecutor)
        {
            // Cast the source entity as vehicle.
            var vehicle = eventExecutor.SourceEntity as Entity.Vehicle.Vehicle;

            // Twenty pounds, per the original's "Find wild fruit." handler (PF = PF + 20). Note the original neglected to
            // check the 2000 pound ceiling here, letting food overflow slightly past maximum; AddQuantity clamps instead,
            // so that overflow is deliberately not reproduced.
            vehicle?.Inventory[EntitiesEnum.Food].AddQuantity(20);
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            return "Find wild fruit.";
        }
    }
}