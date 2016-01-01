// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/29/2015@9:55 PM

namespace TrailSimulation
{
    using System;
    using SimUnit;
    using SimUnit.Form;
    using SimUnit.Form.Input;

    /// <summary>
    ///     Called when the player was targeting a given animal but waited to long or took to long typing in the shooting word
    ///     and the prey ran away after sensing the players intent to shoot it.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class PreyFlee : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InputForm{T}" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public PreyFlee(IWindow window) : base(window)
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
            // Get the last known escaped prey item.
            return
                $"{Environment.NewLine}The {UserData.Hunt.LastEscapee.Animal.Name.ToLowerInvariant()} senses danger " +
                $"and runs away from you.{Environment.NewLine}{Environment.NewLine}";
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