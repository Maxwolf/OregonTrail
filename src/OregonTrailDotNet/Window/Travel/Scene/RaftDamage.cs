using System;
using System.Collections.Generic;
using System.Linq;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The raft's loss routines, restored around <see cref="Presentation.RaftGame" />'s hit counters exactly as
    ///     <c>FLOAT</c> calls them (<c>:700/:710/:950</c> into <c>RIVER.LIB</c>'s <c>:50175/:50185/:50205</c> — the
    ///     same code a bad ford runs, which is why a rock and a bad crossing read the same way). Each collision runs
    ///     one application: drown rolls per party member (the leader is spared while anybody else stands —
    ///     <c>FOR L = (NP&gt;1) TO NP-1</c> — and alone, can drown), each ox rolls, each supply category can lose one
    ///     to all of itself; the loss-line counter <c>Z</c> is zeroed <b>per event</b>, and a single catastrophic hit
    ///     with more than nine loss lines destroys the raft outright.
    /// </summary>
    internal static class RaftDamage
    {
        /// <summary>What one collision's damage application amounted to.</summary>
        /// <param name="Lines">The loss lines, in the order the original would print them.</param>
        /// <param name="Destroyed">More than nine lines: the raft is gone and the party with it.</param>
        internal sealed record Report(List<string> Lines, bool Destroyed);

        /// <summary>The supply categories the river can take, in the original's own order.</summary>
        private static readonly (EntitiesEnum Entity, string Name)[] SupplyCategories =
        [
            (EntitiesEnum.Clothes, "sets of clothing"),
            (EntitiesEnum.Ammo, "bullets"),
            (EntitiesEnum.Wheel, "wagon wheels"),
            (EntitiesEnum.Axle, "wagon axles"),
            (EntitiesEnum.Tongue, "wagon tongues"),
            (EntitiesEnum.Food, "pounds of food")
        ];

        /// <summary>A scrape along the bank: drown 0.15, oxen 0.30, supplies 0.50.</summary>
        internal static Report ShoreHit(Random random) => Apply(random, 0.15, 0.30, 0.50);

        /// <summary>A rock strike: drown 0.60, oxen 0.60, supplies 0.70 — four times the bank's menace.</summary>
        internal static Report RockHit(Random random) => Apply(random, 0.60, 0.60, 0.70);

        /// <summary>Missing the landing is survivable: supplies only at 0.50, and the raft lands anyway.</summary>
        internal static Report MissedLanding(Random random) => Apply(random, 0, 0, 0.50);

        private static Report Apply(Random random, double drown, double oxen, double supplies)
        {
            var vehicle = GameSimulationApp.Instance.Vehicle;
            var lines = new List<string>();

            // FLOAT 700/950: Z = 0 at the head of every application — the counter is per event, never per run,
            // so ordinary scrapes across a whole river can never add up to a destroyed raft.
            var z = 0;

            // Drown (:50175): the leader is index zero and the loop starts past them whenever anyone else is
            // alive; a leader alone takes the roll. V*(Z<11) zeroes the odds once eleven lines accumulate.
            var living = vehicle.Passengers.Where(passenger => passenger.HealthStatus != HealthStatusEnum.Dead)
                .ToList();
            foreach (var passenger in living)
            {
                if (passenger.Leader && living.Count > 1)
                    continue;

                if (z >= 11 || random.NextDouble() >= drown)
                    continue;

                passenger.Kill(CauseOfDeathEnum.Drowned);
                lines.Add($"{passenger.Name} has drowned.");
                z++;
            }

            // Oxen (:50185): each ox rolls independently.
            var oxenItem = vehicle.Inventory[EntitiesEnum.Animal];
            var oxenLost = 0;
            for (var ox = 0; ox < oxenItem.Quantity; ox++)
                if (random.NextDouble() < oxen)
                    oxenLost++;

            if (oxenLost > 0)
            {
                oxenItem.ReduceQuantity(oxenLost);
                lines.Add(oxenLost == 1 ? "You lose an ox." : $"You lose {oxenLost} oxen.");
                z += oxenLost;
            }

            // Supplies (:50205): each category that has anything rolls once, and a loss takes one to all of it.
            foreach (var (entity, name) in SupplyCategories)
            {
                var item = vehicle.Inventory[entity];
                if (item.Quantity <= 0 || random.NextDouble() >= supplies)
                    continue;

                var taken = random.Next(item.Quantity) + 1;
                item.ReduceQuantity(taken);
                lines.Add($"You lose {taken} {name}.");
                z++;
            }

            // :730: more than nine loss lines from ONE event and the river has won outright.
            if (z <= 9)
                return new Report(lines, false);

            foreach (var (entity, _) in SupplyCategories)
                vehicle.Inventory[entity].ReduceQuantity(int.MaxValue);
            vehicle.Inventory[EntitiesEnum.Animal].ReduceQuantity(int.MaxValue);

            foreach (var passenger in vehicle.Passengers)
                if (passenger.HealthStatus != HealthStatusEnum.Dead)
                    passenger.Kill(CauseOfDeathEnum.Drowned);

            lines.Add("The raft is destroyed, everything has been lost.");
            return new Report(lines, true);
        }
    }
}
