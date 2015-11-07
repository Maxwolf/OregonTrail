using System;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Shows the player their vehicle and list of all the points in the trail they could possibly travel to. It marks the
    ///     spot they are on and all the spots they have visited, shows percentage for completion and some other basic
    ///     statistics about the journey that could only be seen from this state.
    /// </summary>
    public sealed class LookAtMapState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     Determines if the player is done looking at the map and total progress of their journey.
        /// </summary>
        private bool _hasLookedAtMap;

        /// <summary>
        ///     Contains all the text that will make up our console text only map to show the player how far along they have come.
        /// </summary>
        private StringBuilder _map;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public LookAtMapState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
        {
            // Create visual progress representation of the trail.
            _map = new StringBuilder();
            _map.Append($"{Environment.NewLine}Trail progress{Environment.NewLine}");
            _map.AppendLine(TextProgress.DrawProgressBar(
                GameSimApp.Instance.Trail.LocationIndex,
                GameSimApp.Instance.Trail.Locations.Count, 32));

            // Wait for user input...
            _map.Append($"{Environment.NewLine}{GameSimApp.PRESS_ENTER}");
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
        public override string OnRenderState()
        {
            return _map.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_hasLookedAtMap)
                return;

            _hasLookedAtMap = true;
            ParentMode.CurrentState = null;
        }
    }
}