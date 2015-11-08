namespace TrailEntities.RiverCross
{
    /// <summary>
    ///     Defines information about the current river crossing game mode the player has come across and needs to decide how
    ///     they would like to proceed.
    /// </summary>
    public sealed class RiverCrossInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TileEntities.RiverCrossInfo" /> class.
        /// </summary>
        public RiverCrossInfo()
        {
            // Randomly generates statistics about the river each time you cross it.
            Depth = GameSimApp.Instance.Random.Next(1, 20);
            FerryCost = GameSimApp.Instance.Random.Next(3, 8);
        }

        /// <summary>
        ///     Determines how deep the river is in feet.
        /// </summary>
        public int Depth { get; }

        /// <summary>
        ///     Determines how much the ferry operator will charge to cross the river.
        /// </summary>
        public int FerryCost { get; }
    }
}