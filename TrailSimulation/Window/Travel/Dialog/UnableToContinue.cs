// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:38 AM

namespace TrailSimulation.Window.Travel.Dialog
{
    using System;
    using System.Text;

    /// <summary>
    ///     Something has happened with the players vehicle and they are no longer able to continue on the trail. This form is
    ///     shown normally after it happens and or is detected. After the player sees the dialog it will be cleared and the
    ///     user shown the travel menu where they can wait, trade, if they try to continue again and problem is not fixed this
    ///     form will be shown again.
    /// </summary>
    [ParentWindow(typeof (Travel))]
    public sealed class UnableToContinue : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UnableToContinue" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public UnableToContinue(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            get { return false; }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            var stuckPrompt = new StringBuilder();
            stuckPrompt.AppendLine($"{Environment.NewLine}You are unable to continue");
            stuckPrompt.AppendLine("your journey. You have no");
            stuckPrompt.AppendLine($"oxen to pull your wagon.{Environment.NewLine}");
            return stuckPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Returns to travel command menu.
            ClearForm();
        }
    }
}