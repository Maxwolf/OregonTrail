using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Confirms with the user if there is any changes they would like to make to their TombstoneItem before it gets saved for
    ///     other travelers on this section of the trail to see.
    /// </summary>
    [ParentWindow(Windows.Travel)]
    public sealed class EpitaphConfirm : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EpitaphConfirm(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var _confirmPrompt = new StringBuilder();

            // TODO: Add TombstoneItem message with here lies player name and their epitaph.

            _confirmPrompt.AppendLine("Would you like to make");
            _confirmPrompt.Append("changes?");
            return _confirmPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            switch (reponse)
            {
                case DialogResponse.Custom:
                case DialogResponse.No:
                    // Restarts the game simulation at the main menu, allowing the player to start a new game, view scores, and exit.
                    GameSimulationApp.Instance.WindowManager.Add(Windows.MainMenu);
                    break;
                case DialogResponse.Yes:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reponse), reponse, null);
            }
        }
    }
}