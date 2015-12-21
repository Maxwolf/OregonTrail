using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Used when the event director fires event that game simulation subscribes to which passes along events that should
    ///     be triggered when they occur so this state can be attached to the travel Windows and also have the event data
    ///     passed into it so it may be executed and data shown in text user interface for this state.
    /// </summary>
    [ParentWindow(GameWindow.RandomEvent)]
    public sealed class EventExecutor : InputForm<RandomEventInfo>
    {
        /// <summary>
        ///     Determines if the player has seen the random event and is done reading with what it had to say.
        /// </summary>
        private bool _eventPlayerAcknowledge;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EventExecutor(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Create new string builder that will hold event execution data.
            var _randomEventText = new StringBuilder();

            // Execute the event which should return us some text to display to user about what it did to running simulation.
            UserData.DirectorEvent.Execute(UserData);
            var eventText = UserData.DirectorEvent.Render(UserData);

            // Complain if the event text is empty.
            if (string.IsNullOrEmpty(eventText) || string.IsNullOrWhiteSpace(eventText))
                throw new InvalidOperationException(
                    $"Executed random event {UserData.DirectorEvent.Name} from director, but it returned no text data!");

            // Add the text to the user data so we can print it on another form if needed.
            UserData.EventText = eventText;

            // Check what we should do with the random event form now that the user is done with this part of it.
            if (UserData.DaysToSkip <= 0)
            {
                // Attaches a new form that will skip over the required number of days we have detected.
                SetForm(typeof(EventSkipDay));
                return "Loading Event...";
            }

            // Add the text to our output about the random event.
            _randomEventText.AppendLine(
            $"{Environment.NewLine}{eventText}{Environment.NewLine}");
            return _randomEventText.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Person has already acknowledged this event form.
            if (_eventPlayerAcknowledge)
                return;

            // Prevent multiple closures of this form, and window.
            _eventPlayerAcknowledge = true;

            // Only remove the entire random event form if we don't have any days to skip.
            ParentWindow.RemoveWindowNextTick();

            // Fires off event so events can do something special when the event closes.
            UserData.DirectorEvent.OnEventClose();
        }
    }
}