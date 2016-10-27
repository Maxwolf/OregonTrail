// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using OregonTrailDotNet.Event.Vehicle;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.RandomEvent
{
    /// <summary>
    ///     Called by the event system when one of the events damages the vehicle in a permanent way that requires the player
    ///     to repair or replace the broken part which has been selected prior to this.
    /// </summary>
    [ParentWindow(typeof(RandomEvent))]
    public sealed class VehicleBrokenPrompt : InputForm<RandomEventInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InputForm{T}" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public VehicleBrokenPrompt(IWindow window) : base(window)
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
        /// <returns>
        ///     The dialog prompt text.<see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            var brokenPrompt = new StringBuilder();
            brokenPrompt.AppendLine(
                $"{Environment.NewLine}Broken {GameSimulationApp.Instance.Vehicle.BrokenPart.Name.ToLowerInvariant()}. Would you");
            brokenPrompt.Append("like to try and repair it? Y/N");
            return brokenPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Get game instance to improve readability.
            var game = GameSimulationApp.Instance;

            switch (reponse)
            {
                case DialogResponse.No:
                case DialogResponse.Custom:
                    // Player has chosen to not attempt a repair and opted instead for replacement with spare part.
                    game.EventDirector.TriggerEvent(game.Vehicle, typeof(NoRepairVehicle));
                    break;
                case DialogResponse.Yes:
                    // Depending on dice roll player might be able to fix their broken vehicle part.
                    game.EventDirector.TriggerEvent(game.Vehicle,
                        game.Random.NextBool() ? typeof(RepairVehiclePart) : typeof(NoRepairVehicle));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reponse), reponse, null);
            }
        }
    }
}