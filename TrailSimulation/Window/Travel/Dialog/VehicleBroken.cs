// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Window.Travel.Dialog
{
    using System;
    using System.Text;
    using WolfCurses.Window;
    using WolfCurses.Window.Form;
    using WolfCurses.Window.Form.Input;

    /// <summary>
    ///     Vehicle is unable to continue down the trail because some of the parts that makeup the vehicle infrastructure have
    ///     become damaged or malfunctioned meaning the vehicle cannot continue down the trail until they are repaired or
    ///     replaced.
    /// </summary>
    [ParentWindow(typeof (Travel))]
    public sealed class VehicleBroken : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InputForm{T}" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public VehicleBroken(IWindow window) : base(window)
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
            var stuckPrompt = new StringBuilder();
            stuckPrompt.AppendLine(
                $"{Environment.NewLine}You must trade for an {GameSimulationApp.Instance.Vehicle.BrokenPart.Name.ToLowerInvariant()}");
            stuckPrompt.AppendLine($"to be able to continue.{Environment.NewLine}");
            return stuckPrompt.ToString();
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