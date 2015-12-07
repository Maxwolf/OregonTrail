using System;

namespace TrailSimulation.Game
{
    public abstract class TombstoneCloneable : ICloneable
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
        protected TombstoneCloneable(string playerName, int mileMarker, string epitaph)
        {
            Epitaph = epitaph;
            MileMarker = mileMarker;
            PlayerName = playerName;
        }

        protected TombstoneCloneable()
        {
            // Loop through all the vehicle passengers and find the leader.
            foreach (var passenger in GameSimulationApp.Instance.Vehicle.Passengers)
            {
                // Skip if not the leader.
                if (!passenger.IsLeader)
                    continue;

                // Add the leaders name to the TombstoneItem header.
                PlayerName = passenger.Name;
                break;
            }

            // Grabs the current mile marker where the player died on the trail for the TombstoneItem to sit at.
            MileMarker = GameSimulationApp.Instance.Vehicle.Odometer;

            // Epitaph is left empty by default and ready to be filled in.
            Epitaph = string.Empty;
        }

        /// <summary>
        ///     Name of the player whom died and the TombstoneItem is paying respects to.
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        ///     Defines where on the trail in regards to length in miles traveled. The purpose of this is so we can check for this
        ///     TombstoneItem in the exact same spot where the player actually died on the trail.
        /// </summary>
        public int MileMarker { get; set; }

        /// <summary>
        ///     Message that can be included on the TombstoneItem below the players name. It can only be a few characters long but
        ///     interesting to see what people leave as warnings or just silliness for other travelers.
        /// </summary>
        public string Epitaph { get; set; }

        /// <summary>
        ///     Creates a shallow copy of our tombstone, used to add to list without having direct copy still tied to it.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var clone = (TombstoneCloneable) MemberwiseClone();
            return clone;
        }
    }
}