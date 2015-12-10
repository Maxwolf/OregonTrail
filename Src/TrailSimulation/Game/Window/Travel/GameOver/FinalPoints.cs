using System;
using System.Linq;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Shows point tabulation based on current simulation statistics. This way if the player dies or finishes the game we
    ///     just attach this state to the travel mode and it will show the final score and reset the game and return to main
    ///     menu when the player is done.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class FinalPoints : Form<TravelInfo>
    {
        /// <summary>
        /// Holds the final point tabulation for the player to see.
        /// </summary>
        private StringBuilder _pointsPrompt;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public FinalPoints(IWindow window) : base(window)
        {
            _pointsPrompt = new StringBuilder();
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderForm()
        {
            // Build up a representation of the current points the player has.
            _pointsPrompt.AppendLine($"{Environment.NewLine}Points for arriving in Oregon");

            // Grab any person that is not dead and check their profession.
            //GameSimulationApp.Instance.

            //_pointsPrompt.AppendLine("");

            return _pointsPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the game Windows current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Completely resets the game to default state it was in when it first started.
            GameSimulationApp.Instance.Restart();
        }
    }
}