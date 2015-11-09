namespace TrailEntities.Mode
{
    /// <summary>
    ///     Defines a game mode state that can be created at runtime using reflection using a factory pattern.
    /// </summary>
    public abstract class StateProduct : IStateProduct<ModeProduct, IModeInfo>
    {
        /// <summary>
        ///     Creates a new game mode state, typically this is called from factory pattern.
        /// </summary>
        /// <param name="parentMode">Parent game mode that spawned this state.</param>
        /// <param name="userData">Custom user data object that is carried between states and the game mode.</param>
        protected StateProduct(IModeProduct parentMode, IModeInfo userData)
        {
            // Set the game most that is currently hosting this state.
            ParentMode = parentMode as ModeProduct;
            UserData = userData;
        }

        /// <summary>
        ///     Current parent game mode which this state is binded to and is doing work on behalf of.
        /// </summary>
        public ModeProduct ParentMode { get; }

        /// <summary>
        ///     User data objects for game modes and any attached states on them.
        /// </summary>
        public IModeInfo UserData { get; }

        /// <summary>
        ///     Name of the state, the base product will return the type name.
        /// </summary>
        public string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public virtual bool AcceptsInput
        {
            get { return false; }
        }

        /// <summary>
        ///     Forces the current game mode state to update itself, this typically results in moving to the next state.
        /// </summary>
        public virtual void TickState()
        {
            // Move along, nothing to see here...
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public abstract string OnRenderState();

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public abstract void OnInputBufferReturned(string input);
    }
}