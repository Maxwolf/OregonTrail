using System;
using System.Collections.Generic;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     First panel on point information, shows how health of party members contributes to final score.
    /// </summary>
    [RequiredMode(Mode.MainMenu)]
    public sealed class PointsHealthState : DialogState<MainMenuInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public PointsHealthState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Build up string of help about points for people.
            var _pointsHealth = new StringBuilder();
            _pointsHealth.Append($"{Environment.NewLine}On Arriving in Oregon{Environment.NewLine}{Environment.NewLine}");
            _pointsHealth.Append($"Your most important resource is the{Environment.NewLine}");
            _pointsHealth.Append($"people you have with you. You{Environment.NewLine}");
            _pointsHealth.Append($"receive points for each member of{Environment.NewLine}");
            _pointsHealth.Append($"your party who arrives safely; you{Environment.NewLine}");
            _pointsHealth.Append($"receive more points if they arrive{Environment.NewLine}");
            _pointsHealth.Append($"in good health!{Environment.NewLine}{Environment.NewLine}");

            // Repair status reference dictionary.
            var _repairLevels = new Dictionary<string, int>();
            foreach (var repairStat in Enum.GetNames(typeof (RepairLevel)))
            {
                _repairLevels.Add(repairStat, (int) Enum.Parse(typeof (RepairLevel), repairStat));
            }

            // Build a text table from people point distribution with custom headers.
            var partyTextTable = _repairLevels.Values.ToStringTable(
                new[] {"Health of Party", "Points per Person"},
                u => Enum.Parse(typeof (RepairLevel), u.ToString()).ToString(),
                u => u);

            // Print the table to the screen buffer.
            _pointsHealth.AppendLine(partyTextTable);
            return _pointsHealth.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.CurrentState = new PointsResourcesState(parentGameMode, UserData);
            SetState(typeof (PointsResourcesState));
        }
    }
}