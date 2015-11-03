using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Offers up some free information about what items are important to the player and what they mean for the during the
    ///     course of the simulation.
    /// </summary>
    public sealed class StoreAdviceState : ModeState<StoreInfo>
    {
        /// <summary>
        ///     Keeps track if the player has read all the advice and this dialog needs to be closed.
        /// </summary>
        private bool _hasReadAdvice;

        /// <summary>
        ///     Keeps track of the message we want to show to the player but only build it actually once.
        /// </summary>
        private StringBuilder _storeHelp;

        /// <summary>
        ///     Determines which panel of information we have shown to the user, pressing return will cycle through them.
        /// </summary>
        private int adviceCount;

        public StoreAdviceState(IMode gameMode, StoreInfo userData) : base(gameMode, userData)
        {
            _hasReadAdvice = false;
            _storeHelp = new StringBuilder();
            UpdateAdvice();
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return false; }
        }

        /// <summary>
        ///     Since the advice can change we have to do this in chunks.
        /// </summary>
        private void UpdateAdvice()
        {
            // Clear any previous string builder message.
            _storeHelp.Clear();

            // Create the current state of our advice to player.
            _storeHelp.Append("\nHello, I'm Matt. So you're going\n");
            _storeHelp.Append("to Oregon! I can fix you up with\n");
            _storeHelp.Append("what you need:\n\n");

            if (adviceCount <= 0)
            {
                _storeHelp.Append(" - a team of oxen to pull\n");
                _storeHelp.Append(" your vehicle\n\n");

                _storeHelp.Append(" - clothing for both\n");
                _storeHelp.Append(" summer and winter\n\n");
            }
            else if (adviceCount == 1)
            {
                _storeHelp.Append(" - plenty of food for the\n");
                _storeHelp.Append(" trip\n\n");

                _storeHelp.Append(" - ammunition for your\n");
                _storeHelp.Append(" rifles\n\n");

                _storeHelp.Append(" - spare parts for your\n");
                _storeHelp.Append(" wagon\n\n");
            }

            // Wait for user input...
            _storeHelp.Append("Press ENTER KEY to continue.\n");
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            return _storeHelp.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            // Tick the advice to next panel when we get input.
            if (adviceCount <= 0)
            {
                adviceCount++;
                UpdateAdvice();
                return;
            }

            // Make sure we don't run final logic to show actual store until we show all advice to player.
            if (adviceCount < 1)
                return;

            // On the last advice panel we flip a normal boolean to know we are definitely done here.
            if (_hasReadAdvice)
                return;

            ParentMode.CurrentState = null;
            _hasReadAdvice = true;
        }
    }
}