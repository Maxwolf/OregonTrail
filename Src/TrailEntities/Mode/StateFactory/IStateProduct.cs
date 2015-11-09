namespace TrailEntities.Mode
{
    /// <summary>
    ///     Abstracted state so we can call standard operators minus the user data which is created in the concrete product
    ///     implementation.
    /// </summary>
    public interface IStateProduct<TParent, TData> where TParent : class where TData : class
    {
        /// <summary>
        ///     Current parent game mode which this state is binded to and is doing work on behalf of.
        /// </summary>
        TParent ParentMode { get; }

        /// <summary>
        ///     User data objects for game modes and any attached states on them.
        /// </summary>
        TData UserData { get; }

        /// <summary>
        ///     Name of the state, the base product will return the type name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        bool AcceptsInput { get; }

        /// <summary>
        ///     Forces the current game mode state to update itself, this typically results in moving to the next state.
        /// </summary>
        void TickState();

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        string OnRenderState();

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        void OnInputBufferReturned(string input);
    }
}