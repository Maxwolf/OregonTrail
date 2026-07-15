// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Collections.Generic;
using OregonTrailDotNet.Persistence;

namespace OregonTrailDotNet.Module.Tombstone
{
    /// <summary>
    ///     Keeps track of all the tombstones in a nice collection and also supports saving them to disk and loading them again
    ///     using JSON. Finally it also has all the needed methods to check for a Tombstone at a particular spot on the
    ///     trail.
    /// </summary>
    public sealed class TombstoneModule : WolfCurses.Module.Module
    {
        /// <summary>Optional persistence for tombstones; null keeps the module purely in-memory (bot/tests).</summary>
        private readonly TombstoneStore _store;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TombstoneModule" /> class, loading any previously saved tombstones
        ///     from the game database when persistence is enabled.
        /// </summary>
        /// <param name="store">Tombstone persistence, or null to run without saving (in-memory only).</param>
        public TombstoneModule(TombstoneStore store = null)
        {
            Tombstones = new Dictionary<int, Tombstone>();
            _store = store;

            if (_store == null)
                return;

            foreach (var (mileMarker, playerName, epitaph) in _store.All())
                Tombstones[mileMarker] = new Tombstone(playerName, mileMarker, epitaph);
        }

        /// <summary>Gets or sets the element with the specified key.</summary>
        /// <returns>The element with the specified key.</returns>
        /// <param name="key">The key of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The property is set and the<see cref="T:System.Collections.IDictionary" /> object is read-only.-or- The property is
        ///     set,<paramref name="key" /> does not exist in the collection, and the
        ///     <see cref="T:System.Collections.IDictionary" />
        ///     has a fixed size.
        /// </exception>
        public Tombstone this[int key]
        {
            get => Tombstones[key];
            set => Tombstones[key] = value;
        }

        /// <summary>
        ///     References all of the currently loaded tombstones.
        /// </summary>
        private Dictionary<int, Tombstone> Tombstones { get; }

        /// <summary>
        ///     Creates a shallow copy of the tombstone item and adds it to the list of tombstones. Does not check if it already
        ///     exists. Only safety is that multiple tombstones cannot be placed at the same mile marker.
        /// </summary>
        /// <param name="tombstoneItem">The tombstone Item.</param>
        public void Add(Tombstone tombstoneItem)
        {
            // Clone the tombstone.
            var tombstoneClone = tombstoneItem.Clone() as Tombstone;

            // Skip if the cloning fails.
            if (tombstoneClone == null)
                return;

            // Check if we already have a tombstone at this mile marker.
            if (Tombstones.ContainsKey(tombstoneClone.MileMarker))
                return;

            // Actually adds the tombstone to the internal list of them using mile marker as a key.
            Tombstones.Add(tombstoneItem.MileMarker, tombstoneClone);

            // Persist it (a no-op when persistence is off); INSERT OR IGNORE mirrors the one-per-mile-marker rule above.
            _store?.Insert(tombstoneClone.MileMarker, tombstoneClone.PlayerName, tombstoneClone.Epitaph);
        }

        /// <summary>
        ///     Clears any existing tombstone data that might be loaded in the module. When persistence is on this also clears the
        ///     saved tombstones from the game database.
        /// </summary>
        public void Reset()
        {
            Tombstones.Clear();
            _store?.Clear();
        }

        /// <summary>Delegating method for the internal tombstones dictionary, used to make grabbing values less verbose.</summary>
        /// <param name="odometer">Number of miles the vehicle has traveled (to check for grave-site).</param>
        /// <param name="foundTombstone">Tombstone item returned from this mile marker if one exists, NULL if no tombstone found.</param>
        public void FindTombstone(int odometer, out Tombstone foundTombstone)
        {
            Tombstones.TryGetValue(odometer, out foundTombstone);
        }

        /// <summary>Checks if there is a tombstone at the given mile marker, does not return it but only indicates if one exists.</summary>
        /// <param name="odemeter">Current mile marker the vehicle is located at on the trail.</param>
        /// <returns>TRUE if tombstone exists for this mile marker.</returns>
        public bool ContainsTombstone(int odemeter)
        {
            return Tombstones.ContainsKey(odemeter);
        }
    }
}