using System;
using System.Text;
using TrailEntities.Simulation;

namespace TrailEntities.Game
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

        public StoreAdviceState(IModeProduct gameMode, StoreInfo userData) : base(gameMode, userData)
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
            _storeHelp.Append($"{Environment.NewLine}Hello, I'm Matt. So you're going{Environment.NewLine}");
            _storeHelp.Append($"to Oregon! I can fix you up with{Environment.NewLine}");
            _storeHelp.Append($"what you need:{Environment.NewLine}{Environment.NewLine}");

            if (adviceCount <= 0)
            {
                _storeHelp.Append($" - a team of oxen to pull{Environment.NewLine}");
                _storeHelp.Append($" your vehicle{Environment.NewLine}{Environment.NewLine}");

                _storeHelp.Append($" - clothing for both{Environment.NewLine}");
                _storeHelp.Append($" summer and winter{Environment.NewLine}{Environment.NewLine}");
            }
            else if (adviceCount == 1)
            {
                _storeHelp.Append($" - plenty of food for the{Environment.NewLine}");
                _storeHelp.Append($" trip{Environment.NewLine}{Environment.NewLine}");

                _storeHelp.Append($" - ammunition for your{Environment.NewLine}");
                _storeHelp.Append($" rifles{Environment.NewLine}{Environment.NewLine}");

                _storeHelp.Append($" - spare parts for your{Environment.NewLine}");
                _storeHelp.Append($" wagon{Environment.NewLine}{Environment.NewLine}");
            }

            // Wait for user input...
            _storeHelp.Append(InputManager.PRESS_ENTER);
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
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

            //ParentMode.CurrentState = null;
            ClearState();
            _hasReadAdvice = true;
        }
    }
}