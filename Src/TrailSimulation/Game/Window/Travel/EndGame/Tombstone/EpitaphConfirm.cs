using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Confirms with the user if there is any changes they would like to make to their TombstoneItem before it gets saved
    ///     for
    ///     other travelers on this section of the trail to see.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class EpitaphConfirm : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EpitaphConfirm(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var _confirmPrompt = new StringBuilder();

            // Add TombstoneItem message with here lies player name and their epitaph.
            _confirmPrompt.Clear();
            _confirmPrompt.AppendLine($"{Environment.NewLine}{UserData.TombstoneItem}");
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
                    // Add the TombstoneItem as is to the TombstoneItem manager for future players to see.
                    UserData.TombstoneManager.Add(UserData.TombstoneItem);
                    SetForm(typeof (TombstoneViewer));
                    break;
                case DialogResponse.Yes:
                    // Clears whatever was entered for epitaph before and restarts the entry process for that.
                    UserData.TombstoneItem.Epitaph = string.Empty;
                    SetForm(typeof (EpitaphEditor));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reponse), reponse, null);
            }
        }
    }
}