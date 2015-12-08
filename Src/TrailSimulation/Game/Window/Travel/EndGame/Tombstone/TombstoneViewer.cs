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
        ///     References all of the text data that will be shown to user.
        /// </summary>
        private StringBuilder _tombstone;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public TombstoneViewer(IWindow gameMode) : base(gameMode)
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
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Grab the current TombstoneItem based on players progress on the trail so far.
            TombstoneItem foundTombstone;
            UserData.TombstoneManager.Tombstones.TryGetValue(GameSimulationApp.Instance.Vehicle.Odometer,
                out foundTombstone);

            // Check if the tombstone manager returned anything, if not then check for user data it's player death then.
            _tombstone.Clear();
            if (foundTombstone == null && UserData.TombstoneItem != null)
            {
                // Adds TombstoneItem from the user data because the player died.
                _tombstone.AppendLine($"{Environment.NewLine}{UserData.TombstoneItem}");
            }
            else if (foundTombstone != null && UserData.TombstoneItem == null)
            {
                // Add TombstoneItem message we want to show the player from TombstoneItem manager.
                _tombstone.AppendLine($"{Environment.NewLine}{foundTombstone}");
            }
            else
            {
                // Could not find TombstoneItem in either the manager or user data we have none to actually display...
                _tombstone.AppendLine($"{Environment.NewLine}[NO TOMBSTONE FOUND!]");
            }

            return _tombstone.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Determine if we are showing the player a tombstone because they died.
            if (UserData.TombstoneItem != null)
            {
                // Completely resets the game to default state it was in when it first started.
                GameSimulationApp.Instance.Restart();
                return;
            }

            // Return to travel mode menu if we are just looking at some other dead guy grave.
            ClearForm();
        }
    }
}