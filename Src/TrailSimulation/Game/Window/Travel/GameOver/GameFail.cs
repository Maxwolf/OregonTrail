using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Fired when the simulation has determined the player has died. It specifically only attaches at this time. The flow
    ///     for death like this is to first show the player the failure state like this, then ask if they want to leave an
    ///     epitaph, process that decision, confirm it, and finally show the viewer that will also show the reason why the
    ///     player died using description attribute from an enumeration value that determines how they died.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class GameFail : InputForm<TravelInfo>
    {
        private StringBuilder _tombstone;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public GameFail(IWindow window) : base(window)
        {
            _tombstone = new StringBuilder();
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
        ///     Determines if this dialog state is allowed to receive any input at all, even empty line returns. This is useful for
        ///     preventing the player from leaving a particular dialog until you are ready or finished processing some data.
        /// </summary>
        public override bool AllowInput
        {
            get { return true; }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Clear any previous message.
            _tombstone.Clear();

            // Grab the leader name, complain if one does not exist.
            var leaderPerson = GameSimulationApp.Instance.Vehicle.PassengerLeader;
            if (leaderPerson == null)
                throw new InvalidOperationException("Unable to grab the leader from the vehicle!");

            // We prompt and say the leader name is dead here since that is how we identity with player.
            _tombstone.AppendLine($"{Environment.NewLine}Here lies {leaderPerson.Name}{Environment.NewLine}");

            // Adds the underlying reason for the games failure if it was not obvious to the player by now.
            _tombstone.AppendLine("All the members of");
            _tombstone.AppendLine("your party have");
            _tombstone.AppendLine($"died.{Environment.NewLine}");

            // Prints out the message for death of all vehicle passengers.
            return _tombstone.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            GameSimulationApp.Instance.WindowManager.Add(GameWindow.Tombstone);
        }
    }
}