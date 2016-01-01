// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Event.Prefab
{
    using Module.Director;
    using Window.RandomEvent;

    /// <summary>
    ///     Forces the player to advance time in the date, this will make it so they will have to face harsher weather
    ///     conditions and also other random events can fire from this one.
    /// </summary>
    public abstract class LoseTime : EventProduct
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
            // Add to the days to skip since multiple events in a chain could keep adding to the total.
            userData.DaysToSkip += DaysToSkip();
        }

        /// <summary>
        ///     Fired after the event is executed and allows the inheriting event prefab know post event execution.
        /// </summary>
        /// <param name="eventExecutor">Form that executed the event from the random event window.</param>
        internal override bool OnPostExecute(EventExecutor eventExecutor)
        {
            base.OnPostExecute(eventExecutor);

            // Check what we should do with the random event form now that the user is done with this part of it.
            if (eventExecutor.UserData.DaysToSkip > 0)
                return false;

            // Attaches a new form that will skip over the required number of days we have detected.
            eventExecutor.SetForm(typeof (EventSkipDay));
            return true;
        }

        /// <summary>
        ///     Grabs the correct number of days that should be skipped by the lose time event. The event skip day form that
        ///     follows will count down the number of days to zero before letting the player continue.
        /// </summary>
        /// <returns>Number of days that should be skipped in the simulation.</returns>
        protected abstract int DaysToSkip();

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
            return OnLostTimeReason();
        }

        /// <summary>
        ///     Defines the string that will be used to define the event and how it affects the user. It will automatically append
        ///     the number of days lost and count them down this only wants the text that days what the player lost the days
        ///     because of.
        /// </summary>
        /// <returns>
        ///     The reason days were skipped.<see cref="string" />.
        /// </returns>
        protected abstract string OnLostTimeReason();
    }
}