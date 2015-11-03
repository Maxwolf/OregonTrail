using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Shows information about what the player leader professions mean and how it affects the party, vehicle, game
    ///     difficulty, and scoring at the end (if they make it).
    /// </summary>
    public sealed class ProfessionAdviceState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     Determines if the player is done reading the profession advice.
        /// </summary>
        private bool _hasReadProfessionAdvice;

        /// <summary>
        ///     Holds reference to advice string that is built in constructor.
        /// </summary>
        private StringBuilder _professionAdvice;

        public ProfessionAdviceState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            // Information about professions and how they work.
            _professionAdvice = new StringBuilder();
            _professionAdvice.Append("Various occupations have advantages over one another:\n");
            _professionAdvice.Append("-------------------------------------------------------------------------------\n");
            _professionAdvice.Append("OCCUPATION   | CASH  |  ADVANTAGES                                |FINAL BONUS|\n");
            _professionAdvice.Append("-------------------------------------------------------------------------------\n");
            _professionAdvice.Append("Banker       |$1,600 | none                                       | x1        |\n");
            _professionAdvice.Append("Carpenter    |$800   | more likely to repair broken wagon parts.  | x2        |\n");
            _professionAdvice.Append("Farmer       |$400   | oxen are less likely to get sick and die.  | x3        |\n");
            _professionAdvice.Append("-------------------------------------------------------------------------------\n");
            _professionAdvice.Append("Cash = how much cash a person of that occupation begins with.\n");
            _professionAdvice.Append("Advantages = special individual attributes of the occupation.\n");
            _professionAdvice.Append("Final Bonus = amount that your final point total will be multiplied by.\n\n");

            _professionAdvice.Append("Press ENTER KEY to return to profession selection.\n");
        }

        public override bool AcceptsInput
        {
            get { return false; }
        }

        public override string GetStateTUI()
        {
            return _professionAdvice.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            if (_hasReadProfessionAdvice)
                return;

            // Return the select profession state if we are ready for that.
            _hasReadProfessionAdvice = true;
            ParentMode.CurrentState = new SelectProfessionState(ParentMode, UserData);
        }
    }
}