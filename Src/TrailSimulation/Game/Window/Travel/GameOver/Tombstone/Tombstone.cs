using System;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Facilitates a tombstone base class that supports shallow copies of itself to be created.
    /// </summary>
    public sealed class Tombstone : ICloneable
    {
        /// <summary>
        ///     Message that can be included on the Tombstone below the players name. It can only be a few characters long but
        ///     interesting to see what people leave as warnings or just silliness for other travelers.
        /// </summary>
        private string _epitaph;

        /// <summary>
        ///     Defines where on the trail in regards to length in miles traveled. The purpose of this is so we can check for this
        ///     Tombstone in the exact same spot where the player actually died on the trail.
        /// </summary>
        private int _mileMarker;

        /// <summary>
        ///     Name of the player whom died and the Tombstone is paying respects to.
        /// </summary>
        private string _playerName;

        /// <summary>
        ///     Creates a Tombstone from scratch, useful for when re-creating them from disk to be loaded back into the
        ///     simulation
        ///     for other players to find on the trail.
        /// </summary>
        /// <param name="playerName">Name of the player that died on the trail.</param>
        /// <param name="mileMarker">
        ///     Mile marker where the player died so we can trigger it at the right spot (assuming the trail
        ///     is long enough for that to happen if it is randomized).
        /// </param>
        /// <param name="epitaph">
        ///     Optional message the player can leave on the Tombstone to warn, tease, or taunt other players.
        ///     Cool part is not knowing what it will say!
        /// </param>
        public Tombstone(string playerName, int mileMarker, string epitaph)
        {
            Epitaph = epitaph;
            MileMarker = mileMarker;
            PlayerName = playerName;

            // Note: This will be able to produce approximately 5,316,911,983,139,663,491,615,228,241,121,400,000 unique values.
            TombstoneID = Guid.NewGuid().ToString("N");

            // New tombstones can be edited until they are shallow copied.
            Locked = false;
        }

        /// <summary>
        ///     Creates a nice formatted version of the Tombstone for use in text renderer.
        /// </summary>
        public override string ToString()
        {
            return $"Here lies {PlayerName}" +
                   $"{Environment.NewLine}{Epitaph}";
        }

        /// <summary>
        ///     Creates a shallow copy of the tombstone, generates a new tombstone ID in the process.
        /// </summary>
        public Tombstone()
        {
            // Loop through all the vehicle passengers and find the leader.
            foreach (var passenger in GameSimulationApp.Instance.Vehicle.Passengers)
            {
                // Skip if not the leader.
                if (!passenger.IsLeader)
                    continue;

                // Add the leaders name to the Tombstone header.
                PlayerName = passenger.Name;
                break;
            }

            // Grabs the current mile marker where the player died on the trail for the Tombstone to sit at.
            MileMarker = GameSimulationApp.Instance.Vehicle.Odometer;

            // Epitaph is left empty by default and ready to be filled in.
            Epitaph = string.Empty;

            // Create a new ID, even though it is clone of existing tombstone it is a different instance.
            TombstoneID = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        ///     Name of the player whom died and the Tombstone is paying respects to.
        /// </summary>
        public string PlayerName
        {
            get { return _playerName; }
            private set
            {
                if (Locked)
                    return;

                _playerName = value;
            }
        }

        /// <summary>
        ///     Determines if the tombstone data should be locked in and 'set in stone'. This is typically done when the tombstone
        ///     is copied, this is done typically when it is added to a list. Both of these assumptions together make for a tightly
        ///     closed loop of functionality that will do exactly what I want.
        /// </summary>
        private bool Locked { get; set; }

        /// <summary>
        ///     Determines the unique and randomly generated tombstone identification string. This is created once for every
        ///     tombstone and should be unique to each one, used to check if the tombstone is the same as the one for player who
        ///     recently died so we know if they should be offered a chance to edit it.
        /// </summary>
        private string TombstoneID { get; }

        /// <summary>
        ///     Defines where on the trail in regards to length in miles traveled. The purpose of this is so we can check for this
        ///     Tombstone in the exact same spot where the player actually died on the trail.
        /// </summary>
        public int MileMarker
        {
            get { return _mileMarker; }
            private set
            {
                if (Locked)
                    return;

                _mileMarker = value;
            }
        }

        /// <summary>
        ///     Message that can be included on the Tombstone below the players name. It can only be a few characters long but
        ///     interesting to see what people leave as warnings or just silliness for other travelers.
        /// </summary>
        public string Epitaph
        {
            get { return _epitaph; }
            set
            {
                if (Locked)
                    return;

                _epitaph = value;
            }
        }

        /// <summary>
        ///     Creates a shallow copy of our tombstone, used to add to list without having direct copy still tied to it.
        /// </summary>
        public object Clone()
        {
            var clone = (Tombstone) MemberwiseClone();

            // Cloned tombstones prevent further edits, for the clone and parent.
            Locked = true;
            clone.Locked = true;

            return clone;
        }
    }
}