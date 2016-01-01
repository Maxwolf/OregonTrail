// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Window.Travel.Hunt.Help
{
    using System;
    using System.Text;
    using WolfCurses.Utility;
    using WolfCurses.Window;
    using WolfCurses.Window.Form;
    using WolfCurses.Window.Form.Input;

    /// <summary>
    ///     Prompt that proceeds the hunting mode when accessed from the travel menu. Explains to the player how the controls
    ///     work and what is expected of them in regards to how the game mode operates.
    /// </summary>
    [ParentWindow(typeof (Travel))]
    public sealed class HuntingPrompt : InputForm<TravelInfo>
    {
        /// <summary>
        ///     References the message we show to the user that explains how hunting works.
        /// </summary>
        private StringBuilder huntHelp;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InputForm{T}" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public HuntingPrompt(IWindow window) : base(window)
        {
            huntHelp = new StringBuilder();
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The dialog prompt text.<see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            // Clear out previous hunting help messages.
            huntHelp.Clear();

            // Create the prompt for explaining how hunting works.
            huntHelp.AppendLine($"{Environment.NewLine}Hunting Instructions{Environment.NewLine}");

            // Explain how timer works, how killing works and animal weight limits.
            const string huntTextTop =
                "Hunting has a timer which represents the total daylight remaining. When the timer reaches zero the hunt is over. " +
                "You can only take 100 pounds of food back to the wagon, don't kill more than you keep since you just waste bullets.";

            // Explain how shooting works, how player has limited window of opportunity to shoot the animal.
            const string huntTextBottom =
                "When animal appears you have until it disappears to type the shooting word shown. " +
                "If you don't type fast enough you risk missing your shot and bullet on nothing!";

            // Add the top and bottom hunting text on their own lines.
            huntHelp.AppendLine(huntTextTop.WordWrap());
            huntHelp.AppendLine(huntTextBottom.WordWrap());

            // Returns the now processed hunting help prompt to renderer.
            return huntHelp.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Creates a new hunt with animals for the player to kill.
            UserData.GenerateHunt();

            // Attaches the form that lets us manipulate and view this data.
            SetForm(typeof (Hunting));
        }
    }
}