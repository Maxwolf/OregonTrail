namespace TrailEntities.Simulation.Mode
{
    /// <summary>
    ///     Represents a dialog box that acts like a pop-up where it displays some piece of data, accepts any key for input and
    ///     then closes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DialogState<T> : ModeState<T> where T : IModeInfo, new()
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        protected DialogState(IModeProduct gameMode, T userData) : base(gameMode, userData)
        {
        }
    }
}