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

        public StoreAdviceState(IMode gameMode, StoreInfo userData) : base(gameMode, userData)
        {
            _hasReadAdvice = false;
            _storeHelp = new StringBuilder();
            _storeHelp.Append("Matt the owner of the store offers you some free advice:\n");
            _storeHelp.Append("'I recommend at least six oxen to pull your wagon. And you'll need plenty of\n");
            _storeHelp.Append("flour, sugar, bacon, coffee, and other types of food. I suggest you start out\n");
            _storeHelp.Append("with at least 200 pounds for each person in your party'\n\n");

            _storeHelp.Append("'You'll need good, warm clothing, especially for the mountains. I recommend\n");
            _storeHelp.Append("taking at least 2 sets of clothing per person. You'll need ammunition, too.\n");
            _storeHelp.Append("Each box of ammunition contains 20 bullets.'\n\n");

            _storeHelp.Append("'Finally, you might want to take along some spare wagon parts.  Wagon Wheels,\n");
            _storeHelp.Append("axles, and tongues are liable to break along the way. If you're unable to\n");
            _storeHelp.Append("repair a broken wagon, you'll be in big trouble!'\n\n");

            _storeHelp.Append("Press ENTER KEY to return to store.\n");
        }

        public override bool AcceptsInput
        {
            get { return false; }
        }

        public override string GetStateTUI()
        {
            return _storeHelp.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            if (_hasReadAdvice)
                return;

            ParentMode.CurrentState = null;
            _hasReadAdvice = true;
        }
    }
}