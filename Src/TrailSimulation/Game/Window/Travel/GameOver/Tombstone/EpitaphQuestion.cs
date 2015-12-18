using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Asks the user if they would like to write a custom message on their Tombstone for other users to see when the
    ///     come
    ///     across this part of the trail in the future.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class EpitaphQuestion : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EpitaphQuestion(IWindow window) : base(window)
        {
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
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var epitaphPrompt = new StringBuilder();

            // Add Tombstone message with here lies player name, no epitaph yet.
            epitaphPrompt.Clear();
            epitaphPrompt.Append($"{Environment.NewLine}{GameSimulationApp.Instance.Graveyard.TempTombstone}");
            epitaphPrompt.AppendLine($"{Environment.NewLine}Would you like to write");
            epitaphPrompt.Append("an epitaph? Y/N");
            return epitaphPrompt.ToString();
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
                case DialogResponse.Yes:
                    // Allows player to enter custom message on their grave...
                    SetForm(typeof (EpitaphEditor));
                    break;
                case DialogResponse.No:
                case DialogResponse.Custom:
                    // Add the Tombstone as is to the Tombstone manager for future players to see.
                    GameSimulationApp.Instance.Graveyard.Add(GameSimulationApp.Instance.Graveyard.TempTombstone);
                    GameSimulationApp.Instance.Graveyard.ClearTempTombstone();
                    SetForm(typeof (TombstoneViewer));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reponse), reponse, null);
            }
        }
    }
}