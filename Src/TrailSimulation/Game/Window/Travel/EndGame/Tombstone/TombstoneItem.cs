namespace TrailSimulation.Game
{
    /// <summary>
    ///     Facilitates the data keeping for tombstones so they can be separated from the actual rendering and logic systems of
    ///     the simulation. The purpose of which is for serialization so the data can be saved for this particular location
    ///     index so when players traveling to it in the future encounter it will trigger ability to view the tombstone and the
    ///     epitaph the player wrote on it if it exists.
    /// </summary>
    public sealed class TombstoneItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Game.TombstoneItem" /> class.
        /// </summary>
        public TombstoneItem(string playerName, int mileMarker, string epitaph)
        {
            Epitaph = epitaph;
            MileMarker = mileMarker;
            PlayerName = playerName;
        }

        /// <summary>
        ///     Name of the player whom died and the tombstone is paying respects to.
        /// </summary>
        public string PlayerName { get; }

        /// <summary>
        ///     Defines where on the trail in regards to length in miles traveled. The purpose of this is so we can check for this
        ///     tombstone in the exact same spot where the player actually died on the trail.
        /// </summary>
        public int MileMarker { get; }

        /// <summary>
        ///     Message that can be included on the tombstone below the players name. It can only be a few characters long but
        ///     interesting to see what people leave as warnings or just silliness for other travelers.
        /// </summary>
        public string Epitaph { get; }
    }
}