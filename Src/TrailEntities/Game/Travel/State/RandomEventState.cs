using System;
using System.Text;
using TrailEntities.Entity;
using TrailEntities.Event;
using TrailEntities.Game.RandomEvent;
using TrailEntities.Mode;
using TrailEntities.Simulation;

namespace TrailEntities.Game.Travel
{
    /// <summary>
    ///     Used when the event director fires event that game simulation subscribes to which passes along events that should
    ///     be triggered when they occur so this state can be attached to the travel mode and also have the event data passed
    ///     into it so it may be executed and data shown in text user interface for this state.
    /// </summary>
    public sealed class RandomEventState : ModeStateProduct
    {
        /// <summary>
        ///     Holds all of the text from the random event after it executes so we can display it to the user without having to
        ///     run it again.
        /// </summary>
        private StringBuilder _randomEventText;

        /// <summary>
        ///     Determines if the player is done reading about what the random event has done to them.
        /// </summary>
        private bool _readRandomEventText;

        /// <summary>
        ///     Random event class is typically attached via an event delegate being triggered somewhere else in the simulation.
        /// </summary>
        /// <param name="gameMode">Parent game mode of this state.</param>
        /// <param name="userData">Custom user data for this game mode that is shared across all states.</param>
        /// <param name="simEntity">Simulation entity that triggered the event to occur in the first place.</param>
        /// <param name="directorEventItem">
        ///     The actual event that director wants executed and information displayed to user about what it
        ///     does.
        /// </param>
        public RandomEventState(ModeProduct gameMode, RandomEventInfo userData, IEntity simEntity, DirectorEventItem directorEventItem)
            : base(gameMode, userData)
        {
            // Create new string builder that will hold event execution data.
            _randomEventText = new StringBuilder();

            // Execute the event which should return us some text to display to user about what it did to running simulation.
            var eventText = directorEventItem.Execute(_randomEventText);

            // Complain if the event text is empty.
            if (string.IsNullOrEmpty(eventText) || string.IsNullOrWhiteSpace(eventText))
                throw new InvalidOperationException(
                    $"Executed random event {directorEventItem.Name} from director, but it returned no text data!");

            // Add the text to our output about the random event.
            _randomEventText.AppendLine($"{simEntity.Name} {eventText}{Environment.NewLine}");

            // Wait for user input...
            _randomEventText.Append(GameSimulationApp.PRESS_ENTER);
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            return _randomEventText.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_readRandomEventText)
                return;

            _readRandomEventText = true;
            ParentMode.RemoveState();
        }
    }
}