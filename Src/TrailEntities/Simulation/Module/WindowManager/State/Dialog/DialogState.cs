using System;
using System.Text;

namespace TrailEntities.Simulation
{
    /// <summary>
    ///     Represents a dialog box that acts like a pop-up where it displays some piece of data, accepts any key for input and
    ///     then closes.
    /// </summary>
    /// <typeparam name="T">Mode information object that will be applied to this state on when constructor is called.</typeparam>
    public abstract class DialogState<T> : StateProduct<T> where T : ModeInfo, new()
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
        ///     This constructor will be used by the other one
        /// </summary>
        protected DialogState(IModeProduct gameMode) : base(gameMode)
        {
            _prompt = new StringBuilder();
            StateActivate();
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
        public override bool AcceptsInput
        {
            get
            {
                switch (DialogType)
                {
                    case DialogType.Prompt:
                        return false;
                    case DialogType.YesNo:
                    case DialogType.YesNoContinue:
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected abstract string OnDialogPrompt();

        /// <summary>
        ///     Fired when the constructor is called on the dialog state class. Collects string data that will be sent back to
        ///     simulation as text user interface (TUI).
        /// </summary>
        private void StateActivate()
        {
            // Build up the dialog prompt using abstract methods to get text to show user.
            _prompt.Append(OnDialogPrompt());

            // Wait for user input by asking them to press ANY key.
            switch (DialogType)
            {
                case DialogType.Prompt:
                    _prompt.Append(InputManagerMod.PRESS_ENTER);
                    break;
                case DialogType.YesNo:
                case DialogType.YesNoContinue:
                    _prompt.Append(InputManagerMod.PRESS_YESNO);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            return _prompt.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
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
                    OnDialogResponse(DialogResponse.Continue);
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