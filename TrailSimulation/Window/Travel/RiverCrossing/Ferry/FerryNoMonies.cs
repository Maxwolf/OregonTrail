// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:38 AM

namespace TrailSimulation.Window.Travel.RiverCrossing.Ferry
{
    using System;
    using System.Text;

    /// <summary>
    ///     Attached when the user attempts to cross the river using the ferry, confirms they would like to but does not have
    ///     enough money at this point this state will be attached and explain to the user they cannot use the ferry and must
    ///     pick one of the other two options.
    /// </summary>
    [ParentWindow(typeof (Travel))]
    public sealed class FerryNoMonies : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FerryNoMonies" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public FerryNoMonies(IWindow window) : base(window)
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
            var _prompt = new StringBuilder();
            _prompt.AppendLine($"{Environment.NewLine}You do not have enough");
            _prompt.AppendLine("monies to take the");
            _prompt.AppendLine($"ferry.{Environment.NewLine}");
            return _prompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            SetForm(typeof (RiverCross));
        }
    }
}