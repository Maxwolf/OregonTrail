// Created by Maxwolf (bigmaxwolf.com)

using System;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.RiverCrossing
{
    /// <summary>
    ///     Told to the player when they ask for the ferry across water the ferryman cannot float his boat on. He is not
    ///     refusing the fare out of spite; there is simply not enough river. A party turned away here can ford it themselves
    ///     for nothing, which is the point: the ferry is only worth paying for when the river is high.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class FerryNotRunning : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FerryNotRunning" /> class.
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
        public FerryNotRunning(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>The dialog prompt text.</returns>
        protected override string OnDialogPrompt()
        {
            return $"{Environment.NewLine}The ferry is not operating today because the river is too shallow." +
                   $"{Environment.NewLine}";
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponseEnum reponse)
        {
            SetForm(typeof(RiverCross));
        }
    }
}
