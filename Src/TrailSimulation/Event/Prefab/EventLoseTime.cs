// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLoseTime.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Forces the player to advance time in the date, this will make it so they will have to face harsher weather
//   conditions and also other random events can fire from this one.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Event
{
    using Game;

    /// <summary>
    ///     Forces the player to advance time in the date, this will make it so they will have to face harsher weather
    ///     conditions and also other random events can fire from this one.
    /// </summary>
    public abstract class EventLoseTime : EventProduct
    {
        /// <summary>Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.</summary>
        /// <param name="userData">Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.</param>
        public override void Execute(RandomEventInfo userData)
        {
            // Add to the days to skip since multiple events in a chain could keep adding to the total.
            userData.DaysToSkip += DaysToSkip();
        }

        /// <summary>
        ///     Grabs the correct number of days that should be skipped by the lose time event. The event skip day form that
        ///     follows will count down the number of days to zero before letting the player continue.
        /// </summary>
        /// <returns>Number of days that should be skipped in the simulation.</returns>
        protected abstract int DaysToSkip();

        /// <summary>Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.</summary>
        /// <param name="userData">Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.</param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            return OnLostTimeReason();
        }

        /// <summary>
        ///     Defines the string that will be used to define the event and how it affects the user. It will automatically append
        ///     the number of days lost and count them down this only wants the text that days what the player lost the days
        ///     because of.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected abstract string OnLostTimeReason();
    }
}