// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 02/01/2016@11:23 PM

namespace TrailSimulation
{
    using System;
    using WolfCurses;
    using WolfCurses.Form;
    using WolfCurses.Form.Input;

    /// <summary>
    ///     Special form used by random event system when communicating to the user they were able to use a spare part in the
    ///     vehicle inventory to fix the vehicle so it may continue down the trail.
    /// </summary>
    [ParentWindow(typeof (RandomEvent))]
    public sealed class VehicleUseSparePart : InputForm<RandomEventInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InputForm{T}" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public VehicleUseSparePart(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogType DialogType
        {
            get { return DialogType.Prompt; }
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            get { return false; }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The dialog prompt text.<see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            return $"{Environment.NewLine}You were able to repair the " +
                   $"{GameSimulationApp.Instance.Vehicle.BrokenPart.Name.ToLowerInvariant()} using your spare.{Environment.NewLine}{Environment.NewLine}";
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Check to make sure the source entity is a vehicle.
            var vehicle = UserData.SourceEntity as Vehicle;
            if (vehicle == null)
                return;

            // Ensures the vehicle will be able to continue down the trail.
            vehicle.Status = VehicleStatus.Stopped;

            // Set broken part to nothing.
            vehicle.BrokenPart = null;

            // Removes this form and random event window.
            ParentWindow.RemoveWindowNextTick();
        }
    }
}