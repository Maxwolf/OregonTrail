using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Information about what fording a river means and how it works for the player vehicle and their party members.
    /// </summary>
    [ParentWindow(Windows.Travel)]
    public sealed class FordRiverHelp : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public FordRiverHelp(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var fordRiver = new StringBuilder();
            fordRiver.AppendLine($"{Environment.NewLine}To ford a river means to");
            fordRiver.AppendLine("pull your wagon across a");
            fordRiver.AppendLine("shallow part of the river,");
            fordRiver.AppendLine("with the oxen still");
            fordRiver.AppendLine($"attached.{Environment.NewLine}");
            return fordRiver.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.State = new CaulkRiverHelp(parentGameMode, UserData);
            SetForm(typeof (CaulkRiverHelp));
        }
    }
}