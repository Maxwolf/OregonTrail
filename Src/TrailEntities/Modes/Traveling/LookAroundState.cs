using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Tells the player about the point of interest they have just arrived at if they choose to look at it. Some locations
    ///     like the first location will force the player to look around regardless if they want to or not and print a
    ///     different message about traveling back in time.
    /// </summary>
    public sealed class LookAroundState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public LookAroundState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
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
            var welcomePoint = new StringBuilder();
            if (GameSimulationApp.Instance.TrailSim.VehicleLocation == 0)
            {
                // First point of interest has slightly different message about time travel.
                welcomePoint.Append($"Going back to {GameSimulationApp.Instance.Time.CurrentYear}...\n");
            }
            else
            {
                // Every other point of interest will say the name and current date.
                welcomePoint.Append(ParentMode.CurrentPoint);
                welcomePoint.Append(GameSimulationApp.Instance.Time.Date);
            }

            welcomePoint.Append("Press RETURN KEY to continue");
            return welcomePoint.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (UserData.HasLookedAround)
                return;

            UserData.HasLookedAround = true;
            ParentMode.CurrentState = null;
        }
    }
}