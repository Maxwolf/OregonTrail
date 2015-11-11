using System;
using System.Text;
using TrailEntities.Simulation.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Used when the event director fires event that game simulation subscribes to which passes along events that should
    ///     be triggered when they occur so this state can be attached to the travel mode and also have the event data passed
    ///     into it so it may be executed and data shown in text user interface for this state.
    /// </summary>
    public sealed class RandomEventState : DialogState<RandomEventInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RandomEventState(IModeProduct gameMode, RandomEventInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Create new string builder that will hold event execution data.
            var _randomEventText = new StringBuilder();

            // Execute the event which should return us some text to display to user about what it did to running simulation.
            UserData.DirectorEvent.Execute();
            var eventText = UserData.DirectorEvent.Render();

            // Complain if the event text is empty.
            if (string.IsNullOrEmpty(eventText) || string.IsNullOrWhiteSpace(eventText))
                throw new InvalidOperationException(
                    $"Executed random event {UserData.DirectorEvent.Name} from director, but it returned no text data!");

            // Add the text to our output about the random event.
            _randomEventText.AppendLine($"{UserData.SourceEntity.Name} {UserData.DirectorEvent.Name} {eventText}{Environment.NewLine}");
            return _randomEventText.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            ParentMode.CurrentState = null;
        }
    }
}