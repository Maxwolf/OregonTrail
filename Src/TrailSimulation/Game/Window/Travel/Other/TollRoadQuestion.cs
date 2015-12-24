// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/23/2015@2:52 PM

namespace TrailSimulation.Game
{
    using System;
    using System.Text;
    using Core;
    using Entity;

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
        /// <summary>
        ///     Figures out of the vehicle has enough cash to use the toll road, this is generally used as a check for the dialog.
        /// </summary>
        private bool canAffordToll;

        /// <summary>
        ///     Reference to game simulation.
        /// </summary>
        private GameSimulationApp game;

        /// <summary>
        ///     Holds reference to the actual toll road so we can use it as a reference for how much the player owes.
        /// </summary>
        private TollRoad tollRoad;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InputForm{T}" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public TollRoadQuestion(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            get { return canAffordToll; }
        }

        /// <summary>
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogType DialogType
        {
            get { return canAffordToll ? DialogType.YesNo : DialogType.Prompt; }
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Grab instance of the game simulation.
            game = GameSimulationApp.Instance;

            // Cast the current location as a toll road.
            tollRoad = game.Trail.CurrentLocation as TollRoad;
            if (tollRoad == null)
                throw new InvalidCastException("Unable to cast current location to toll road.");

            canAffordToll = game.Vehicle.Inventory[Entities.Cash].TotalValue >= tollRoad.Cost;
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

            // First portion of the message that both get no matter what.
            tollPrompt.AppendLine($"You must pay {tollRoad.Cost.ToString("C0")} to travel the");
            tollPrompt.Append($"{tollRoad.Name}.");

            // Check if the player has enough money to pay for the toll road.
            if (game.Vehicle.Inventory[Entities.Cash].TotalValue >= tollRoad.Cost)
            {
                tollPrompt.AppendLine("Are you willing");
                tollPrompt.AppendLine("to do this? Y/N");
            }
            else
            {
                tollPrompt.AppendLine("You don't have enough");
                tollPrompt.AppendLine("cash for the toll road.");
            }

            return tollPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Check if the player has enough monies to pay for the toll road.
            if (!canAffordToll)
            {
                CancelTollQuestion();
                return;
            }

            // 
        }

        /// <summary>
        ///     Calls the continue on trail method command inside that game Windows, it will trigger the next action accordingly.
        /// </summary>
        private void CancelTollQuestion()
        {
            var travelMode = ParentWindow as Travel;
            if (travelMode == null)
                throw new InvalidCastException(
                    "Unable to cast parent game Windows into travel game Windows when it should be that!");

            travelMode.ContinueOnTrail();
        }
    }
}