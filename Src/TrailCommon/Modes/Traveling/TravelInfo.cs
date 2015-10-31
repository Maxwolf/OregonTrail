namespace TrailCommon
{
    /// <summary>
    ///     Holds all the information about traveling that we want to know, such as how long we need to go until next point,
    ///     what our current mode is like moving, paused, etc.
    /// </summary>
    public sealed class TravelInfo : ModeInfo
    {
        protected override string Name
        {
            get { return "Travel Information"; }
        }

        /// <summary>
        ///     Determines if the player has looked around at the location before prompting them with any decision making.
        /// </summary>
        public bool HasLookedAround { get; set; }

        /// <summary>
        ///     Determines if a question will be asked regarding if the player wants to stop traveling and goto the point of
        ///     interest and check it out.
        /// </summary>
        public bool ForceLookAround { get; set; }
    }
}