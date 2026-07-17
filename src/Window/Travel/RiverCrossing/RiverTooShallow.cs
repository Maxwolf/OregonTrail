// Created by Maxwolf (bigmaxwolf.com)

using System;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.RiverCrossing
{
    /// <summary>
    ///     Told to the player when they try to caulk and float a wagon across water that is not deep enough to carry it. A
    ///     wagon needs something to swim in; over a shallow bed it simply grounds. The crossing is not lost - they are put
    ///     back to the river menu to ford it instead.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class RiverTooShallow : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RiverTooShallow" /> class.
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
        public RiverTooShallow(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>The dialog prompt text.</returns>
        protected override string OnDialogPrompt()
        {
            return $"{Environment.NewLine}The river is too shallow to float across.{Environment.NewLine}";
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
