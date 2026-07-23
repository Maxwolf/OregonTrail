// Created by Maxwolf (bigmaxwolf.com)

using System;
using System.Collections.Generic;

namespace OregonTrailDotNet.Module.Tombstone
{
    /// <summary>
    ///     A pool of short, silly epitaphs used as the default message on a fresh grave, in the spirit of the goofy things
    ///     real players (and bots) scrawled on their tombstones in the original game — "pepperoni and cheese", "welp", "HECK",
    ///     and friends. A human can always overwrite the default with their own message in the epitaph editor. Every entry is
    ///     kept short so it fits the tombstone display and never blows past the epitaph length limit.
    /// </summary>
    public static class EpitaphCatalog
    {
        private static readonly IReadOnlyList<string> Epitaphs = new[]
        {
            "pepperoni and cheese",
            "welp",
            "HECK",
            "oops",
            "R.I.P.",
            "gone but not forgotten",
            "should have bought more oxen",
            "do not eat the berries",
            "the river won",
            "why did i ford that",
            "tell my wagon i love her",
            "we ran out of snacks",
            "this trail is rigged",
            "at least i tried",
            "not like this",
            "brb dying",
            "big mad",
            "yeehaw no more",
            "blame the weather",
            "well that happened"
        };

        /// <summary>All of the silly epitaphs in the pool, exposed mostly so tests can assert invariants over them.</summary>
        public static IReadOnlyList<string> All => Epitaphs;

        /// <summary>
        ///     Picks a random silly epitaph from the pool. Uses <see cref="Random.Shared" /> so it is safe to call from any
        ///     thread (bot training may evaluate several games at once) without seeding anything ourselves.
        /// </summary>
        public static string Random()
        {
            return Epitaphs[System.Random.Shared.Next(Epitaphs.Count)];
        }
    }
}
