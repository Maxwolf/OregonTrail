using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Asks the user if they would like to write a custom message on their tombstone for other users to see when the come
    ///     across this part of the trail in the future.
    /// </summary>
    [ParentWindow(Windows.Travel)]
    public sealed class EpitaphQuestion : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EpitaphQuestion(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var epitaphPrompt = new StringBuilder();

            // TODO: Add tombstone message with here lies player name, no epitaph yet.

            epitaphPrompt.AppendLine("Would you like to write");
            epitaphPrompt.Append("an epitaph?");
            return epitaphPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            throw new NotImplementedException();
        }
    }
}