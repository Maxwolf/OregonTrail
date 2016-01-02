// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@7:40 PM

namespace TrailSimulation
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using WolfCurses;
    using WolfCurses.Control;
    using WolfCurses.Form;
    using WolfCurses.Form.Input;

    /// <summary>
    ///     First panel on point information, shows how health of party members contributes to final score.
    /// </summary>
    [ParentWindow(typeof (MainMenu))]
    public sealed class PointsDistributionHelp : InputForm<NewGameInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PointsDistributionHelp" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public PointsDistributionHelp(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            // Build up string of help about points for people.
            var _pointsHealth = new StringBuilder();
            _pointsHealth.AppendLine($"{Environment.NewLine}On Arriving in Oregon{Environment.NewLine}");
            _pointsHealth.AppendLine("Your most important resource is the");
            _pointsHealth.AppendLine("people you have with you. You");
            _pointsHealth.AppendLine("receive points for each member of");
            _pointsHealth.AppendLine("your party who arrives safely; you");
            _pointsHealth.AppendLine("receive more points if they arrive");
            _pointsHealth.AppendLine($"in good health!{Environment.NewLine}");

            // Repair status reference dictionary.
            var _repairLevels = new Dictionary<string, int>();
            foreach (var repairStat in Enum.GetNames(typeof (HealthStatus)))
            {
                _repairLevels.Add(repairStat, (int) Enum.Parse(typeof (HealthStatus), repairStat));
            }

            // Build a text table from people point distribution with custom headers.
            var partyTextTable = _repairLevels.Values.ToStringTable(
                new[] {"HealthStatus of Party", "Points per Person"},
                u => Enum.Parse(typeof (HealthStatus), u.ToString()).ToDescriptionAttribute(),
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
            // parentGameMode.State = new PointsAwardHelp(parentGameMode, UserData);
            SetForm(typeof (PointsAwardHelp));
        }
    }
}