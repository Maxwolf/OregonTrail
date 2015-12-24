// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/16/2015@11:35 AM

namespace TrailSimulation.Entity
{
    using System.Collections.Generic;
    using Game;

    /// <summary>
    ///     Utility methods used by vehicle entity to make working with passengers and inventory easier on the eyes when used
    ///     one after the other. Typically these methods will be used by random events triggered by the game simulation.
    /// </summary>
    public static class VehicleExtensions
    {
        /// <summary>
        ///     Loops through all the currently living passengers and rolls the dice to see if they should be killed. This method
        ///     is very powerful and can end the game quickly if used excessively.
        /// </summary>
        /// <param name="passengers">List of passengers from the vehicle.</param>
        /// <returns>List of people the method killed, empty list means nobody was killed.</returns>
        public static IEnumerable<Person> TryKill(this IEnumerable<Person> passengers)
        {
            // Determine if we lost any people, this is separate from items in vehicle.
            var peopleKilled = new List<Person>();
            foreach (var person in passengers)
            {
                // It all comes down to a dice roll if the storm kills you.
                if (!GameSimulationApp.Instance.Random.NextBool() ||
                    person.HealthValue == HealthLevel.Dead)
                    continue;

                // Kills the person and adds them to list.
                person.Kill();
                peopleKilled.Add(person);
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
        public static void Damage(this IList<Person> passengers, int amount)
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
                if (person.HealthValue == HealthLevel.Dead)
                    continue;

                // Apply damage to the person we calculated above.
                person.Damage(damagePerPerson);
            }
        }
    }
}