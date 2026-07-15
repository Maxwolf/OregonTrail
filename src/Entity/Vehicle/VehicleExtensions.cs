// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Collections.Generic;
using OregonTrailDotNet.Entity.Person;

namespace OregonTrailDotNet.Entity.Vehicle
{
    /// <summary>
    ///     Utility methods used by vehicle entity to make working with passengers and inventory easier on the eyes when used
    ///     one after the other. Typically these methods will be used by random events triggered by the game simulation.
    /// </summary>
    public static class VehicleExtensions
    {
        /// <summary>
        ///     Loops through the living passengers and rolls, per person, whether a catastrophic event kills them. The chance is
        ///     no longer a flat coin-flip: it starts from <paramref name="baseChancePercent" /> for a hale traveller and climbs
        ///     (toward roughly double) as that person's health falls, so a well-provisioned, healthy party survives a disaster far
        ///     better than a starving, sick one. This is what makes keeping the party fed, rested, clothed and treated pay off
        ///     against events instead of every member facing the same 50% execution regardless of condition.
        /// </summary>
        /// <param name="passengers">List of passengers from the vehicle.</param>
        /// <param name="baseChancePercent">Chance (0-100) that the event kills a full-health member; scaled up for the unwell.</param>
        /// <param name="cause">What to record as the cause of death for anyone this kills.</param>
        /// <returns>List of people the method killed, empty list means nobody was killed.</returns>
        public static IEnumerable<Person.Person> TryKill(this IEnumerable<Person.Person> passengers,
            int baseChancePercent, CauseOfDeath cause)
        {
            var game = GameSimulationApp.Instance;

            // Determine if we lost any people, this is separate from items in vehicle.
            var peopleKilled = new List<Person.Person>();
            foreach (var person in passengers)
            {
                if (person.HealthStatus == HealthStatus.Dead)
                    continue;

                // healthFactor is 1.0 at full (Good) health and rises toward ~2.0 as health approaches death, so the frail are
                // markedly more likely to be taken than the hale.
                var healthFactor = 1.0 +
                                   (double) ((int) HealthStatus.Good - (int) person.HealthStatus) / (int) HealthStatus.Good;

                if (game.Random.Next(100) < baseChancePercent * healthFactor)
                {
                    person.Kill(cause);
                    peopleKilled.Add(person);
                }
            }

            // Gives back the list of people that were killed by this extension method.
            return peopleKilled;
        }

        /// <summary>
        ///     Damages all of the players using the total amount given as a guideline for total amount of damage to be spread out
        ///     amongst the living players. It will be divided by the number of living and spread that way.
        /// </summary>
        /// <param name="passengers">List of passengers from the vehicle.</param>
        /// <param name="amount">Amount of health we should remove from the living passengers.</param>
        // ReSharper disable once UnusedMember.Global
        public static void Damage(this IList<Person.Person> passengers, int amount)
        {
            // Check if there are people to damage.
            if (passengers.Count <= 0)
                return;

            // Check if the amount is greater than zero.
            if (amount <= 0)
                return;

            // Check if the amount is greater than passenger count.
            if (amount <= passengers.Count)
                return;

            // Figure out how much damage needs to be applied to each person.
            var damagePerPerson = amount/passengers.Count;

            // Loop through all the passengers and damage them according to calculated amount.
            foreach (var person in passengers)
            {
                // Skip if the person is already dead.
                if (person.HealthStatus == HealthStatus.Dead)
                    continue;

                // Apply damage to the person we calculated above.
                person.Damage(damagePerPerson);
            }
        }
    }
}