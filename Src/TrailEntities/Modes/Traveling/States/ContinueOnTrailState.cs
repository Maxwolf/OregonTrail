using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Attached when the player wants to continue on the trail, and doing so will force them to leave that point and be
    ///     back on the trail counting up distance traveled until they reach the next one. The purpose of this state is to
    ///     inform the player of the next points name, the distance away that it is, and that is all it will close and
    ///     simulation resume after return key is pressed.
    /// </summary>
    public sealed class ContinueOnTrailState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     Keeps track if we have already triggered the state to remove itself after telling player about distance to next
        ///     one.
        /// </summary>
        private bool hasContinuedOnTrail;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ContinueOnTrailState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return false; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            var nextStop = new StringBuilder();
            var nextPoint = GameSimulationApp.Instance.TrailSim.GetNextPointOfInterest();
            nextStop.Append($"From {ParentMode.CurrentPoint.Name} it is {nextPoint.DistanceLength} ");
            nextStop.Append($"miles to the {nextPoint.Name}");
            return nextStop.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (hasContinuedOnTrail)
                return;

            // Close the continue on screen when we get any user input.
            hasContinuedOnTrail = true;
            ParentMode.CurrentState = null;

            // Inform the time simulation it may resume.
            GameSimulationApp.Instance.Time.ResumeTime();
        }
    }
}