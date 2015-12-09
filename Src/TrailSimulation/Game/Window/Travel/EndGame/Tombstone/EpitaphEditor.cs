using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Allows for the message on the TombstoneItem to be edited or added, either way this form will get the job done. Will
    ///     limit the input of the epitaph also and do basic whitespace checks and trimming.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class EpitaphEditor : Form<TravelInfo>
    {
        /// <summary>
        ///     Defines how long a epitaph on a tombstone can be in characters which will make up the entire string (spaces
        ///     included).
        /// </summary>
        private const int EPITAPH_MAXLENGTH = 38;

        /// <summary>
        ///     String builder that will hold representation of the tombstone for the player to see.
        /// </summary>
        private StringBuilder _epitaphPrompt;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EpitaphEditor(IWindow gameMode) : base(gameMode)
        {
            _epitaphPrompt = new StringBuilder();
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            get { return GameSimulationApp.Instance.InputManager.InputBuffer.Length <= EPITAPH_MAXLENGTH; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderForm()
        {
            // Add TombstoneItem message we want to show the player from TombstoneItem manager.
            _epitaphPrompt.Clear();
            _epitaphPrompt.AppendLine($"{Environment.NewLine}{UserData.TombstoneItem}");
            return _epitaphPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the game Windows current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Trims the string and then cuts off any excess characters that go beyond our allowed limit.
            UserData.TombstoneItem.Epitaph = input.Trim().Truncate(EPITAPH_MAXLENGTH);

            // Confirm with the player this is what they wanted the tombstone to say. Since we truncate need to confirm.
            SetForm(typeof (EpitaphConfirm));
        }
    }
}