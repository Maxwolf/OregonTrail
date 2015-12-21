// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokenVehiclePart.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Triggers one of the vehicle parts to break and will initially ask the player if they would like to try and fix it
//   themselves, if this event fails it will ask the player if they would like to use one of their spare parts. If the
//   player does not have any spare parts then they will become stuck on the trail and have to trade to get their parts
//   and hopefully not die before that happens.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Triggers one of the vehicle parts to break and will initially ask the player if they would like to try and fix it
    ///     themselves, if this event fails it will ask the player if they would like to use one of their spare parts. If the
    ///     player does not have any spare parts then they will become stuck on the trail and have to trade to get their parts
    ///     and hopefully not die before that happens.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class BrokenVehiclePart : EventProduct
    {
        /// <summary>Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.</summary>
        /// <param name="userData">Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.</param>
        public override void Execute(RandomEventInfo userData)
        {
            // TODO: Need to make this trigger travel game window methods to force the player to try and fix the vehicle.
            throw new NotImplementedException();
        }

        /// <summary>Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.</summary>
        /// <param name="userData"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            throw new NotImplementedException();
        }
    }
}