using System;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Facilitates the data keeping for tombstones so they can be separated from the actual rendering and logic systems of
    ///     the simulation. The purpose of which is for serialization so the data can be saved for this particular location
    ///     index so when players traveling to it in the future encounter it will trigger ability to view the TombstoneItem and
    ///     the
    ///     epitaph the player wrote on it if it exists.
    /// </summary>
    public sealed class TombstoneItem : TombstoneBase
    {
        /// <summary>
        ///     Creates a TombstoneItem from scratch, useful for when re-creating them from disk to be loaded back into the
        ///     simulation
        ///     for other players to find on the trail.
        /// </summary>
        /// <param name="playerName">Name of the player that died on the trail.</param>
        /// <param name="mileMarker">
        ///     Mile marker where the player died so we can trigger it at the right spot (assuming the trail
        ///     is long enough for that to happen if it is randomized).
        /// </param>
        /// <param name="epitaph">
        ///     Optional message the player can leave on the TombstoneItem to warn, tease, or taunt other players.
        ///     Cool part is not knowing what it will say!
        /// </param>
        public TombstoneItem(string playerName, int mileMarker, string epitaph) : base(playerName, mileMarker, epitaph)
        {
        }

        /// <summary>
        ///     Creates a new TombstoneItem using the current player, mileage, and leaves the epitaph blank and ready to be filled
        ///     in.
        /// </summary>
        public TombstoneItem()
        {
        }

        /// <summary>
        ///     Creates a nice formatted version of the TombstoneItem for use in text renderer.
        /// </summary>
        public override string ToString()
        {
            return $"Here lies{Environment.NewLine}" +
                   $"{PlayerName}{Environment.NewLine}" +
                   $"{Epitaph}";
        }
    }
}