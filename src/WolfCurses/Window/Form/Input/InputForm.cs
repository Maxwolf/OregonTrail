// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

using System;
using System.Text;
using OregonTrailDotNet.WolfCurses.Core;

namespace OregonTrailDotNet.WolfCurses.Window.Form.Input
{
    /// <summary>
    ///     Represents a dialog box that acts like a pop-up where it displays some piece of data, accepts any key for input and
    ///     then closes.
    /// </summary>
    /// <typeparam name="T">Windows information object that will be applied to this state on when constructor is called.</typeparam>
    public abstract class InputForm<T> : Form<T> where T : WindowData, new()
    {
        /// <summary>
        ///     Reference for all the text we will display for user to read when the state is activated.
        /// </summary>
        private StringBuilder _prompt;

        /// <summary>
        ///     Keeps track if the player is done looking at this dialog prompt.
        /// </summary>
        private bool _seenPrompt;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InputForm{T}" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        protected InputForm(IWindow window) : base(window)
        {
            _prompt = new StringBuilder();
        }

        /// <summary>
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected virtual DialogType DialogType
        {
            get { return DialogType.Prompt; }
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            get
            {
                switch (DialogType)
                {
                    case DialogType.Prompt:
                        return false;
                    case DialogType.YesNo:
                    case DialogType.Custom:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Build up the dialog prompt using abstract methods to get text to show user.
            _prompt.Append(OnDialogPrompt());

            // Wait for user input by asking them to press ANY key.
            switch (DialogType)
            {
                case DialogType.Prompt:
                    _prompt.Append((string) InputManager.PRESSENTER);
                    break;
                case DialogType.YesNo:
                case DialogType.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The dialog prompt text.<see cref="string" />.
        /// </returns>
        protected abstract string OnDialogPrompt();

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string OnRenderForm()
        {
            return _prompt.ToString();
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Only process input for dialog once.
            if (_seenPrompt)
                return;

            // Process input and return dialog result.
            _seenPrompt = true;
            switch (input.ToUpperInvariant())
            {
                case "Y":
                case "YES":
                case "TRUE":
                    OnDialogResponse(DialogResponse.Yes);
                    return;
                case "N":
                case "NO":
                case "FALSE":
                    OnDialogResponse(DialogResponse.No);
                    return;
                default:
                    OnDialogResponse(DialogResponse.Custom);
                    return;
            }
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected abstract void OnDialogResponse(DialogResponse reponse);
    }
}