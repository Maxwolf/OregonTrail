using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Shows the player their vehicle and list of all the points in the trail they could possibly travel to. It marks the
    ///     spot they are on and all the spots they have visited, shows percentage for completion and some other basic
    ///     statistics about the journey that could only be seen from this state.
    /// </summary>
    [RequiredWindow(Windows.Travel)]
    public sealed class LookAtMapState : DialogState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public LookAtMapState(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Create visual progress representation of the trail.
            var _map = new StringBuilder();
            _map.AppendLine($"{Environment.NewLine}Trail progress{Environment.NewLine}");
            _map.AppendLine(TextProgress.DrawProgressBar(
                GameSimulationApp.Instance.Trail.LocationIndex,
                GameSimulationApp.Instance.Trail.Locations.Count, 32) + Environment.NewLine);

            // Build up a table of location names and if the player has visited them.
            var locationTable = GameSimulationApp.Instance.Trail.Locations.ToStringTable(
                new[] {"Visited", "Location Name"},
                u => u.Status >= LocationStatus.Arrived,
                u => u.Name
                );
            _map.AppendLine(locationTable);

            return _map.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            ClearForm();
        }
    }
}