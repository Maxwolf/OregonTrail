using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Attached when the party leader dies, or the vehicle reaches the end of the trail.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class GameWin : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Holds reference to end game text that will be shown to the user.
        /// </summary>
        private StringBuilder _gameOver;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public GameWin(IWindow window) : base(window)
        {
            _gameOver = new StringBuilder();
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            _gameOver.AppendLine("Congratulations! You have ");
            _gameOver.AppendLine("made it to Oregon! Let's see ");
            _gameOver.AppendLine("how many points you have ");
            _gameOver.Append("received.");
            return _gameOver.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            SetForm(typeof (FinalPoints));
        }
    }
}