// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Collections.Generic;
using OregonTrailDotNet.Persistence;

namespace OregonTrailDotNet.Module.Tombstone
{
    /// <summary>
    ///     Keeps track of the trail's tombstones and supports saving them to the game database and loading them again. The
    ///     trail only ever holds two graves — one for each half of the trail — keyed by <see cref="Tombstone.TrailHalf" />, so
    ///     a party that dies in a half simply overwrites whatever grave was already there. Also has the methods to check for a
    ///     Tombstone at a particular spot on the trail.
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

            foreach (var grave in _store.All())
                Tombstones[grave.TrailHalf] = new Tombstone(grave.TrailHalf, grave.MileMarker, grave.PlayerName,
                    grave.Epitaph, grave.LastLandmark, grave.NextLandmark, grave.MilesToNext);
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
        ///     References the currently loaded tombstones, keyed by which half of the trail they occupy (0 or 1) so there is
        ///     never more than one grave per half.
        /// </summary>
        private Dictionary<int, Tombstone> Tombstones { get; }

        /// <summary>
        ///     Creates a shallow copy of the tombstone item and places it in the grave slot for its half of the trail. The
        ///     trail holds only two graves — one per half — so a fresh death in a half overwrites whatever grave was there.
        /// </summary>
        /// <param name="tombstoneItem">The tombstone Item.</param>
        public void Add(Tombstone tombstoneItem)
        {
            // Clone the tombstone.
            var tombstoneClone = tombstoneItem.Clone() as Tombstone;

            // Skip if the cloning fails.
            if (tombstoneClone == null)
                return;

            // Place the grave in the slot for its half of the trail, replacing any earlier grave in that same half.
            Tombstones[tombstoneClone.TrailHalf] = tombstoneClone;

            // Persist it (a no-op when persistence is off); INSERT OR REPLACE mirrors the overwrite-per-half rule above.
            _store?.Insert(tombstoneClone.TrailHalf, tombstoneClone.MileMarker, tombstoneClone.PlayerName,
                tombstoneClone.Epitaph, tombstoneClone.LastLandmark, tombstoneClone.NextLandmark,
                tombstoneClone.MilesToNextLandmark);
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

        /// <summary>Looks up a grave sitting at the given mile marker on the trail (the spot where that party actually died).</summary>
        /// <param name="odometer">Number of miles the vehicle has traveled (to check for grave-site).</param>
        /// <param name="foundTombstone">Tombstone item returned from this mile marker if one exists, NULL if no tombstone found.</param>
        public void FindTombstone(int odometer, out Tombstone foundTombstone)
        {
            foreach (var grave in Tombstones.Values)
            {
                if (grave.MileMarker != odometer)
                    continue;

                foundTombstone = grave;
                return;
            }

            foundTombstone = null;
        }

        /// <summary>Checks if there is a tombstone at the given mile marker, does not return it but only indicates if one exists.</summary>
        /// <param name="odemeter">Current mile marker the vehicle is located at on the trail.</param>
        /// <returns>TRUE if tombstone exists for this mile marker.</returns>
        public bool ContainsTombstone(int odemeter)
        {
            foreach (var grave in Tombstones.Values)
                if (grave.MileMarker == odemeter)
                    return true;

            return false;
        }
    }
}