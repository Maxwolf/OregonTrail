// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Collections.Generic;
using System.Text;
using OregonTrailDotNet.Entity.Person;
using WolfCurses.Utility;
using WolfCurses.Window;
using WolfCurses.Window.Control;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.MainMenu.Help
{
    /// <summary>
    ///     First panel on point information, shows how health of party members contributes to final score.
    /// </summary>
    [ParentWindow(typeof(MainMenu))]
    public sealed class PointsDistributionHelp : InputForm<NewGameInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PointsDistributionHelp" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
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
            var pointsHealth = new StringBuilder();
            pointsHealth.AppendLine($"{Environment.NewLine}On Arriving in Oregon{Environment.NewLine}");
            pointsHealth.AppendLine("Your most important resource is the");
            pointsHealth.AppendLine("people you have with you. You");
            pointsHealth.AppendLine("receive points for each member of");
            pointsHealth.AppendLine("your party who arrives safely; you");
            pointsHealth.AppendLine("receive more points if they arrive");
            pointsHealth.AppendLine($"in good health!{Environment.NewLine}");

            // Repair status reference dictionary.
            var repairLevels = new Dictionary<string, int>();
            foreach (var repairStat in Enum.GetNames(typeof(HealthStatus)))
                repairLevels.Add(repairStat, (int) Enum.Parse(typeof(HealthStatus), repairStat));

            // Build a text table from people point distribution with custom headers.
            var partyTextTable = repairLevels.Values.ToStringTable(
                new[] {"HealthStatus of Party", "Points per Person"},
                u => Enum.Parse(typeof(HealthStatus), u.ToString()).ToDescriptionAttribute(),
                u => u);

            // Print the table to the screen buffer.
            pointsHealth.AppendLine(partyTextTable);
            return pointsHealth.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // parentGameMode.State = new PointsAwardHelp(parentGameMode, UserData);
            SetForm(typeof(PointsAwardHelp));
        }
    }
}