// Created by Maxwolf (bigmaxwolf.com) 
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
        // ReSharper disable once UnusedMember.Global
        public VehicleBrokenPrompt(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogType DialogType => DialogType.YesNo;

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The dialog prompt text.<see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            // Describe the part on the vehicle the event actually targeted (the game vehicle during normal play), rather
            // than assuming the global singleton's vehicle — the two can differ, which would null-reference here.
            var vehicle = UserData.SourceEntity as Entity.Vehicle.Vehicle ?? GameSimulationApp.Instance.Vehicle;

            var brokenPrompt = new StringBuilder();
            brokenPrompt.AppendLine(
                $"{Environment.NewLine}Broken {vehicle.BrokenPart.Name.ToLowerInvariant()}. Would you");
            brokenPrompt.Append("like to try and repair it? Y/N");
            return brokenPrompt.ToString();
        }

        /// <summary>
        ///     Sets a context-specific input prompt in place of the generic "What is your choice?", then renders as normal.
        /// </summary>
        public override string OnRenderForm()
        {
            ParentWindow.PromptText = "Yes or no?";
            return base.OnRenderForm();
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

            // Act on the vehicle the event targeted (the game vehicle during normal play), matching OnDialogPrompt.
            var vehicle = UserData.SourceEntity as Entity.Vehicle.Vehicle ?? game.Vehicle;

            switch (reponse)
            {
                case DialogResponse.No:
                case DialogResponse.Custom:
                    // Player has chosen to not attempt a repair and opted instead for replacement with spare part.
                    game.EventDirector.TriggerEvent(vehicle, typeof(NoRepairVehicle));
                    break;
                case DialogResponse.Yes:
                    // Depending on dice roll player might be able to fix their broken vehicle part.
                    game.EventDirector.TriggerEvent(vehicle,
                        game.Random.NextBool() ? typeof(RepairVehiclePart) : typeof(NoRepairVehicle));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reponse), reponse, null);
            }
        }
    }
}