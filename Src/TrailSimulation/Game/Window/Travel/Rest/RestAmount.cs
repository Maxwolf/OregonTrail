using System;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Attaches a state that will ask the player how long they would like to rest in the number of days, zero is a valid
    ///     response and will not do anything. If greater than zero we will attach another state to tick that many days by in
    ///     the simulation.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class RestAmount : Form<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RestAmount(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderForm()
        {
            return Environment.NewLine + "How many days would you like to rest?";
        }

        /// <summary>
        ///     Fired when the game Windows current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Parse the user input buffer as a unsigned int.
            int parsedInputNumber;
            if (!int.TryParse(input, out parsedInputNumber))
                return;

            // If player rests for more than one day then set the resting state to that, otherwise just go back to travel menu.
            UserData.DaysToRest = parsedInputNumber;
            if (parsedInputNumber > 0)
                SetForm(typeof (Resting));
            else
                ClearForm();
        }
    }
}