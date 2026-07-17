// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Diagnostics.CodeAnalysis;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Module.Director;
using OregonTrailDotNet.Window.RandomEvent;

namespace OregonTrailDotNet.Event.Wild
{
    /// <summary>
    ///     Indians help you find some free food. This is a rescue rather than a windfall: it only ever fires when the party
    ///     has run completely out of food, which is how the original gated it.
    /// </summary>
    [DirectorEvent(EventCategoryEnum.Wild)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class IndiansHelp : EventProduct
    {
        /// <summary>
        ///     Only eligible when the party is completely out of food. The original recomputed this event's odds every day as
        ///     .05 * (food = 0), so it was as common as anything else at zero food and impossible otherwise.
        /// </summary>
        /// <param name="sourceEntity">Entity the event would be executed against.</param>
        /// <returns>TRUE only when the vehicle holds no food at all.</returns>
        public override bool CanExecute(IEntity sourceEntity)
        {
            var vehicle = sourceEntity as Entity.Vehicle.Vehicle;
            return vehicle?.Inventory[EntitiesEnum.Food].Quantity <= 0;
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

            // Indians hook you up with free food, what nice guys.
            vehicle?.Inventory[EntitiesEnum.Food].AddQuantity(30);
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            return "helpful Indians show you where to find more food";
        }
    }
}