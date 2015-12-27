// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/26/2015@7:11 PM

namespace TrailSimulation.Game
{
    using System;
    using Core;

    /// <summary>
    ///     Tabulates results about the hunting session after it ends, depending on the performance of the player and how many
    ///     animals they killed, if any will be calculated in weight. Players can only ever take 100 pounds of meat back to
    ///     the vehicle so this discourages mass killing.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class HuntingResult : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InputForm{T}" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public HuntingResult(IWindow window) : base(window)
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
            throw new NotImplementedException();
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