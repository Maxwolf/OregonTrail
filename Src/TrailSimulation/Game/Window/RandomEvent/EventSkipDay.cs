using System;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Skips over a set amount of time that an event would like to move past. The days will be ticked normally, and not
    ///     forced like a river crossing does so days won't go by while crossing a single river.
    /// </summary>
    [ParentWindow(GameWindow.RandomEvent)]
    public sealed class EventSkipDay : Form<RandomEventInfo>
    {
        /// <summary>
        ///     Holds the message that will be constructed on behalf of the form and then rendered out to the user after execution.
        /// </summary>
        private StringBuilder _skipMessage;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EventSkipDay(IWindow window) : base(window)
        {
            _skipMessage = new StringBuilder();
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            get { return false; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderForm()
        {
            // Clear any previous skip message.
            _skipMessage.Clear();

            // Print out the event information from user data.


            // Determine if we have skipped a single day, or multiple days.
            _skipMessage.AppendLine(UserData.DaysToSkip > 1
                ? $"{Environment.NewLine}Lose {UserData.DaysToSkip} days."
                : $"{Environment.NewLine}Lose 1 day.");

            // Allow the user to stop resting, this will break the cycle and reset days to rest to zero.
            if (UserData.DaysToSkip <= 0)
                _skipMessage.AppendLine($"{Environment.NewLine}{InputManager.PRESS_ENTER}{Environment.NewLine}");

            // Return the skip message data to scene graph.
            return _skipMessage.ToString();
        }

        /// <summary>
        ///     Fired when the game Windows current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Skip if we have not actually skipped any days yet.
            if (UserData.DaysToSkip <= 0)
                return;

            // After the event skips some days we will want to return to what were doing
            UserData.DaysToSkip = 0;
            ParentWindow.RemoveWindowNextTick();
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Only change the vehicle status to stopped if it is moving, it could just be stuck.
            if (GameSimulationApp.Instance.Vehicle.Status == VehicleStatus.Moving)
                GameSimulationApp.Instance.Vehicle.Status = VehicleStatus.Stopped;
        }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        /// <param name="skipDay">
        ///     Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.
        /// </param>
        public override void OnTick(bool systemTick, bool skipDay)
        {
            base.OnTick(systemTick, skipDay);

            // Skip system ticks.
            if (systemTick)
                return;

            // Not ticking when days to skip are zero.
            if (UserData.DaysToSkip <= 0)
                return;

            // Decrease number of days needed to skip, increment number of days skipped.
            UserData.DaysToSkip--;

            // Simulate the days to rest in time and event system, this will trigger another random event if needed.
            GameSimulationApp.Instance.TakeTurn(false);
        }
    }
}