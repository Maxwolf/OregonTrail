namespace TrailCommon
{
    /// <summary>
    ///     Underlying game mode interface, used by base simulation to keep track of what data should currently have control
    ///     over the simulation details. Only top most game mode will ever be ticked.
    /// </summary>
    public interface IMode
    {
        /// <summary>
        ///     Defines the type of game mode this is and what it's purpose will be intended for.
        /// </summary>
        ModeType Mode { get; }

        /// <summary>
        ///     Grabs the text user interface string that will be used for debugging on console application.
        /// </summary>
        /// <returns></returns>
        string GetTUI();

        /// <summary>
        ///     Ticks the internal logic of the game mode so that it may perform linear operations.
        /// </summary>
        void TickMode();

        /// <summary>
        ///     Intended to be used by base simulation to pass along the input buffer after the user has typed several characters
        ///     into the input buffer. This is used when allowing the user to input custom strings like names for their party
        ///     members.
        /// </summary>
        void SendCommand(string returnedLine);
    }
}