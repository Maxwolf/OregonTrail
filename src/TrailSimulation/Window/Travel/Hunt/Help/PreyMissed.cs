// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using OregonTrailDotNet.WolfCurses.Window;
using OregonTrailDotNet.WolfCurses.Window.Form;
using OregonTrailDotNet.WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.TrailSimulation.Window.Travel.Hunt.Help
{
    /// <summary>
    ///     Called when the player fires a shot and it misses the intended target.
    /// </summary>
    [ParentWindow(typeof (Travel))]
    public sealed class PreyMissed : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InputForm{T}" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public PreyMissed(IWindow window) : base(window)
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
            return
                $"{Environment.NewLine}You missed, and the {UserData.Hunt.LastEscapee.Animal.Name.ToLowerInvariant()} " +
                $"got away!{Environment.NewLine}{Environment.NewLine}";
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            SetForm(typeof (Hunting));
        }
    }
}