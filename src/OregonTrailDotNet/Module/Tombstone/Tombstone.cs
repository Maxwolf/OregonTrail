// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;

namespace OregonTrailDotNet.Module.Tombstone
{
    /// <summary>
    ///     Facilitates a tombstone base class that supports shallow copies of itself to be created. Mirrors the data the
    ///     original game recorded in TOMBS.REC: the party leader's name, an epitaph, and where the last member died —
    ///     captured as the surrounding landmarks and the distance to the next one. Only two graves exist at a time (one per
    ///     half of the trail); a fresh death in a half overwrites the grave already there.
    /// </summary>
    public sealed class Tombstone
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Tombstone" /> class.
        ///     Creates a shallow copy of the tombstone, generates a new tombstone ID in the process.
        /// </summary>
        public Tombstone()
        {
            // Loop through all the vehicle passengers and find the leader.
            foreach (var passenger in GameSimulationApp.Instance.Vehicle.Passengers)
            {
                // Skip if not the leader.
                if (!passenger.Leader)
                    continue;

                // Add the leaders name to the Tombstone header.
                PlayerName = passenger.Name;
                break;
            }

            // Grabs the current mile marker where the player died on the trail for the Tombstone to sit at.
            MileMarker = GameSimulationApp.Instance.Vehicle.Odometer;

            // Record the landmarks that bracket the death, exactly as the original TOMBS.REC did, so the grave remembers
            // the location of the death rather than just its raw mileage.
            var trail = GameSimulationApp.Instance.Trail;
            LastLandmark = trail.CurrentLocation?.Name ?? string.Empty;
            NextLandmark = trail.NextLocation?.Name ?? string.Empty;
            MilesToNextLandmark = trail.DistanceToNextLocation;

            // Which half of the trail the death happened in decides which of the two graves this one occupies.
            TrailHalf = CalculateTrailHalf(MileMarker, trail.Length);

            // Start the grave off with a random silly epitaph, like the goofy messages players (and bots) left in the
            // original game. The player can replace it with their own words in the epitaph editor if they want to.
            Epitaph = EpitaphCatalog.Random();
        }

        /// <summary>
        ///     Initializes a tombstone directly from stored values, used when hydrating saved tombstones from the game database
        ///     (as opposed to the parameterless constructor, which reads the live vehicle for a fresh death on the trail).
        /// </summary>
        public Tombstone(int trailHalf, int mileMarker, string playerName, string epitaph, string lastLandmark,
            string nextLandmark, int milesToNextLandmark)
        {
            TrailHalf = trailHalf;
            MileMarker = mileMarker;
            PlayerName = playerName;
            Epitaph = epitaph ?? string.Empty;
            LastLandmark = lastLandmark ?? string.Empty;
            NextLandmark = nextLandmark ?? string.Empty;
            MilesToNextLandmark = milesToNextLandmark;
        }

        /// <summary>
        ///     Determines which half of the trail a given mile marker falls in: 0 for the first half, 1 for the second. A
        ///     zero-or-unknown trail length keeps everything in the first half so nothing divides by zero.
        /// </summary>
        internal static int CalculateTrailHalf(int mileMarker, int trailLength)
        {
            return trailLength > 0 && mileMarker >= trailLength / 2 ? 1 : 0;
        }

        /// <summary>
        ///     Which half of the trail this grave sits in — 0 for the first half, 1 for the second. The trail only ever holds
        ///     one grave per half, so this doubles as the grave's slot: a new death in the same half overwrites it.
        /// </summary>
        public int TrailHalf { get; }

        /// <summary>
        ///     Name of the player whom died and the Tombstone is paying respects to.
        /// </summary>
        public string PlayerName { get; }

        /// <summary>
        ///     Name of the last landmark the party reached before the leader died, matching the original tombstone format's
        ///     record of where the death occurred.
        /// </summary>
        public string LastLandmark { get; }

        /// <summary>
        ///     Name of the landmark the party was traveling toward when the leader died.
        /// </summary>
        public string NextLandmark { get; }

        /// <summary>
        ///     Miles that still remained to <see cref="NextLandmark" /> at the moment of death; together with
        ///     <see cref="LastLandmark" /> this pins the exact spot the party perished, as the original file format did.
        /// </summary>
        public int MilesToNextLandmark { get; }

        /// <summary>
        ///     Defines where on the trail in regards to length in miles traveled. The purpose of this is so we can check for this
        ///     Tombstone in the exact same spot where the player actually died on the trail.
        /// </summary>
        public int MileMarker { get; }

        /// <summary>
        ///     Message that can be included on the Tombstone below the players name. It can only be a few characters long but
        ///     interesting to see what people leave as warnings or just silliness for other travelers.
        /// </summary>
        public string Epitaph { get; set; }

        /// <summary>
        ///     Creates a shallow copy of our tombstone, used to add to list without having direct copy still tied to it.
        /// </summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object Clone()
        {
            var clone = (Tombstone) MemberwiseClone();
            return clone;
        }

        /// <summary>
        ///     Creates a nice formatted version of the Tombstone for use in text renderer.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            // Only print the name if the epitaph is empty or null.
            if (string.IsNullOrEmpty(Epitaph))
                return $"Here lies {PlayerName}{Environment.NewLine}";

            // Print the name and epitaph message the player left for others to read.
            return $"Here lies {PlayerName}{Environment.NewLine}" +
                   $"{Epitaph}{Environment.NewLine}";
        }
    }
}