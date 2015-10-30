using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Requires type parameter that is a reference type with a constructor.
    /// </summary>
    public abstract class ModeState<T> : IModeState where T : class, new()
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        protected ModeState(IMode gameMode, T userData)
        {
            ParentMode = gameMode;
            UserData = userData;
        }

        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game mode.
        /// </summary>
        protected T UserData { get; }

        /// <summary>
        ///     Current parent game mode which this state is binded to and is doing work on behalf of.
        /// </summary>
        protected IMode ParentMode { get; }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public virtual bool AcceptsInput
        {
            get { return !ParentMode.ShouldRemoveMode; }
        }

        /// <summary>
        ///     Forces the current game mode state to update itself, this typically results in moving to the next state.
        /// </summary>
        public virtual void TickState()
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public abstract string GetStateTUI();

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public abstract void OnInputBufferReturned(string input);

        /// <summary>
        ///     Fired when the active game mode has been changed in parent game mode, this is intended for game mode states only so
        ///     they can be aware of these changes and act on them if needed.
        /// </summary>
        public virtual void OnParentModeChanged()
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return GetType().Name;
        }
    }
}