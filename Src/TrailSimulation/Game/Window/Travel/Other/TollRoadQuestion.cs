// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TollRoadQuestion.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TrailSimulation.Game
{
    using System;
    using System.Text;
    using Core;

    /// <summary>
    ///     Prompts the user with a question about the toll road location they are attempting to progress to. Depending on
    ///     player cash reserves they might not be able to afford the toll road, in this case the message will only explain
    ///     this to the user and return to the fork in the road or travel menu. If the player can afford it they are asked if
    ///     they would like to proceed, if YES then they will have the monies subtracted from their vehicle inventory and
    ///     proceed to toll road location.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class TollRoadQuestion : InputForm<TravelInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="InputForm{T}"/> class.
        ///     This constructor will be used by the other one</summary>
        /// <param name="window">The window.</param>
        public TollRoadQuestion(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The dialog prompt text.<see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            var tollPrompt = new StringBuilder();

            //tollPrompt.AppendLine($"You must pay {UserData.}");
            tollPrompt.AppendLine("");
            tollPrompt.AppendLine("");

            return tollPrompt.ToString();
        }

        /// <summary>Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.</summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            throw new NotImplementedException();
        }
    }
}