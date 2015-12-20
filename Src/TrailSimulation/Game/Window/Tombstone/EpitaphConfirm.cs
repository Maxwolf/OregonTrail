using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Confirms with the user if there is any changes they would like to make to their Tombstone before it gets saved
    ///     for other travelers on this section of the trail to see.
    /// </summary>
    [ParentWindow(GameWindow.Tombstone)]
    public sealed class EpitaphConfirm : InputForm<TombstoneInfo>
    {
        private StringBuilder _confirmPrompt;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EpitaphConfirm(IWindow window) : base(window)
        {
            _confirmPrompt = new StringBuilder();
        }

        /// <summary>
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogType DialogType
        {
            get { return DialogType.YesNo; }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game window and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Clear previous tombstone prompt.
            _confirmPrompt.Clear();

            // Add Tombstone message with here lies player name and their epitaph.
            _confirmPrompt.AppendLine(
                $"{Environment.NewLine}{UserData.Tombstone}");

            // Confirmation message if player would like to edit tombstone.
            _confirmPrompt.AppendLine("Would you like to make");
            _confirmPrompt.Append("changes? Y/N");
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
                    // Add the tombstone instance to manager for future players.
                    GameSimulationApp.Instance.Graveyard.Add(UserData.Tombstone.Clone() as Tombstone);
                    SetForm(typeof(TombstoneView));
                    break;
                case DialogResponse.Yes:
                    // Clears whatever was entered for epitaph before and restarts the entry process for that.
                    UserData.Tombstone.Epitaph = string.Empty;
                    SetForm(typeof (EpitaphEditor));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reponse), reponse, null);
            }
        }
    }
}