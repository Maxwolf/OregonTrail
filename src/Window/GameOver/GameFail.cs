// Created by Maxwolf (bigmaxwolf.com)
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using OregonTrailDotNet.Entity.Person;
using WolfCurses.Utility;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.GameOver
{
    /// <summary>
    ///     Fired when the simulation has determined the entire party has died. It shows the player what happened - the cause of
    ///     death, how far they traveled, and what supplies remained - before handing off to the graveyard where they can leave
    ///     an epitaph and the game resets.
    /// </summary>
    [ParentWindow(typeof(GameOver))]
    public sealed class GameFail : InputForm<GameOverInfo>
    {
        /// <summary>
        ///     Holds reference to the death summary text shown to the user.
        /// </summary>
        private readonly StringBuilder _failPrompt;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameFail" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
        public GameFail(IWindow window) : base(window)
        {
            _failPrompt = new StringBuilder();
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>The dialog prompt text.</returns>
        protected override string OnDialogPrompt()
        {
            var game = GameSimulationApp.Instance;

            _failPrompt.Clear();
            _failPrompt.AppendLine($"{Environment.NewLine}Your party has perished.{Environment.NewLine}");

            // Explain the cause of death when the party leader has a recorded one, otherwise fall back to a generic line.
            var leader = game.Vehicle.PassengerLeader;
            if ((leader != null) && (leader.Cause != CauseOfDeathEnum.Unknown))
                _failPrompt.AppendLine($"{leader.Name} {leader.Cause.ToDescriptionAttribute()}.");

            // Show how far the party managed to travel before they died.
            _failPrompt.AppendLine($"You traveled {game.Vehicle.Odometer:N0} miles.{Environment.NewLine}");

            // Show whatever supplies remained in the wagon (with the money left over).
            _failPrompt.AppendLine("Remaining supplies:");
            _failPrompt.AppendLine(SupplyPanel.Build(includeCash: true));

            return _failPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponseEnum reponse)
        {
            // Move on to the graveyard where the player can leave an epitaph; the graveyard flow resets the game.
            GameSimulationApp.Instance.WindowManager.Add(typeof(Graveyard.Graveyard));
        }
    }
}
