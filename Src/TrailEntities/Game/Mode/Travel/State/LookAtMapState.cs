using System;
using System.Text;
using TrailEntities.Simulation;
using TrailEntities.Widget;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Shows the player their vehicle and list of all the points in the trail they could possibly travel to. It marks the
    ///     spot they are on and all the spots they have visited, shows percentage for completion and some other basic
    ///     statistics about the journey that could only be seen from this state.
    /// </summary>
    public sealed class LookAtMapState : DialogState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public LookAtMapState(IModeProduct gameMode, TravelInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Create visual progress representation of the trail.
            var _map = new StringBuilder();
            _map.Append($"{Environment.NewLine}Trail progress{Environment.NewLine}");
            _map.AppendLine(TextProgress.DrawProgressBar(
                GameSimulationApp.Instance.Trail.LocationIndex,
                GameSimulationApp.Instance.Trail.Locations.Count, 32));
            return _map.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.CurrentState = null;
            ClearState();
        }
    }
}