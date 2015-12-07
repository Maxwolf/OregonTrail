using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Special form we can use to view existing tombstones as a dialog offering no changes or input from other players
    ///     letting them only look at the name and epitaph message left if any.
    /// </summary>
    [ParentWindow(Windows.Travel)]
    public sealed class TombstoneViewer : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public TombstoneViewer(IWindow gameMode) : base(gameMode)
        {
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
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Grab the current TombstoneItem based on players progress on the trail so far.
            var currentStone = UserData.TombstoneManager;
            if (currentStone == null)
                throw new ArgumentException("Unable to get current TombstoneItem from vehicle total miles traveled!");

            // Add TombstoneItem message we want to show the player from TombstoneItem manager.
            var _tombstone = new StringBuilder();
            _tombstone.AppendLine($"{Environment.NewLine}{currentStone}");

            return _tombstone.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Return to travel mode menu.
            ClearForm();
        }
    }
}