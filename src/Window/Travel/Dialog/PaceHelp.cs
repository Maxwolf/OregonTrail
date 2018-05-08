// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using OregonTrailDotNet.Window.Travel.Command;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.Dialog
{
    /// <summary>
    ///     Shows information about what the different pace settings mean in terms for the simulation and how they will affect
    ///     vehicle, party, and events.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class PaceHelp : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PaceHelp" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
        public PaceHelp(IWindow window) : base(window)
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
            // Steady
            var paceHelp = new StringBuilder();
            paceHelp.Append($"{Environment.NewLine}steady - You travel about 8 hours a{Environment.NewLine}");
            paceHelp.Append($"day, taking frequent rests. You take{Environment.NewLine}");
            paceHelp.Append($"care not to get too tired.{Environment.NewLine}{Environment.NewLine}");

            // Strenuous
            paceHelp.Append($"strenuous - You travel about 12 hours{Environment.NewLine}");
            paceHelp.Append($"a day, starting just after sunrise{Environment.NewLine}");
            paceHelp.Append($"and stopping shortly before sunset.{Environment.NewLine}");
            paceHelp.Append($"You stop to rest only when necessary.{Environment.NewLine}");
            paceHelp.Append($"You finish each day feeling very{Environment.NewLine}");
            paceHelp.Append($"tired.{Environment.NewLine}{Environment.NewLine}");

            // Grueling
            paceHelp.Append($"grueling - You travel about 16 hours{Environment.NewLine}");
            paceHelp.Append($"a day, starting before sunrise and{Environment.NewLine}");
            paceHelp.Append($"continuing until dark. You almost{Environment.NewLine}");
            paceHelp.Append($"never stop to rest. You do not get{Environment.NewLine}");
            paceHelp.Append($"enough sleep at night. You finish{Environment.NewLine}");
            paceHelp.Append($"each day feeling absolutely{Environment.NewLine}");
            paceHelp.Append($"exhausted, and your health suffers.{Environment.NewLine}{Environment.NewLine}");
            return paceHelp.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // parentGameMode.State = new ChangePace(parentGameMode, UserData);
            SetForm(typeof(ChangePace));
        }
    }
}