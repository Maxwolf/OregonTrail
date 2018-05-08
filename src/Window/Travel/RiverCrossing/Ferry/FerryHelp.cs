// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.RiverCrossing.Ferry
{
    /// <summary>
    ///     The ferry help.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class FerryHelp : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FerryHelp" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
        public FerryHelp(IWindow window) : base(window)
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
            var prompt = new StringBuilder();
            prompt.AppendLine($"{Environment.NewLine}To use a ferry means to put");
            prompt.AppendLine("your wagon on top of a flat");
            prompt.AppendLine("boat that belongs to someone");
            prompt.AppendLine("else. The owner of the");
            prompt.AppendLine("ferry will take your wagon");
            prompt.AppendLine($"across the river.{Environment.NewLine}");
            return prompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            SetForm(typeof(RiverCross));
        }
    }
}