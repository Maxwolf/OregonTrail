using System;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Forces the player to advance time in the date, this will make it so they will have to face harsher weather
    ///     conditions and also other random events can fire from this one.
    /// </summary>
    public abstract class EventLoseTime : EventProduct
    {
        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventInfo">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(RandomEventInfo eventInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo eventInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the event is closed by the user or system after being executed and rendered out on text user interface.
        /// </summary>
        public override void OnEventClose()
        {
            base.OnEventClose();

            // Grab the game simulation instance into smaller variable.
            var game = GameSimulationApp.Instance;

            // Check if the window manager has the random event window attached (it should be).
            if (!game.WindowManager.ContainsWindow(GameWindow.RandomEvent))
                return;

            // Cast the grabbed window into the random event window.
            var randomWindow = game.WindowManager.Windows[GameWindow.RandomEvent] as RandomEvent;

            // Complain if the window is null.
            if (randomWindow == null)
                throw new InvalidCastException(
                    "Unable to cast window manager random event window to random event window type!");

            // Set the form of the random event window to the event for skipping days.
            randomWindow.SetForm(typeof (EventSkipDay));
        }
    }
}