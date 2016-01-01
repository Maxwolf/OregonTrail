// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:38 AM

namespace TrailSimulation
{
    using System;
    using SimUnit;
    using SimUnit.Form;
    using SimUnit.Form.Input;

    /// <summary>
    ///     Shown when the player does not have enough bullets to go hunting, this prevents them from wasting the time of
    ///     loading the game mode just so nothing can happen until it times out. Rather than letting the player suffer from
    ///     that mistake we will just tell them they don't have enough.
    /// </summary>
    [ParentWindow(typeof (Travel))]
    public sealed class NoAmmo : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InputForm{T}" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public NoAmmo(IWindow window) : base(window)
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
            return $"{Environment.NewLine}You need more bullets{Environment.NewLine}" +
                   $"to go hunting.{Environment.NewLine}{Environment.NewLine}";
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            ClearForm();
        }
    }
}